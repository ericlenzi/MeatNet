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

### SQL directo
- No hay proyecto de consultas aparte ni `IDbConnection` registrado: todo pasa por `MeatContext`.
- Cuando hace falta SQL a mano (`context.Database.ExecuteSqlRawAsync` / `SqlQueryRaw`, o
  `migrationBuilder.Sql`) va en dialecto PostgreSQL: ver `docs/infraestructure.md`. Ese SQL **no**
  aplica los query filters, así que el filtro por empresa y `FechaBaja` se escriben explícitos.

## Stack Técnico

| Componente         | Tecnología                      | Versión  |
|--------------------|---------------------------------|----------|
| Runtime            | .NET 8                          | 8.0      |
| ORM                | Entity Framework Core           | 8.0.11   |
| Base de datos      | PostgreSQL (Npgsql.EntityFrameworkCore.PostgreSQL) | 8.0.11 |
| Mediator           | MediatR                         | 12.4.1   |
| Mapper             | AutoMapper                      | 16.1.1   |
| Autenticación      | JWT Bearer (IdentityModel 8.14.0) | 8.0.31 |
| Serialización      | Newtonsoft.Json                  | 13.0.3   |
| Documentación API  | Swashbuckle (Swagger)           | 6.9.0    |
| Observabilidad     | Application Insights            | 2.22.0   |
| PDFs               | iTextSharp                      | 5.5.13.3 |
| Excel              | DotNetCore.NPOI                 | 1.2.3    |
| Códigos de barra   | ZXing.Net                       | 0.16.8   |
| Servicios externos | WCF (System.ServiceModel)       | 4.8.1    |

## Entidades registradas en MeatContext

Empresas, Sucursales, Parametros, Clientes, ClientesEstablecimientos, Establecimientos,
EstablecimientosEspecies, Roles, Usuarios, UsuariosSucursales, UsuariosEstablecimientos,
Provincias, Puestos, Almacenes, Materiales, Especies, TiposEmpresas, TiposClientes,
TiposAlmacenes, TiposSexos, TiposEspecies, EmpresasTiposEspecies, OrigenesHaciendas,
UsosHaciendas, TiposMateriales, UnidadesMedidas, NumeradoresTropas, TiposEstadosIngresos,
TiposEstadosTropas, TiposEstadosHacienda, IngresosHaciendas, IngresosHaciendasPesadas,
IngresosHaciendasUbicaciones, Tropas, TropasMovimientos, TiposEstadosListasMatanzas,
ListasMatanzas, ListasMatanzasDetalles, ListasMatanzasMovimientos, TiposPuestos,
TiposMagnitudes, TiposMediciones, TiposPuestos, Puestos, Tipificadores,
DestinosComerciales, TipificacionesOficiales, Conformaciones,
GradosEngrasamiento, Denticiones, TiposContusiones, MotivosDecomisos, Numeradores,
UnidadesFaenas, Tipificaciones,
DespiecesMateriales, Romaneos, RomaneosPiezas, RomaneosPiezasMediciones,
TiposMovimientosCamaras, MovimientosCamaras

> Son los 57 `DbSet` de `MeatContext`. La lista anterior habia quedado en 19 e incluia
> `AlmacenesMateriales`, que se elimino en la migracion 31, y `TiposDenticiones`, que se
> elimino con la migracion 68.

## Autenticación y Autorización
- JWT con clave simétrica (HMAC SHA-256), configurada en `JwtOptions:SigninKey`
- Roles en claims: controllers usan `[Authorize(Roles = "ABASTADMIN,ADMIN")]`. Los roles vigentes
  son `SUPERADMIN`, `ADMIN`, `ABASTADMIN`, `ABAST`, `FAENAADMIN` y `FAENA`.
- `MeatBaseController.CurrentUser` extrae Id, UserName, RolId, CodigoEmpresa del token
- Passwords con **PBKDF2 + salt por usuario**. El SHA1 legacy se elimino.

## Convenciones C#
- Clases y métodos en **PascalCase**
- Variables locales y parámetros en **camelCase**
- Interfaces con prefijo `I` (ej: `IFtpService`)
- Async/await en todos los métodos que accedan a BD o I/O
- Namespaces siguen la estructura de carpetas

## Verificacion de consistencia

Despues de cualquier migracion que toque **claves primarias**, correr:

```bash
python tools/verificar-consistencia.py
```

Compara los manuales y los DTO contra los tipos reales del dominio. Las migraciones 49 y 59
movieron varias PK de `string Codigo` a `Guid Id` y de vuelta, y cada ida y vuelta dejo rastros
que solo aparecieron mucho despues: manuales desactualizados, comentarios apuntando a un
`.Codigo` inexistente y un DTO que rompio el Tipificador sin que nadie lo notara.

## Convenciones de Entidades
- PKs: depende del tipo de tabla. `Guid` generada en la Factory para las tablas propias de una
  empresa, `string Codigo` para los catalogos globales. La regla completa (y el tercer caso,
  catalogo global + configuracion por empresa) esta en `CLAUDE.md` y en `docs/BasisCRUD.md` §4;
  el `MeatContext` la valida al construir el modelo
- Soft delete: no agregar `FechaBaja` a la entidad — lo maneja `MeatContext` como shadow property
- `FechaActualizacion` la asigna la API (`DateTime.Now`), en la factory o en el handler. La base no
  genera fechas (sin `now()` ni defaults de fecha): la API es el único reloj
- Filtros de texto de los listados con `EF.Functions.ILike(columna, Busqueda.Contiene(request.Filter))`,
  no con `.Contains(...)`: PostgreSQL distingue mayúsculas
- Códigos que tipea el usuario con índice único: columna `citext` en la región `PostgreSQL` de `MeatContext`
- Data Annotations para PK (`[Key]`, `[DatabaseGenerated(None)]`)

## API REST
- Rutas: `[Route("[controller]")]` — se infiere del nombre del controller (PascalCase plural)
- Controllers heredan de `MeatBaseController` — no manejar responses manualmente
- Para crear un nuevo endpoint CRUD: crear carpeta en Application, Handler, Request, Response, y agregar acción al Controller

## Migraciones EF Core
- Assembly de migraciones: `Meat.Repositories`, schema `meat` de PostgreSQL
- Se aplican automáticamente en `Program.cs` con `context.Database.Migrate()`
- **Nunca modificar una migración ya aplicada** — crear una nueva
- Línea base: `01_InitialPostgres` (esquema) y `02_SeedCatalogos` (catálogos globales, empresa
  administrativa y superadmin). El historial de SQL Server (migraciones 1 a 79) quedó en git.
- Detalle y reglas del SQL a mano: `docs/infraestructure.md`

## Ambientes
- Configuración por ambiente: solo `appsettings.Development.json` y `appsettings.Production.json`.
  En producción los secretos van por variables de entorno (ver `docs/infraestructure.md`)
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
