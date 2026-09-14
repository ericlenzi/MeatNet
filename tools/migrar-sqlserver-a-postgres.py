#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Copia los datos de la base SQL Server de desarrollo a la base PostgreSQL local.

Para que existe
---------------
MeatNet se paso de SQL Server a PostgreSQL (ver docs/infraestructure.md). El historial de
migraciones no se podia reutilizar, asi que Postgres arranca con una linea base nueva y los
datos de ejemplo de desarrollo (proveedores Lenz y Diaz con existencia en camara, jornadas,
romaneos) se traen con este script. SQL Server solo se lee: nunca se modifica.

Como se corre
-------------
1. Crear el esquema en Postgres:  dotnet ef database update  (desde source/api/Meat)
2. Instalar dependencias:         python -m pip install pyodbc "psycopg[binary]"
3. Correr:                        python tools/migrar-sqlserver-a-postgres.py

La conexion a Postgres sale de ConnectionStrings:Default de source/api/Meat/appsettings.Development.json.
SQL Server se lee con autenticacion de Windows; se puede cambiar con --sqlserver y --base.

Que hace
--------
- Toma las tablas del schema meat de Postgres y las ordena por sus FKs (padres primero).
- Copia cada tabla desde SQL Server con INSERT ... ON CONFLICT DO NOTHING, asi convive con los
  catalogos que ya sembro la migracion y se puede correr mas de una vez.
- Incluye las filas dadas de baja (FechaBaja): otras filas pueden apuntarles.
- Todo va en una transaccion: si una tabla falla, Postgres queda como estaba.
- Al final compara la cantidad de filas por tabla y devuelve 1 si alguna no coincide.
"""
import argparse
import os
import re
import sys

import psycopg
import pyodbc

RAIZ = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
APPSETTINGS = os.path.join(RAIZ, "source", "api", "Meat", "appsettings.Development.json")
SCHEMA = "meat"
EXCLUIDAS = {"__EFMigrationsHistory"}


def conexion_postgres():
    with open(APPSETTINGS, encoding="utf-8-sig") as f:
        contenido = f.read()
    m = re.search(r'"Default"\s*:\s*"(Host=[^"]+)"', contenido)
    if not m:
        sys.exit(f"No se encontro una connection string de PostgreSQL en {APPSETTINGS}")

    partes = dict(p.split("=", 1) for p in m.group(1).split(";") if "=" in p)
    claves = {k.lower(): v for k, v in partes.items()}
    if claves.get("password") in (None, "", "COMPLETAR"):
        sys.exit("Completar la password de PostgreSQL en appsettings.Development.json")

    return psycopg.connect(
        host=claves.get("host", "localhost"),
        port=claves.get("port", "5432"),
        dbname=claves.get("database"),
        user=claves.get("username"),
        password=claves.get("password"),
    )


def tablas_en_orden(pg):
    tablas = [r[0] for r in pg.execute(
        "SELECT table_name FROM information_schema.tables "
        "WHERE table_schema = %s AND table_type = 'BASE TABLE' ORDER BY table_name", (SCHEMA,))
        if r[0] not in EXCLUIDAS]

    deps = {t: set() for t in tablas}
    for hijo, padre in pg.execute("""
            SELECT hijo.relname, padre.relname
            FROM pg_constraint c
            JOIN pg_class hijo ON hijo.oid = c.conrelid
            JOIN pg_class padre ON padre.oid = c.confrelid
            JOIN pg_namespace n ON n.oid = hijo.relnamespace
            WHERE c.contype = 'f' AND n.nspname = %s""", (SCHEMA,)):
        if hijo in deps and padre in deps and hijo != padre:
            deps[hijo].add(padre)

    orden = []
    while deps:
        listas = sorted(t for t, d in deps.items() if not (d - set(orden)))
        if not listas:
            sys.exit(f"Ciclo de FKs entre: {sorted(deps)}")
        orden.extend(listas)
        for t in listas:
            del deps[t]
    return orden


def columnas_postgres(pg, tabla):
    return [r[0] for r in pg.execute(
        "SELECT column_name FROM information_schema.columns "
        "WHERE table_schema = %s AND table_name = %s ORDER BY ordinal_position", (SCHEMA, tabla))]


def columnas_sqlserver(ss, tabla):
    return {r[0] for r in ss.cursor().execute(
        "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = ?", tabla).fetchall()}


def main():
    parser = argparse.ArgumentParser(description=__doc__.split("\n\n")[0])
    parser.add_argument("--sqlserver", default=r".\sqlexpress", help="instancia de SQL Server")
    parser.add_argument("--base", default="MeatNet", help="base de SQL Server")
    args = parser.parse_args()

    ss = pyodbc.connect(
        "DRIVER={ODBC Driver 17 for SQL Server};"
        f"SERVER={args.sqlserver};DATABASE={args.base};Trusted_Connection=yes;")

    comparacion = []
    with conexion_postgres() as pg:
        orden = tablas_en_orden(pg)

        with pg.transaction():
            for tabla in orden:
                destino = columnas_postgres(pg, tabla)
                origen = columnas_sqlserver(ss, tabla)
                if not origen:
                    sys.exit(f"{tabla}: no existe en SQL Server")
                faltantes = [c for c in destino if c not in origen]
                if faltantes:
                    sys.exit(f"{tabla}: columnas de Postgres que no estan en SQL Server: {faltantes}")

                lista = ", ".join(f"[{c}]" for c in destino)
                filas = ss.cursor().execute(f"SELECT {lista} FROM dbo.[{tabla}]").fetchall()

                if filas:
                    insert = (f'INSERT INTO {SCHEMA}."{tabla}" (' + ", ".join(f'"{c}"' for c in destino) + ") "
                              f"VALUES ({', '.join(['%s'] * len(destino))}) ON CONFLICT DO NOTHING")
                    with pg.cursor() as cur:
                        cur.executemany(insert, [tuple(f) for f in filas])

                en_postgres = pg.execute(f'SELECT COUNT(*) FROM {SCHEMA}."{tabla}"').fetchone()[0]
                comparacion.append((tabla, len(filas), en_postgres))
                print(f"  {tabla:<32} {len(filas):>6}")

    print(f"\n{'Tabla':<32} {'SQL Server':>10} {'Postgres':>10}")
    distintas = 0
    for tabla, en_sqlserver, en_postgres in comparacion:
        marca = "" if en_sqlserver == en_postgres else "   <-- NO COINCIDE"
        distintas += bool(marca)
        print(f"{tabla:<32} {en_sqlserver:>10} {en_postgres:>10}{marca}")

    print(f"\n{len(comparacion)} tablas, {distintas} con diferencias.")
    return 1 if distintas else 0


if __name__ == "__main__":
    sys.exit(main())
