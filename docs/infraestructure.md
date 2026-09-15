# Infraestructura de MeatNet

Información básica de infraestructura: entornos, base de datos, configuración, migraciones y deploy.
Consultarlo antes de tocar la conexión, los appsettings, las migraciones o cualquier SQL escrito a mano.

## 1. Entornos

| Entorno | API | Base de datos | Configuración |
|---|---|---|---|
| **Development** | Local (`dotnet run --project Meat`, puerto 5822) | PostgreSQL local, `localhost:5432`, base `meatnet` | `appsettings.Development.json` |
| **Production** | VPS de DonWeb detrás de nginx: `https://vps-6285555-x.dattaweb.com/meatnet/` | Supabase, proyecto propio de MeatNet (São Paulo), base `postgres`, schema `meat` | `appsettings.Production.json` + variables de entorno |

Frontend: en development `npm run dev` en `source/web` (Vite, `localhost:5173`, Node 20); en production
Vercel, `https://meatnet.vercel.app` (ver §6.3).

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
| `JwtOptions__SigninKey` | Clave de firma JWT, generada en el servidor. Sin ella la API no arranca, y tampoco con una de las claves versionadas en el repo |
| `Cors__Origins__0` | Origen del frontend, por ejemplo `https://meatnet.vercel.app`. Se agregan más con `__1`, `__2`. Sin orígenes, ningún navegador puede llamar a la API |
| `ASPNETCORE_ENVIRONMENT` / `ASPNETCORE_URLS` | `Production` / `http://127.0.0.1:5002` (solo local: el acceso público es por nginx) |

Diferencias de comportamiento entre entornos (`Program.cs`):

| | Development | Production |
|---|---|---|
| CORS | Cualquier origen | Solo `Cors:Origins` |
| Swagger (`/swagger`) | Publicado | No se publica |
| `EnableSensitiveDataLogging` | Activo | Apagado |
| Encabezados `X-Forwarded-*` | Se aceptan de localhost (nginx en el mismo servidor) | Ídem |

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
es propia de producción, se guarda en un gestor de contraseñas y tiene **solo letras y números**
(32 caracteres o más): `;` rompe la connection string, y comillas o barras invertidas las interpreta
el `EnvironmentFile` de systemd, con lo que la API manda otra password. No guardar la query como snippet.

> **Supabase bloquea la IP tras 2 passwords incorrectas seguidas** (30 minutos, las conexiones reciben
> `Connection refused`). Se ve y se levanta en *Database → Settings → Network Bans*. Un servicio que se
> reinicia solo con una password mal cargada dispara el bloqueo en segundos: ante un `28P01` detener el
> servicio antes de corregir.

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

### 6.2 API en el VPS *(desplegada el 2026-09-15)*

**Servidor.** VPS de DonWeb, Ubuntu 24.04, 2 núcleos, 1,9 GB de RAM. **Lo comparte con otra API**
(`galecore-datafeed`, .NET en `127.0.0.1:5001`): cualquier cambio en nginx o en el sistema tiene que
cuidarla. SSH en el puerto 5061; administrar con el usuario `elenzi` y `sudo`. El servidor ya está en
hora de Argentina (`timedatectl`), así que no hace falta `TZ` y `DateTime.Now` da la hora correcta.
.NET: ASP.NET Core Runtime 8.0.31 de los paquetes de Ubuntu.

