#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Verifica que los manuales y los DTO sigan coincidiendo con el modelo de datos.

Para que existe
---------------
Las migraciones 49 y 59 movieron varias claves primarias de `string Codigo` a `Guid Id` y de
vuelta. Cada ida y vuelta dejo rastros: manuales que describian una FK como texto cuando ya era
un Guid, comentarios del dominio apuntando a un `.Codigo` que no existe, y un DTO que viajaba
solo con su Codigo mientras el endpoint que lo consumia esperaba el Id. Ese ultimo dejo al
Tipificador sin poder registrar romaneos y no lo vio nadie hasta ejecutarlo.

Correr esto despues de cualquier migracion que toque claves primarias.

Uso
---
    python tools/verificar-consistencia.py

Devuelve 1 si hay desajustes duros (chequeos A o B). El chequeo C es heuristico: lista
candidatos para revisar a ojo, no falla la corrida.
"""
import glob
import io
import os
import re
import sys
import collections

RAIZ = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
DOMINIO = os.path.join(RAIZ, "source", "api", "Meat.Domain", "**", "*.cs")
MANUALES = os.path.join(RAIZ, "docs", "**", "*.md")
DTOS = [
    os.path.join(RAIZ, "source", "api", "Meat.Application", "**", "*.cs"),
    os.path.join(RAIZ, "source", "api", "Meat", "Controllers", "*.cs"),
]

PROP = re.compile(r"public\s+(?:virtual\s+)?([\w\.<>\?\[\]]+)\s+(\w+)\s*\{\s*get;\s*set;\s*\}")
CLASE = re.compile(r"\b(?:public|internal)\s+(?:sealed\s+|abstract\s+|partial\s+)*class\s+(\w+)")
# En los manuales las propiedades se listan como "- TropaId (Guid, FK)" o como snippet C#.
LISTA_MD = re.compile(r"^-\s*(\w*Id)\s*\((\w+\??)")


def norm(tipo):
    tipo = tipo.strip().replace("System.", "").rstrip("?")
    return {"String": "string", "Boolean": "bool", "Int32": "int", "Double": "double"}.get(tipo, tipo)


def leer(ruta):
    return io.open(ruta, encoding="utf-8-sig", errors="replace").read()


def es_generado(ruta):
    return os.sep + "bin" + os.sep in ruta or os.sep + "obj" + os.sep in ruta


def cargar_dominio():
    """Tipos reales de cada propiedad y entidades cuya PK es un Guid."""
    tipos = collections.defaultdict(set)
    pk_guid = set()
    for ruta in glob.glob(DOMINIO, recursive=True):
        txt = leer(ruta)
        for m in PROP.finditer(txt):
            tipos[m.group(2)].add(norm(m.group(1)))
        clave = re.search(r"\[Key\][\s\S]{0,120}?public\s+([\w\?]+)\s+(\w+)\s*\{", txt)
        if clave and norm(clave.group(1)) == "Guid":
            pk_guid.add(os.path.splitext(os.path.basename(ruta))[0])
    return tipos, pk_guid


def partir_clases(txt):
    marcas = [(m.start(), m.group(1)) for m in CLASE.finditer(txt)]
    for i, (pos, nombre) in enumerate(marcas):
        fin = marcas[i + 1][0] if i + 1 < len(marcas) else len(txt)
        yield nombre, txt[pos:fin]


def chequeo_a(tipos):
    """Manuales: cada propiedad *Id declarada contra el tipo real."""
    problemas, revisadas = [], 0
    for ruta in sorted(glob.glob(MANUALES, recursive=True)):
        for nro, linea in enumerate(leer(ruta).splitlines(), 1):
            L = linea.strip()
            pares = []
            m = LISTA_MD.match(L)
            if m:
                pares.append((m.group(1), m.group(2)))
            for m2 in re.finditer(r"public\s+(?:virtual\s+)?([\w\.<>\?]+)\s+(\w*Id)\s*\{\s*get;\s*set;\s*\}", L):
                pares.append((m2.group(2), m2.group(1)))

            for nombre, declarado in pares:
                reales = tipos.get(nombre)
                if not reales:
                    continue
                revisadas += 1
                if norm(declarado) not in reales:
                    problemas.append((ruta, nro, nombre, declarado, sorted(reales), L))
    return problemas, revisadas


def chequeo_b(tipos):
    """DTO y controllers: cada propiedad *Id contra el tipo real."""
    problemas, revisadas = [], 0
    for patron in DTOS:
        for ruta in sorted(glob.glob(patron, recursive=True)):
            if es_generado(ruta):
                continue
            for clase, cuerpo in partir_clases(leer(ruta)):
                for m in PROP.finditer(cuerpo):
                    nombre, tipo = m.group(2), norm(m.group(1))
                    if not nombre.endswith("Id") or nombre == "Id":
                        continue
                    reales = tipos.get(nombre)
                    if not reales:
                        continue
                    revisadas += 1
                    if tipo not in reales:
                        problemas.append((ruta, clase, nombre, tipo, sorted(reales)))
    return problemas, revisadas


def chequeo_c(pk_guid):
    """Heuristico: DTO que representa una entidad de PK Guid, expone Codigo y no expone Id.

    Es la forma que tenia TipificacionCandidata cuando rompio el Tipificador: el combo mandaba
    el Codigo y el endpoint esperaba el Id. Da falsos positivos legitimos (los Create no llevan
    Id porque la fila no existe todavia, y los catalogos globales tienen el Codigo como PK), asi
    que la lista se revisa a ojo.
    """
    candidatos = []
    for patron in DTOS:
        for ruta in sorted(glob.glob(patron, recursive=True)):
            if es_generado(ruta):
                continue
            for clase, cuerpo in partir_clases(leer(ruta)):
                props = {m.group(2) for m in PROP.finditer(cuerpo)}
                if "Codigo" not in props or "Id" in props:
                    continue
                for entidad in pk_guid:
                    if entidad.lower() in clase.lower() and clase.lower() != entidad.lower():
                        candidatos.append((ruta, clase, entidad))
                        break
    return candidatos


def rel(ruta):
    return os.path.relpath(ruta, RAIZ).replace("\\", "/")


def main():
    tipos, pk_guid = cargar_dominio()
    duros = 0

    problemas_a, revisadas_a = chequeo_a(tipos)
    print("A) Manuales: %d propiedades *Id verificadas, %d desajustes" % (revisadas_a, len(problemas_a)))
    for ruta, nro, nombre, declarado, reales, linea in problemas_a:
        print("   %s:%d" % (rel(ruta), nro))
        print("     %s: el manual dice '%s', el dominio tiene '%s'" % (nombre, declarado, "/".join(reales)))
        print("     %s" % linea)
    duros += len(problemas_a)

    problemas_b, revisadas_b = chequeo_b(tipos)
    print("\nB) DTO y controllers: %d propiedades *Id verificadas, %d desajustes"
          % (revisadas_b, len(problemas_b)))
    for ruta, clase, nombre, tipo, reales in problemas_b:
        print("   %s" % rel(ruta))
        print("     %s.%s es '%s', el dominio tiene '%s'" % (clase, nombre, tipo, "/".join(reales)))
    duros += len(problemas_b)

    candidatos = chequeo_c(pk_guid)
    print("\nC) DTO con Codigo y sin Id sobre entidades de PK Guid: %d para revisar" % len(candidatos))
    for ruta, clase, entidad in candidatos:
        print("   %-42s %s  (entidad %s)" % (clase, rel(ruta), entidad))
    if candidatos:
        print("\n   Revisar a ojo: es legitimo en los Create (la fila no existe todavia), en las")
        print("   busquedas por codigo y en los catalogos globales, cuya PK es el Codigo. No lo es")
        print("   si el DTO alimenta un combo cuyo valor despues viaja como Id.")

    print("\n%s" % ("Sin desajustes duros." if duros == 0 else "Hay %d desajuste(s) duro(s)." % duros))
    return 1 if duros else 0


if __name__ == "__main__":
    sys.exit(main())
