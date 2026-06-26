# MeatNet

## Descripción del Proyecto
Sistema de gestión para frigoríficos que permite administrar operaciones del negocio cárnico relacionadas al Ciclo I de producción.
La aplicación de tecnología web funciona en la nube, y debe estar preparada para integrarse con un ERP externo y con aplicaciones de captura de datos en los puestos de producción. 
La tecnología de desarrollo básicamente se resume para el frontend (React-Vite), backend api (.net 8) y bd propia (SQL Server).

## Ambito del Proyecto
El proceso productivo de la cadena de valor cárnica en Argentina se organiza en tres bloques, cada uno dependiente del anterior: 
- Campo (producción primaria)
- Industria Frigorífica (industrialización)
- Mercado (comercialización y distribución)
Nuestro sistema se centra en la gestión para frigoríficos, donde el ser vivo se convierte en alimento perecedero, dividiéndose en subproductos y carne apta para el consumo.

El proceso industrial del frigorífico completo abarca el Ciclo I (Faena) y el Ciclo II (Despostada):
- Ciclo I: Se transforma la hacienda en "media res". 
- Ciclo II: Se desarma esa "media res" en cortes primarios o minoristas envasados al vacío.
Nuestro sistema se centra en la gestión del Ciclo I, haciendo foco en la Recepción de hacienda, Linea de faena, Tipificación y romaneo y Cámaras de enfriamiento.

## Modelo de Negocio
La aplicación opera para **una única Empresa**, que puede tener múltiples **Sucursales** y múltiples **Establecimientos** (plantas de faena). 
Cada Establecimiento está asociado a una Sucursal y puede operar con distintas **Especies** a través de **EstablecimientosEspecies**, lo que hace al sistema **multiespecie**.

Entidades que sí pertenecen a la estructura organizacional:
`Empresa` → `Sucursal` → `Establecimiento` → `EstablecimientoEspecie`

## Estructura del Repositorio
```
MeatNet/
├── docs/           # Documentación: arquitectura, decisiones, guías
│   ├── rules/      # api.md, web.md
│   └── manuales/   # Detalles de los procesos 
├── source/
│   ├── api/        # Backend C# .NET (ver /docs/rules/api.md)
│   └── web/        # Frontend React / Next.js (ver docs/rules/web.md)
```

## Stack
- **API:** C# .NET — ver `source/api/`
- **Web:** React + Next.js — ver `source/web/`
- **Base de datos:** SQL Server
- **ORM:** Entity Framework Core (asumir si no se indica lo contrario)

## Convenciones Generales
- Código en **inglés** (variables, funciones, clases, comentarios técnicos)
- Commits y documentación en **español**
- Siempre respetar la separación `api/` y `web/` — no mezclar responsabilidades
- Antes de crear un archivo nuevo, verificar si ya existe algo similar en el proyecto

## Documentación
- Arquitectura y decisiones técnicas: `docs/`
- Reglas específicas de API: `.claude/rules/api.md`
- Reglas específicas de Web: `.claude/rules/web.md`
- **Guia para CRUDs nuevos: `docs/BasisCRUD.md`** — Seguir SIEMPRE este documento al crear un CRUD de una entidad nueva. Contiene los patrones de backend (Entity, Handlers, Controller, migraciones) y frontend (Types, Service, Pages, rutas). Las entidades globales no llevan EmpresaId ni filtro por empresa.

## Lo que NO hacer
- No modificar migraciones de EF Core ya aplicadas — crear una nueva migración
- No instalar paquetes sin consultar primero
- No cambiar la estructura de carpetas sin confirmación