| Pieza | MeatNet |
|---|---|
| Usuario de sistema (sin login) | `meatnet` |
| Aplicación | `/srv/meatnet/app` (archivos de `root`, el servicio solo los lee) |
| Secretos | `/etc/meatnet/api.env` (`root`, permisos 600) |
| Servicio systemd | `meatnet-api` → `/etc/systemd/system/meatnet-api.service` |
| Puerto interno | `127.0.0.1:5002` |
| nginx | `/etc/nginx/snippets/meatnet-api.conf`, incluido en el sitio `galecore-datafeed` antes de su `location /` |
| URL pública | `https://vps-6285555-x.dattaweb.com/meatnet/` (certificado Let's Encrypt del sitio existente) |
| Base de datos | Conexión directa por IPv6 a `db.<project-ref>.supabase.co:5432`, usuario `meatnet` |

**nginx.** El `location /meatnet/` hace `proxy_pass http://127.0.0.1:5002/`: la barra final saca el
prefijo, así que la API recibe `/Clientes` y no necesita saber que vive bajo `/meatnet`. `location =
/meatnet` redirige a `/meatnet/`. nginx elige el prefijo más largo, por eso galecore sigue recibiendo
todo lo demás. Copia del sitio anterior al cambio: `/etc/nginx/sites-available/galecore-datafeed.bak-meatnet`.
Antes de recargar siempre `sudo nginx -t`.

**Deployar una versión nueva.**
1. En la PC: `dotnet publish source\api\Meat\Meat.csproj -c Release -o <carpeta>` y empaquetar sin
   `appsettings.Development.json`: `tar -czf meatnet-api.tar.gz --exclude=./appsettings.Development.json -C <carpeta> .`
2. Copiar desde **PowerShell en la PC** (no desde la sesión SSH):
   `scp -P 5061 meatnet-api.tar.gz elenzi@<ip-del-vps>:/tmp/meatnet-api.tar.gz`
3. En el VPS, descomprimir en una carpeta nueva e intercambiarla con la actual, dejando la anterior
   en `/srv/meatnet/app.old` para volver atrás:

   ```bash
   sudo -v && sudo systemctl stop meatnet-api && sudo rm -rf /srv/meatnet/app.new && sudo install -d -o root -g root -m 755 /srv/meatnet/app.new && sudo tar -xzf /tmp/meatnet-api.tar.gz -C /srv/meatnet/app.new && sudo chown -R root:root /srv/meatnet/app.new && sudo chmod -R u=rwX,go=rX /srv/meatnet/app.new && sudo rm -rf /srv/meatnet/app.old && sudo mv /srv/meatnet/app /srv/meatnet/app.old && sudo mv /srv/meatnet/app.new /srv/meatnet/app && rm /tmp/meatnet-api.tar.gz
   ```

   Volver atrás: `sudo systemctl stop meatnet-api`, `mv` de `app.old` a `app` y arrancar.
4. Arranque controlado: si el servicio falla, se detiene antes del segundo intento (así una
   password mal cargada no dispara el bloqueo de IP de Supabase). Las migraciones pendientes las
   aplica `Migrate()` al arrancar.

   ```bash
   sudo systemctl start meatnet-api; sleep 6; if systemctl is-active --quiet meatnet-api; then echo ACTIVO; else sudo systemctl stop meatnet-api; echo "FALLO: detenido"; fi; sudo journalctl -u meatnet-api -n 20 --no-pager -o cat
   ```
5. Verificar sin token: `/Usuarios` y cualquier endpoint que no sea el login responden `401`
   (`curl -s -o /dev/null -w "%{http_code}" http://127.0.0.1:5002/Usuarios`), y el login con un
   usuario inexistente responde `400`.

En la sesión SSH, **pegar de a un comando**: si `sudo` pide la password, las líneas pegadas a
continuación se consumen como password. Validar antes con `sudo -v`.

- **Password del `superadmin`:** el seed (`02_SeedCatalogos`) copia el hash de la base de desarrollo,
  así que en una base nueva entra con la password de desarrollo. En producción se cambió el 2026-09-15
  desde "Cambiar contraseña". En cualquier base creada desde cero, cambiarla **antes** de abrir la
  aplicación a otros usuarios.
- **Reinicios del servidor:** afectan también a la otra API. Antes de reiniciar, verificar que
  `meatnet-api`, `galecore-datafeed` y `nginx` estén `enabled` (SSH arranca por `ssh.socket`) y que
  `sudo nginx -t` pase; si SSH no vuelve, entrar por la consola del panel de DonWeb. El 2026-09-15 se
  reinició por el kernel 6.8.0-139 y todo volvió solo.
- **Pendiente del servidor:** deshabilitar el login SSH directo de `root`.

### 6.3 Frontend en Vercel *(desplegado el 2026-09-15)*

| Aspecto | Valor |
|---|---|
| URL | `https://meatnet.vercel.app` |
| Proyecto | `meatnet`, importado de `ericlenzi/MeatNet` |
| Rama de producción | `master` (la rama por defecto). Se publica mergeando `development` en `master` |
| Root Directory | `source/web` (preset Vite: `npm run build`, salida `dist`) |
| Variable | `VITE_API_BASE_URL` = `https://vps-6285555-x.dattaweb.com/meatnet`, sin barra final, entorno Production |
| Plan | Hobby: uso personal y no comercial. Para uso comercial corresponde Pro |

- **Visibilidad de la variable: *Config*, no *Secret*/*Sensitive*.** Las variables `VITE_` se copian al
  JavaScript que descarga el navegador, así que Vercel no deja marcarlas como secretas y rechaza guardarla.
  No es un secreto: es la dirección pública de la API.
- **Vite fija la variable al compilar.** Cambiarla en Vercel no alcanza: hay que hacer **Redeploy** sin
  la caché del build. Si la variable falta, el build **no falla**: queda sin URL de la API y las llamadas
  van contra el propio dominio de Vercel, así que el login falla. Verificar en el JavaScript publicado
  que aparezca la URL de la API.
- **`source/web/.env.development`** tiene `VITE_API_BASE_URL=http://localhost:5822` para `npm run dev`.
  Vite solo lo carga en modo desarrollo, así que nunca entra en un build de producción. Antes se llamaba
  `.env` y se cargaba en todos los modos: con la variable mal cargada en Vercel, el primer deploy salió
  apuntando a `localhost:5822` sin ningún error.
- **`source/web/vercel.json`** reescribe todas las rutas a `index.html`: el frontend usa `BrowserRouter`
  y sin la regla recargar una ruta interna da 404.
- **CORS:** la API acepta solo `https://meatnet.vercel.app` (`Cors__Origins__0` en `/etc/meatnet/api.env`,
  y `sudo systemctl restart meatnet-api`). Las URLs de *preview* de Vercel no están habilitadas. Otro
  dominio se agrega como `Cors__Origins__1`.

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
