# Infraestructura de MeatNet

Información básica de infraestructura: entornos, base de datos, configuración, migraciones y deploy.
Consultarlo antes de tocar la conexión, los appsettings, las migraciones o cualquier SQL escrito a mano.

## 1. Entornos

| Entorno | API | Base de datos | Configuración |
|---|---|---|---|
| **Development** | Local (`dotnet run --project Meat`, puerto 5822) | PostgreSQL local, `localhost:5432`, base `meatnet` | `appsettings.Development.json` |
| **Production** | VPS *(pendiente de deploy)* | Supabase, proyecto propio de MeatNet (São Paulo), base `postgres`, schema `meat` | `appsettings.Production.json` + variables de entorno |

Solo existen esos dos entornos y esos dos archivos de configuración en `source/api/Meat/`.
`launchSettings.json` levanta la API en `Development`, y `dotnet ef` también usa ese entorno
(no hay `IDesignTimeDbContextFactory`).

## 2. Base de datos

MeatNet usa **PostgreSQL** con **Entity Framework Core 8** y el proveedor
`Npgsql.EntityFrameworkCore.PostgreSQL` (8.0.11). Hasta la migración 79 usaba SQL Server Express;
se cambió porque SQL Server en la nube es mucho más caro que PostgreSQL.

| Aspecto | Valor | Dónde se define |
|---|---|---|
| Versión local | PostgreSQL 18 | Instalación |
| Versión en Supabase | PostgreSQL 17.6 | Supabase |
| Schema | `meat` | `MeatContext` (`HasDefaultSchema`) |
| Extensión | `citext` (local: schema `public`; Supabase: schema `extensions`) | `MeatContext` (`HasPostgresExtension`) y §6.1 |
| Collation del texto | `es-AR-x-icu` | `MeatContext.AplicarTiposPostgres` |
| Fechas | `timestamp without time zone`, hora local de Argentina | `MeatContext.AplicarTiposPostgres` |
| Zona horaria del servidor | La que venga por default: no se usa | — |

**No usar nada exclusivo de PostgreSQL 18**: producción puede estar en una versión anterior.

### Por qué cada decisión

- **Schema `meat`, no `public`.** Supabase expone el schema `public` por su Data API (REST). El
  aislamiento por empresa lo garantiza `MeatContext`, no la base (no hay RLS), así que las tablas no
  pueden quedar en un schema expuesto. Un schema nuevo no se expone salvo que se lo agregue en
  *Exposed schemas* y se den permisos a `anon`/`authenticated`; además, en el proyecto de MeatNet la
  Data API está deshabilitada.
- **Toda la configuración regional vive en el modelo, no en el servidor.** Así la base queda igual en
  local y en Supabase sin depender de cómo se instaló cada una. No se configura nada a mano en el
  servidor.
- **`citext` en los códigos que tipea el usuario** (`Usuario.UserName`, `CodigoSucursal`,
  `CodigoEstablecimiento`, `CodigoCliente`, `CodigoPuesto`, `Tipificador.Matricula`,
  `CodigoMaterial` y los `Codigo` de `Numerador`, `Parametro`, `DestinoComercial`, `UnidadFaena` y
  `Tipificacion`). SQL Server no distinguía mayúsculas (collation `Modern_Spanish_CI_AS`), así que el
  login y los índices únicos trataban `NOV` y `nov` como iguales. PostgreSQL sí las distingue;
  `citext` conserva el comportamiento anterior. Los códigos de los catálogos globales (PK `Codigo`)
  quedan sensibles a mayúsculas: los administra el SUPERADMIN y se usan en mayúsculas.
- **Collation `es-AR-x-icu` en el resto del texto.** Sin ella, `ORDER BY "Nombre"` ordena por byte:
  mayúsculas antes que minúsculas, y los acentos y la ñ al final. Con ella ordena como SQL Server.
- **La API es el único reloj.** Todas las fechas las pone .NET (`DateTime.Now`); la base no tiene
  defaults `now()` ni SQL que genere fechas. Por eso la zona horaria del servidor no importa (Supabase
  usa UTC). Lo que sí importa es la zona horaria de la máquina donde corre la API (ver §6).

## 3. Configuración

### Development

`appsettings.Development.json`:

```json
"ConnectionStrings": {
  "Default": "Host=localhost;Port=5432;Database=meatnet;Username=meatnet;Password=<password de dev>"
}
```

Es una password solo para desarrollo, de una base que escucha en la máquina local.

### Production

Los secretos **no** van en `appsettings.Production.json`: se configuran como variables de entorno del
servicio en el VPS.

| Variable | Qué es |
|---|---|
| `ConnectionStrings__Default` | Connection string de Supabase |
| `JwtOptions__SigninKey` | Clave de firma JWT (la API no arranca con una de las claves versionadas en el repo) |
| `Directories__Images` | Carpeta de imágenes (ruta Linux) |
| `TZ` | `America/Argentina/Buenos_Aires` |

## 4. Preparar PostgreSQL local

Se hace una sola vez, en pgAdmin 4 (Query Tool sobre la base `postgres`, conectado con el superusuario):

