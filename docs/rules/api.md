# Reglas — source/api (C# .NET)

## Arquitectura
- Patrón preferido: **Clean Architecture** o **N-Capas** según lo existente en el proyecto
- Separar: Controllers → Services → Repositories → Models
- DTOs para toda comunicación entrada/salida de endpoints

## Convenciones C#
- Clases y métodos en **PascalCase**
- Variables locales y parámetros en **camelCase**
- Interfaces con prefijo `I` (ej: `IProductService`)
- Async/await en todos los métodos que accedan a BD o I/O

## Entity Framework Core + SQL Server
- Usar migraciones de EF Core — nunca modificar la BD a mano
- Nombres de tablas en **PascalCase singular** (ej: `Product`, `SaleOrder`)
- Siempre incluir `CreatedAt` y `UpdatedAt` en entidades principales
- Usar `decimal(18,2)` para precios y pesos de cortes

## API REST
- Rutas en **kebab-case** y plural (ej: `/api/products`, `/api/sale-orders`)
- Respuestas consistentes: usar un wrapper `ApiResponse<T>` si ya existe
- Códigos HTTP correctos: 200, 201, 400, 404, 500
- Validaciones con **Data Annotations** o **FluentValidation**

## Dominio Cárnico (contexto)
- Entidades clave probables: `Productos` (cortes), `Empresas`, `Sucursales`, `Establecimientos`, `Almacenes`, `Stock`
- Los pesos siempre en **kilogramos** como unidad base
- Precios siempre con IVA explícito o indicar si es neto

## Comandos frecuentes
```bash
dotnet run                        # Levantar API
dotnet ef migrations add <Nombre> # Nueva migración
dotnet ef database update         # Aplicar migraciones
dotnet test                       # Correr tests
```
