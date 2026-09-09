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
La aplicación es **multiempresa**: cada registro de `Empresas` es un tenant aislado del resto.
Una Empresa tiene múltiples **Sucursales** y múltiples **Establecimientos** (plantas de faena).
Cada Establecimiento está asociado a una Sucursal y puede operar con distintas **Especies** a través de **EstablecimientosEspecies**, lo que hace al sistema **multiespecie**.

Entidades que sí pertenecen a la estructura organizacional:
`Empresa` → `Sucursal` → `Establecimiento` → `EstablecimientoEspecie`

La clave primaria de `Empresa` es su **código de negocio** (un string, ex `CodigoEmpresa`), no un Guid:
es el mismo valor que viaja en el claim `empresa_id` del JWT, así las consultas filtran sin joinear a `Empresas`.

**El aislamiento lo garantiza el `MeatContext`, no los handlers.** Un query filter global combina
el soft delete con la empresa activa, que sale del JWT vía `ITenantContext`. En las altas, `OnBeforeSaving`
asigna el `EmpresaId` solo, y rechaza modificar filas de otra empresa. Un handler nuevo **no** debe
escribir `WHERE EmpresaId = ...`: ya está aplicado.

Hay dos clases de empresa, distinguidas por `TipoEmpresa`:
- **Operativas** (`FG` frigorífico, `FA` faenadora): tienen sucursales, establecimientos y faenan.
  Al darlas de alta, el `EmpresaSeeder` las deja operables — master data, una sucursal, un
  establecimiento y un usuario `<empresa>.admin`.
- **Administrativa** (`AD`, hoy la empresa `ADM`): no tiene sucursales ni establecimientos ni opera.
  Existe para administrar el padrón de empresas y los catálogos globales. Nace vacía.
  Como no tiene sucursales, el login no le exige una a sus usuarios.

Roles: `SUPERADMIN` administra desde la empresa administrativa; `ADMIN` administra su propia empresa.

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

## Patrones de Tablas (Modelo de Datos)

**La regla, en una línea:**

> Lo que **no** lleva `EmpresaId` es un **catálogo global**: PK `string Codigo` + `Nombre` + …,
> y lo administra la **empresa ADM**.
> Lo que **sí** lleva `EmpresaId` es **propio de una empresa**: PK `Guid Id` + `Nombre` + …,
> y lo administra **cada empresa**.

Es decir: **PK `Guid Id` si y solo si lleva `EmpresaId`.** El `MeatContext` valida esta invariante
al construir el modelo, así que una entidad mal clasificada hace fallar el arranque de la aplicación
en vez de filtrar datos entre empresas en silencio.

Lo que decide a cuál corresponde una entidad es **si sus datos son comunes a todas las empresas o
propios de una**. De ahí salen las tres consecuencias: la forma de la clave, el aislamiento y quién
la administra.

### 1. Catálogos globales — comunes a todas las empresas

- PK `string Codigo` + `Nombre` + `Activo`. Sin Guid, sin Factory.
- **No** llevan `EmpresaId` ni filtro por empresa. Se siembran con sus códigos en la migración.
- Para conjuntos acotados y estables que clasifican datos (`TipoAlmacen`, `TipoEstadoIngreso`,
  `TipoEstadoHacienda`) y para los nomencladores oficiales del rubro (`TipificacionOficial`,
  `MotivoDecomiso`, `Denticion`).
- **Los administra el `SUPERADMIN` desde la empresa `ADM`**, porque una fila la comparten todas las
  empresas: si la editara el ADMIN de una, estaría cambiando datos del resto.
- En el controller eso se traduce en separar lectura de escritura: **escritura**
  `[Authorize(Roles = "SUPERADMIN")]`, **lectura** abierta a cualquier usuario autenticado. La lectura
  tiene que quedar abierta porque estos catálogos son FKs que aparecen en pantallas operativas;
  restringirlas deja combos vacíos. Ver `EspeciesController` y `RolesController`.

### 2. Tablas propias de una empresa — proceso / negocio

- PK `Guid Id` (autogenerada por Factory) + `string EmpresaId` implementando `ITenantScoped`,
  con sus FKs y navegaciones.
- Si la entidad tiene un código de negocio, va como columna `Codigo` con índice único
  `(EmpresaId, Codigo)`, **no** como PK: el código es un atributo editable, no la identidad.
- **Las administra el `ADMIN` de cada empresa**, y el query filter global se encarga de que solo vea
  las suyas.
- Es el patrón completo (Entity, Handlers CQRS, Controller, migraciones, frontend) descrito en
  `docs/BasisCRUD.md`.

Ojo con la intuición: que algo *parezca* un catálogo no alcanza. `UnidadFaena`, `TipoEspecie` y
`DestinoComercial` tienen códigos estándar del rubro, pero cada empresa ajusta sus pesos teóricos,
su código de ERP y cuál es la opción por defecto — así que son del tipo 2.

> `docs/BasisCRUD.md` aplica al **tipo 2 (Guid Id)**, no a los catálogos globales.

## Documentación
- Arquitectura y decisiones técnicas: `docs/`
- Reglas específicas de API: `.claude/rules/api.md`
- Reglas específicas de Web: `.claude/rules/web.md`
- **Guia para CRUDs nuevos: `docs/BasisCRUD.md`** — Seguir SIEMPRE este documento al crear un CRUD de una entidad nueva **de proceso/negocio (PK `Guid Id`)**. No aplica a las tablas de catálogo (PK `string Codigo`, ver "Patrones de Tablas"). Contiene los patrones de backend (Entity, Handlers, Controller, migraciones) y frontend (Types, Service, Pages, rutas). Las entidades globales no llevan EmpresaId ni filtro por empresa.

## Lo que NO hacer
- No modificar migraciones de EF Core ya aplicadas — crear una nueva migración
- No instalar paquetes sin consultar primero
- No cambiar la estructura de carpetas sin confirmación