```sql
CREATE ROLE meatnet WITH LOGIN PASSWORD '<password de dev>';
CREATE DATABASE meatnet OWNER meatnet ENCODING 'UTF8' TEMPLATE template0;
```

- El rol `meatnet` es dueño de la base: puede crear el schema `meat` y la extensión `citext`, y no hace
  falta el superusuario `postgres` en la aplicación (en Supabase tampoco se tiene).
- `TEMPLATE template0` con `UTF8` deja la base limpia sin importar la configuración regional de Windows.
- Verificar que exista la collation: `SELECT collname FROM pg_collation WHERE collname = 'es-AR-x-icu';`

Después, desde `source/api/`:

```bash
dotnet ef database update --project Meat.Repositories --startup-project Meat
```

(Al arrancar, la API también aplica las migraciones pendientes con `context.Database.Migrate()`.)

## 5. Migraciones

El flujo con EF Core es el de siempre:

```bash
# Desde source/api/
dotnet ef migrations add <NN_Nombre> --project Meat.Repositories --startup-project Meat
dotnet ef database update --project Meat.Repositories --startup-project Meat
dotnet ef migrations script --idempotent --project Meat.Repositories --startup-project Meat -o deploy.sql
```

- **Línea base:** `01_InitialPostgres` (esquema completo) y `02_SeedCatalogos` (catálogos globales, la
  empresa administrativa `0` y el usuario `superadmin`, con los valores que tenía SQL Server al
  migrar). El historial de SQL Server (migraciones 1 a 79) quedó en git.
- **Nunca escribir una migración a mano:** siempre `dotnet ef migrations add`, que también genera el
  `.Designer.cs` y el snapshot. Si hace falta SQL, se agrega con `migrationBuilder.Sql(...)` dentro de
  la migración generada.
- **No usar `--no-build`** con `migrations add` cuando hay migraciones sin compilar: EF usa el
  ensamblado viejo, no ve el snapshot nuevo y vuelve a generar el esquema completo.

### Reglas del SQL escrito a mano

Aplican a `migrationBuilder.Sql`, `HasFilter`, `ExecuteSqlRawAsync` y `SqlQueryRaw`:

| SQL Server | PostgreSQL |
|---|---|
| `[FechaBaja]` | `"FechaBaja"` — PostgreSQL pasa a minúsculas los nombres sin comillas |
| `Numeradores` | `meat."Numeradores"` — con el schema |
| `1` / `0` en columnas `bit` | `true` / `false` |
| `NEWID()` | `gen_random_uuid()` |
| `GETDATE()` / `SYSDATETIME()` | **no usar**: la fecha la manda la API como parámetro |
| `IF NOT EXISTS (...) INSERT` | `INSERT ... ON CONFLICT DO NOTHING` o `INSERT ... SELECT ... WHERE NOT EXISTS` |
| `LIKE` (no distinguía mayúsculas) | `ILIKE` |

Además, el SQL a mano **no pasa por los query filters**: el filtro por empresa y `"FechaBaja" IS NULL`
se escriben explícitos. En C#, el SQL va en strings verbatim (`@"..."`) con las comillas dobles
duplicadas (`meat.""Numeradores""`). Ejemplo de referencia: `Meat.Application/Numeradores/Correlativos.cs`.

### Búsquedas de texto

Los listados filtran con `ILike`, no con `Contains`:

```csharp
queryable = queryable.Where(x =>
    EF.Functions.ILike(x.Nombre, Busqueda.Contiene(request.Filter)) ||
    EF.Functions.ILike(x.Codigo, Busqueda.Contiene(request.Filter)));
```

`Busqueda.Contiene` (`Meat.Application/Shared/Busqueda.cs`) arma el patrón `%texto%` y escapa los
comodines `%` y `_` que tipee el usuario.

## 6. Producción

### 6.1 Base de datos en Supabase *(hecho el 2026-09-14)*

**Proyecto.** MeatNet tiene su **propio proyecto** de Supabase, separado de cualquier otro de la
organización. En Supabase cada proyecto es una sola base, y compartirla implicaría que un restore de
backup (que vuelve atrás la base entera y deja el proyecto inaccesible mientras dura), la pausa por
inactividad, los recursos y la password de `postgres` afecten a los dos proyectos.

| Aspecto | Valor |
|---|---|
| Región | South America (São Paulo) |
| PostgreSQL | 17.6 |
| Plan de la organización | Free: pausa tras 1 semana sin actividad, sin backups, 500 MB. **Pasar a Pro antes del uso real** |
| Data API | Deshabilitada (MeatNet no la usa) |
| Project ref | En la URL del dashboard: `supabase.com/dashboard/project/<project-ref>` |

**Preparación de la base** (una sola vez, en *SQL Editor*, como `postgres`). La password de `meatnet`
es propia de producción, se guarda en un gestor de contraseñas y no debe contener `;`. No guardar la
query como snippet.

