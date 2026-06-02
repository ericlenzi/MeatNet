# MeatNet

## Descripción del Proyecto
Sistema de gestión para frigoríficos. Permite administrar operaciones del negocio cárnico relacionadas al Ciclo I de producción, 
Desde el Ingreso de hacienda y Ubicación en corrales, hasta la Planificación, Ejecusión y Evaluación de la Faena.
La aplicación funciona sola, y debe estar preparada para integrarse con un ERP y con aplicaciones en los puestos de producción. 
Es una aplicación que corre en la nube, web (React-Vite), api (.net 8), bd propia (SQL Server)

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
- **Guia para CRUDs nuevos: `docs/BasisCRUD.md`** — Seguir SIEMPRE este documento al crear un CRUD de una entidad nueva. Contiene los patrones de backend (Entity, Handlers, Controller, migraciones) y frontend (Types, Service, Pages, rutas), incluyendo la regla obligatoria de filtro por empresa activa.

## Lo que NO hacer
- No modificar migraciones de EF Core ya aplicadas — crear una nueva migración
- No instalar paquetes sin consultar primero
- No cambiar la estructura de carpetas sin confirmación
