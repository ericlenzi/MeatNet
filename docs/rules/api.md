# Reglas — source/api (C# .NET 8)

## Solución y Proyectos

```
Meat.sln
├── Meat/                    # Host ASP.NET Core (entry point)
│   ├── Controllers/         # Controllers REST — heredan de MeatBaseController
│   ├── Infrastructure/      # Middleware, JWT config, Swagger, App Insights
│   ├── Services/            # Background services (MeatService)
│   └── Program.cs           # Composición y pipeline HTTP
│
├── Meat.Application/        # Lógica de negocio — Handlers MediatR + DTOs
│   ├── {Entidad}/           # Una carpeta por entidad
│   │   ├── Create{E}/       # Handler, Request, Response (y Mapper si aplica)
│   │   ├── Get{E}/
│   │   ├── Get{Es}/         # Listado
│   │   ├── Update{E}/
│   │   └── Delete{E}/
│   ├── Autenticacion/       # Login (genera JWT)
│   ├── Enums/               # Handlers para enums de dominio
│   └── Shared/              # Helpers, excepciones, settings, base classes
│
├── Meat.Domain/             # Entidades + Factories + Enums de dominio
│   └── {Entidad}/           # Clase entidad + Factory estática
│
├── Meat.Repositories/       # EF Core — DbContext, Migrations, TSQL scripts
│   ├── MeatContext.cs        # DbSets, Soft Delete global, SaveChanges override
│   └── Migrations/
│
├── Meat.Queries/            # Queries de lectura con Dapper/ADO (IDbConnection)
│
└── Meat.Infrastructure/     # Servicios externos (FTP, WCF, config endpoints)
```

### Dependencias entre proyectos
```
Meat (Host) → Meat.Application → Meat.Repositories → Meat.Domain
                               → Meat.Queries
                               → Meat.Infrastructure
```

## Patrones de Arquitectura

### CQRS liviano con MediatR
- Cada operación es un **Request/Response** despachado por MediatR
- Un **Handler** por operación (CreateSucursalHandler, GetSucursalesHandler, etc.)
- Los Controllers solo despachan: `await this.Handle(request)` vía `MeatBaseController`
- `MeatBaseController.Handle<T>()` resuelve automáticamente el status HTTP según el verbo (GET→200/404, POST→201, PUT/DELETE→204)

### Mapeo con AutoMapper
- Profiles dentro de cada carpeta de operación (ej: `CreateSucursalMapperProfile`)
- Se registran por assembly scan desde `Program.cs`

### Factories en Domain
- Cada entidad tiene una `{Entidad}Factory` con métodos `Create()` que asignan `Id = Guid.NewGuid()` y valores por defecto
- Las entidades usan **Guid** como PK, generada en el servidor (no auto-increment)

### Soft Delete global
- `MeatContext` agrega shadow property `FechaBaja` (DateTime?) a **todas** las entidades
- Query filter global: solo devuelve registros con `FechaBaja == null`
- Al eliminar, EF intercepta el `Delete` y lo convierte en `Update FechaBaja = DateTime.Now`
- Para borrar físicamente hay que bypassear el filtro

### Queries de lectura (Meat.Queries)
- Proyecto separado para consultas complejas/reportes que usan `IDbConnection` (ADO.NET/Dapper)
- No pasa por EF Core — útil para consultas de solo lectura con SQL directo

## Stack Técnico

| Componente         | Tecnología                      | Versión  |
|--------------------|---------------------------------|----------|
| Runtime            | .NET 8                          | 8.0      |
| ORM                | Entity Framework Core           | 8.0.0    |
| Base de datos      | SQL Server (Microsoft.Data.SqlClient) | 5.2.0 |
| Mediator           | MediatR                         | 12.4.1   |
| Mapper             | AutoMapper                      | 16.1.1   |
| Autenticación      | JWT Bearer                      | 8.0.0    |
| Serialización      | Newtonsoft.Json                  | 13.0.3   |
| Documentación API  | Swashbuckle (Swagger)           | 6.9.0    |
| Observabilidad     | Application Insights            | 2.22.0   |
| PDFs               | iTextSharp                      | 5.5.13.3 |
| Excel              | DotNetCore.NPOI                 | 1.2.3    |
| Códigos de barra   | ZXing.Net                       | 0.16.8   |
| Servicios externos | WCF (System.ServiceModel)       | 4.8.1    |

## Entidades registradas en MeatContext

Empresas, Sucursales, Parametros, ParametrosSucursales, Puestos, Establecimientos,
Roles, Usuarios, UsuariosSucursales, Provincias, Almacenes, Materiales, Especies,
TiposEmpresas, TiposAlmacenes, TiposSexos, TiposEspecies, AlmacenesMateriales

## Autenticación y Autorización
- JWT con clave simétrica (HMAC SHA-256), configurada en `JwtOptions:SigninKey`
- Roles en claims: controllers usan `[Authorize(Roles = "Admin, Abastecimiento")]`
- `MeatBaseController.CurrentUser` extrae Id, UserName, RolId, CodigoEmpresa del token
- Passwords hasheados con SHA1 (legacy)

## Convenciones C#
- Clases y métodos en **PascalCase**
- Variables locales y parámetros en **camelCase**
- Interfaces con prefijo `I` (ej: `IFtpService`)
- Async/await en todos los métodos que accedan a BD o I/O
- Namespaces siguen la estructura de carpetas

## Convenciones de Entidades
- PKs: `Guid`, generadas con `Guid.NewGuid()` en la Factory
- Soft delete: no agregar `FechaBaja` a la entidad — lo maneja `MeatContext` como shadow property
- `FechaActualizacion` con default SQL `getdate()` donde aplique
- Data Annotations para PK (`[Key]`, `[DatabaseGenerated(None)]`)

## API REST
- Rutas: `[Route("[controller]")]` — se infiere del nombre del controller (PascalCase plural)
- Controllers heredan de `MeatBaseController` — no manejar responses manualmente
- Para crear un nuevo endpoint CRUD: crear carpeta en Application, Handler, Request, Response, y agregar acción al Controller

## Migraciones EF Core
- Assembly de migraciones: `Meat.Repositories`
- Se aplican automáticamente en `Program.cs` con `context.Database.Migrate()`
- **Nunca modificar una migración ya aplicada** — crear una nueva

## Ambientes
- Configuración por ambiente: `appsettings.{Environment}.json` (Development, Integration, Testing, Production)
- `IsApiLocal`: flag booleano que controla si corre el BackgroundService y App Insights

## Middleware
- `ExceptionHandlerMiddleware`: captura excepciones y devuelve JSON con status code apropiado
  - `ValidationException` → 400
  - `ResourceNotFoundException` → 404
  - Otras → 500

## Comandos frecuentes
```bash
# Desde source/api/
dotnet run --project Meat          # Levantar API
dotnet build                       # Compilar solución
dotnet ef migrations add <Nombre> --project Meat.Repositories --startup-project Meat
dotnet ef database update --project Meat.Repositories --startup-project Meat
```