```sql
-- citext en el schema extensions, como recomienda Supabase
create extension if not exists citext with schema extensions;

-- Rol de la aplicacion: las migraciones y la API se conectan con este usuario
create role meatnet with login password '<PASSWORD>';

-- Puede crear el schema meat (las migraciones lo crean) y usar las extensiones
grant create on database postgres to meatnet;
grant usage on schema extensions to meatnet;

-- Encuentra sus tablas y el tipo citext sin anteponer el schema
alter role meatnet set search_path = meat, public, extensions;

-- El administrador (postgres) puede ver y administrar lo que cree meatnet
grant meatnet to postgres;
```

El `search_path` del rol es imprescindible: si no incluye `extensions`, los operadores de `citext`
no se encuentran y las comparaciones de `UserName` y los códigos pasan a distinguir mayúsculas **sin
dar error**. La migración ejecuta `CREATE EXTENSION IF NOT EXISTS citext`, que no hace nada porque la
extensión ya existe.

**Conexión.**

| Desde | Conexión | Host y usuario |
|---|---|---|
| PC de desarrollo (IPv4) | Session pooler, puerto 5432 | `aws-0-sa-east-1.pooler.supabase.com`, usuario `meatnet.<project-ref>` |
| VPS | Directa (IPv6), puerto 5432 — recomendada por Supabase para servidores persistentes. Si el VPS solo tiene IPv4, session pooler | `db.<project-ref>.supabase.co`, usuario `meatnet` |

**No usar *transaction mode*** (puerto 6543): no soporta prepared statements. Todos los datos de
conexión salen del botón **Connect** del dashboard. Formato para Npgsql:
`Host=<host>;Port=5432;Database=postgres;Username=<usuario>;Password=<password>;SSL Mode=Require`.

**Aplicar migraciones.** Desde la raíz del repo, en PowerShell, sin que la password quede en archivos
ni en el historial:

```powershell
$secure = Read-Host "Password de meatnet" -AsSecureString
$password = [Runtime.InteropServices.Marshal]::PtrToStringAuto([Runtime.InteropServices.Marshal]::SecureStringToBSTR($secure))
$conn = "Host=aws-0-sa-east-1.pooler.supabase.com;Port=5432;Database=postgres;Username=meatnet.<project-ref>;Password=$password;SSL Mode=Require"
dotnet ef database update --project source\api\Meat.Repositories --startup-project source\api\Meat --connection $conn
Remove-Variable secure, password, conn
```

`dotnet ef` no lista las migraciones que aplica: termina en `Done.`. Verificar siempre en Supabase.

**Verificación** (en *SQL Editor*). Resultado al hacer el deploy inicial:

| Consulta | Resultado |
|---|---|
| `select "MigrationId" from meat."__EFMigrationsHistory"` | `01_InitialPostgres`, `02_SeedCatalogos` |
| Tablas del schema `meat` (`pg_tables`) | 61, dueño `meatnet` |
| Especies / TiposEspecies / MotivosDecomisos / Roles / Empresas / Usuarios | 6 / 13 / 28 / 6 / 1 / 1 |
| Columnas `citext` / con collation `es-AR-x-icu` | 12 / 203 |
| `set search_path = meat, public, extensions;` y buscar `UserName = 'SUPERADMIN'` | `superadmin` |

### 6.2 API en el VPS *(pendiente)*

- **Password del `superadmin`:** el seed copió el hash de la base de desarrollo, así que en producción
  entra con la password de desarrollo. Cambiarla **antes** de abrir la API a otros usuarios.
- **Región:** un VPS cercano a São Paulo. El Monitor de Faena refresca seguido y cada consulta suma la
  latencia de red.
- **Migraciones:** decidir si se aplican a mano (§6.1) o con el `context.Database.Migrate()` de
  `Program.cs`, que hoy las aplica al arrancar.
- **Zona horaria:** un servidor Linux corre en UTC y `DateTime.Now` devolvería 3 horas de más.
  Configurar `TZ=America/Argentina/Buenos_Aires` en el servicio.
- **Secretos y rutas:** variables de entorno de §3. `Directories:Images` hoy apunta a una ruta de Windows.

## 7. Script de migración de datos desde SQL Server

`tools/migrar-sqlserver-a-postgres.py` copió los datos de desarrollo de SQL Server a PostgreSQL al
hacer el cambio. Queda por si hay que repetirlo (por ejemplo, para recrear la base local).

```bash
python -m pip install pyodbc "psycopg[binary]"
python tools/migrar-sqlserver-a-postgres.py
```

- Requiere el esquema ya creado (`dotnet ef database update`) y el ODBC Driver 17 for SQL Server.
- Lee la connection string de PostgreSQL de `appsettings.Development.json`. SQL Server se lee con
  autenticación de Windows (`.\sqlexpress`, base `MeatNet`, configurable con `--sqlserver` y `--base`)
  y **nunca se modifica**.
- Copia las tablas del schema `meat` en orden de FKs, con `ON CONFLICT DO NOTHING` (convive con el
  seed y se puede correr más de una vez), en una sola transacción, e incluye las filas dadas de baja.
- Al final compara la cantidad de filas por tabla y devuelve 1 si alguna no coincide.

Antes de migrar se hizo un backup de SQL Server:
`C:\Program Files\Microsoft SQL Server\MSSQL14.SQLEXPRESS\MSSQL\Backup\MeatNet_pre_postgres.bak`.
