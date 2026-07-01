# Manual de Proceso — Ingreso de Hacienda

> Proceso del Ciclo I (Recepción de hacienda). Cubre desde el arribo del camión jaula
> hasta la ubicación de la hacienda en los corrales y su disponibilidad como stock para
> el plan de faena.
>
> Este documento es la **especificación funcional y de modelo de datos**. Para los
> patrones técnicos de implementación (Entity, Handlers, Controller, migraciones, Types,
> Service, Pages) seguir **`docs/BasisCRUD.md`**.

---

## Índice

1. [Objetivo y alcance](#1-objetivo-y-alcance)
2. [Glosario](#2-glosario)
3. [Flujo del proceso](#3-flujo-del-proceso)
4. [Máquinas de estado](#4-máquinas-de-estado)
5. [Modelo de datos](#5-modelo-de-datos)
6. [Catálogos de estado](#6-catálogos-de-estado)
7. [Reglas de negocio](#7-reglas-de-negocio)
8. [Cálculos](#8-cálculos)
9. [Existencia de hacienda](#9-existencia-de-hacienda)
10. [Filtro por empresa y establecimiento](#10-filtro-por-empresa-y-establecimiento)
11. [Superficie de API](#11-superficie-de-api)
12. [Frontend (pantallas)](#12-frontend-pantallas)
13. [Prerrequisitos del modelo (gaps a resolver)](#13-prerrequisitos-del-modelo-gaps-a-resolver)
14. [Checklist de implementación](#14-checklist-de-implementación)

---

## 1. Objetivo y alcance

Registrar **todos los ingresos de hacienda** a un Establecimiento y conocer, al final del
proceso, **qué tropas hay en stock por corral**, discriminadas por tipo de especie y
cliente, respetando la **capacidad máxima de animales** de cada corral.

La información preliminar del DT-e permite una previsión de ingresos, pero el proceso
**comienza cuando ingresa físicamente el camión**. El pesaje se hace sobre la jaula entera
y el peso se distribuye entre las categorías para determinar las cantidades de animales.

**Dentro de alcance:** documentación del ingreso (DT-e), pesaje del camión, distribución
por categoría, ubicación en corrales, generación de tropas y stock por corral.

**Fuera de alcance (Ciclo I aguas abajo):** plan de faena, línea de faena, tipificación y
romaneo. Estos consumen las **tropas disponibles en corrales** que genera este proceso.

---

## 2. Glosario

| Término | Definición |
|---|---|
| **DT-e** | Documento de Tránsito electrónico (SENASA). Ampara el movimiento de hacienda. En este proceso se cargan sus datos (N° y fecha de emisión), **no** es una entidad propia. |
| **RENSPA** | Registro Nacional Sanitario de Productores Agropecuarios. Identifica el establecimiento de origen. Vive en `ClienteEstablecimiento.CodigoRenspa`. |
| **CUIG** | Clave Única de Identificación Ganadera. Vive en `ClienteEstablecimiento.NumeroCUIG`. |
| **Tropa** | Lote de animales de un mismo cliente y especie que ingresa. Unidad de **trazabilidad** aguas abajo (faena/romaneo). Lleva número correlativo por Cliente-Establecimiento + Especie. |
| **Procedencia** | Establecimiento de origen de la hacienda, modelado como `ClienteEstablecimiento` (aporta RENSPA y CUIG). |
| **Peso teórico** | Peso de referencia de una categoría (`TipoEspecie.PesoTeorico`). Se usa para estimar la cantidad de animales a partir del peso pesado. |
| **Corral** | `Almacen` de tipo Corral. Tiene capacidad máxima de animales (`CantidadAnimales`). |

---

## 3. Flujo del proceso

### Paso 1 — Arribo del camión → se crea el Ingreso `[Borrador]`
El **operador de recepción** abre un nuevo Ingreso y carga la documentación:

- **DT-e**: `NumeroDte`, `FechaEmisionDte`
- **Cliente** (dueño / consignatario)
- **Procedencia**: `ClienteEstablecimiento` → aporta RENSPA y CUIG
- **Origen geográfico**: **Provincia** (tabla `Provincias`) + **Localidad** (texto)
- **Origen de hacienda** y **Uso de hacienda** (catálogos)
- **Establecimiento destino** (el establecimiento activo) + fecha/hora de ingreso
- **Especie** (`EspecieId`): por defecto la especie activa del establecimiento (como en el Header;
  si el establecimiento tiene una sola, se muestra esa). Limita los tipos de especie a pesar.
- **Transporte** (texto libre): `Transportista`, `Chofer`, `PatenteCamion`, `PatenteJaula`

> **No se genera número de tropa todavía.** El ingreso queda en **Borrador**.
> Regla: **1 Ingreso = 1 DT-e = 1 Cliente = 1 Camión = 1 Especie**.

### Paso 2 — Registro de pesadas (grilla por tipo de especie)
El peso se registra en una **grilla**, una línea por **TipoEspecie** (categoría) — los tipos
disponibles están **filtrados por la Especie** del ingreso. Cada línea:
`TipoEspecie`, `PesoIngreso` (kg), UM = KG, **CantidadEstimada** (según peso teórico) e
**IdPesada** (número de ticket de la balanza del frigorífico, string). El sistema estima la
cantidad de animales:

```
CantidadEstimada = PesoIngreso / TipoEspecie.PesoTeorico   (ajustable por el operador)
PesoNeto          = Σ PesoIngreso   (kg, suma de las pesadas)
```

> No se carga bruto ni tara del camión; el **PesoNeto** del ingreso es la suma de las pesadas.

### Paso 3 — Ubicación en corrales (*ubicación en corrales*)
Cada categoría se distribuye en **uno o más corrales**. Solo pueden ubicarse los **tipos de
especie cargados en el registro de pesadas**. Por línea de ubicación:

- `TipoEspecie`, `Almacen` (corral), **Cantidad** (UN, ajustable)
- **PesoPromedio** = PesoIngreso ÷ Cantidad del tipo especie (calculado)
- **EstadoHacienda**: `En Pie` / `Caídos` / `Muertos`

> Si una tropa **no entra** en un corral, el resto se ubica en **otro corral**.
> La hacienda **En Pie** va a corrales estándar; **Caídos/Muertos** van a corrales
> especiales y **no** cuentan como stock de faena (ver [§7](#7-reglas-de-negocio)).

### Paso 4 — Envío a aprobación → `[Pendiente Aprobación]`
El operador finaliza la carga y envía el ingreso a aprobación. Pasa a **Pendiente
Aprobación** y queda a la espera del supervisor.

> **Guardar borrador** permite guardar el ingreso **sin cargar la ubicación en corrales**.
> Las ubicaciones solo son obligatorias al **enviar a aprobación**.

### Paso 5 — Aprobación del supervisor (pantalla separada) → `[Aprobado]` + nace la Tropa
Desde una **pantalla de aprobación separada** del formulario de carga, el **supervisor**
confirma el ingreso. El sistema, en una transacción:

1. Crea una **`Tropa` por cada Especie** del ingreso.
2. Genera el **número correlativo** vía `NumeradorTropa` (Cliente-Establecimiento + Especie).
3. Marca cada tropa como **Recepcionada** y liga las ubicaciones a su tropa.

> Recién en **Aprobado** la hacienda **En Pie cuenta como stock** del corral y queda
> disponible para el plan de faena.

### Paso 7 — Anulación posterior (si hay que revertir)
La(s) tropa(s) pasan a **Anulada**: el número **queda registrado y no se reutiliza**, y el
stock se descuenta del corral. El ingreso pasa a **Anulado**.

---

## 4. Máquinas de estado

### Ingreso de Hacienda

```
            enviar a aprobación        aprobar (supervisor)
 [Borrador] ───────────────▶ [Pendiente Aprobación] ───────────────▶ [Aprobado]
     │                                │                                   │
     │ descartar                      │ rechazar/volver                   │ anular
     ▼                                ▼                                   ▼
 [Anulado]  ◀──────────────────  [Borrador]                          [Anulado]
```

| Estado | Quién | Editable | Efecto en stock |
|---|---|---|---|
| **Borrador** | Operador | Todo (documentación, pesaje, pesadas, ubicación) | No |
| **Pendiente Aprobación** | Operador → Supervisor | Solo lectura para el operador; el supervisor aprueba o rechaza | No |
| **Aprobado** | Supervisor | No editable (solo anulable) | **Sí** (En Pie) |
| **Anulado** | Supervisor | No editable | No (se descuenta) |

### Tropa

```
 (no existe)  ──aprobar ingreso──▶ [Recepcionada]  ──anular──▶ [Anulada]
```

- La tropa **se crea al aprobar** (no antes).
- El **número correlativo nunca se reutiliza**, ni siquiera tras anulación.

---

## 5. Modelo de datos

Convenciones del proyecto (ver `docs/BasisCRUD.md`): PK `Guid` autogenerada por factory,
soft-delete vía shadow property `FechaBaja`, query filter global `WHERE FechaBaja IS NULL`,
índices únicos **filtrados** con `HasFilter("[FechaBaja] IS NULL")`.

### 5.1 `IngresoHacienda` (cabecera)

```csharp
namespace Meat.Domain.IngresosHaciendas
{
    public class IngresoHacienda
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public long NumeroIngreso { get; set; }            // correlativo por establecimiento

        // Destino (aporta el filtro por empresa vía Establecimiento.Empresa)
        public Guid EstablecimientoId { get; set; }
        public virtual Establecimiento Establecimiento { get; set; }

        public DateTime FechaHoraIngreso { get; set; }

        // Especie del ingreso (habilitada para el establecimiento)
        public string EspecieId { get; set; }
        public virtual Especie Especie { get; set; }

        // DT-e (datos, no entidad)
        public string NumeroDte { get; set; }
        public DateTime FechaEmisionDte { get; set; }

        // Dueño / consignatario
        public Guid ClienteId { get; set; }
        public virtual Cliente Cliente { get; set; }

        // Procedencia (RENSPA / CUIG)
        public Guid ClienteEstablecimientoId { get; set; }
        public virtual ClienteEstablecimiento ClienteEstablecimiento { get; set; }

        // Origen geográfico de la hacienda
        public int ProvinciaId { get; set; }
        public virtual Provincia Provincia { get; set; }
        public string Localidad { get; set; }               // texto por ahora

        // Clasificación
        public string OrigenHaciendaId { get; set; }
        public virtual OrigenHacienda OrigenHacienda { get; set; }
        public string UsoHaciendaId { get; set; }
        public virtual UsoHacienda UsoHacienda { get; set; }

        // Transporte (texto libre — v1)
        public string Transportista { get; set; }
        public string Chofer { get; set; }
        public string PatenteCamion { get; set; }
        public string PatenteJaula { get; set; }

        // Peso neto total del ingreso (kg) = suma de las pesadas por tipo especie
        public double PesoNeto { get; set; }

        // Estado
        public string EstadoIngresoId { get; set; }         // FK TiposEstadosIngresos
        public virtual TipoEstadoIngreso EstadoIngreso { get; set; }
        public DateTime? FechaAprobacion { get; set; }
        public Guid? UsuarioAprobacionId { get; set; }

        public DateTime FechaActualizacion { get; set; }

        public virtual ICollection<IngresoHaciendaPesada> Pesadas { get; set; }
        public virtual ICollection<IngresoHaciendaUbicacion> Ubicaciones { get; set; }
        public virtual ICollection<Tropa> Tropas { get; set; }
    }
}
```

### 5.2 `Tropa` (entidad de primer nivel — trazabilidad)

```csharp
namespace Meat.Domain.Tropas
{
    public class Tropa
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public Guid IngresoHaciendaId { get; set; }
        public virtual IngresoHacienda IngresoHacienda { get; set; }

        // Contexto de numeración (NumeradorTropa: ClienteEstablecimiento + Especie)
        public Guid ClienteEstablecimientoId { get; set; }
        public string EspecieCodigo { get; set; }
        public virtual Especie Especie { get; set; }

        public long NumeroTropa { get; set; }               // correlativo, no reutilizable
        public string EstadoTropaId { get; set; }            // FK TiposEstadosTropas
        public virtual TipoEstadoTropa EstadoTropa { get; set; }
        public DateTime FechaRecepcion { get; set; }
    }
}
```

### 5.3 `IngresoHaciendaPesada` (registro de pesadas)

```csharp
namespace Meat.Domain.IngresosHaciendas
{
    public class IngresoHaciendaPesada
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public Guid IngresoHaciendaId { get; set; }
        public virtual IngresoHacienda IngresoHacienda { get; set; }

        public string TipoEspecieId { get; set; }
        public virtual TipoEspecie TipoEspecie { get; set; }

        public double PesoIngreso { get; set; }              // kg
        public string UnidadMedida { get; set; }             // "KG"
        public string IdPesada { get; set; }                 // n° de ticket de la balanza (string)
    }
}
```

### 5.4 `IngresoHaciendaUbicacion` (ubicación en corrales)

```csharp
namespace Meat.Domain.IngresosHaciendas
{
    public class IngresoHaciendaUbicacion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public Guid IngresoHaciendaId { get; set; }
        public virtual IngresoHacienda IngresoHacienda { get; set; }

        public Guid? TropaId { get; set; }                   // null en Borrador; se liga al aprobar
        public virtual Tropa Tropa { get; set; }

        public string TipoEspecieId { get; set; }
        public virtual TipoEspecie TipoEspecie { get; set; }

        public Guid AlmacenId { get; set; }                  // corral
        public virtual Almacen Almacen { get; set; }

        public int Cantidad { get; set; }                    // UN (animales)
        public double PesoPromedio { get; set; }             // calculado (kg)
        public string EstadoHaciendaId { get; set; }         // FK TiposEstadosHacienda
        public virtual TipoEstadoHacienda EstadoHacienda { get; set; }
    }
}
```

### 5.5 Índices únicos filtrados (`MeatContext.OnModelCreating`)

```csharp
// Número de ingreso correlativo por establecimiento
modelBuilder.Entity<IngresoHacienda>()
    .HasIndex(x => new { x.EstablecimientoId, x.NumeroIngreso })
    .IsUnique()
    .HasFilter("[FechaBaja] IS NULL");

// Número de tropa único por Cliente-Establecimiento + Especie
modelBuilder.Entity<Tropa>()
    .HasIndex(x => new { x.ClienteEstablecimientoId, x.EspecieCodigo, x.NumeroTropa })
    .IsUnique()
    .HasFilter("[FechaBaja] IS NULL");
```

---

## 6. Catálogos de estado

Los estados **no se modelan como enums**, sino como **tablas de catálogo**, siguiendo el
patrón de `TipoAlmacen` (PK `string Codigo`, `Nombre`, `Activo`).

> **Decisión:** se descarta el enfoque de enums para estados.
> - Se **elimina `TipoAlmacenEnum`** de `Meat.Domain.Enums` (la tabla `TiposAlmacenes` ya cubre ese rol).
> - Se **eliminan las tablas `AlmacenesMateriales` y `TiposAnimales`** (sin uso en la aplicación; `AlmacenesMateriales` era la única dependencia de `TiposAnimales`).

Tres catálogos nuevos:

| Tabla (DbSet) | Entidad | Códigos sugeridos |
|---|---|---|
| `TiposEstadosIngresos` | `TipoEstadoIngreso` | `BORRADOR`, `PENDIENTE`, `APROBADO`, `ANULADO` |
| `TiposEstadosTropas` | `TipoEstadoTropa` | `RECEPCIONADA`, `ANULADA` |
| `TiposEstadosHacienda` | `TipoEstadoHacienda` | `EN_PIE`, `CAIDOS`, `MUERTOS` |

Patrón de cada entidad (idéntico a `TipoAlmacen`):

```csharp
namespace Meat.Domain.TiposEstadosIngresos
{
    public class TipoEstadoIngreso
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public bool Activo { get; set; }
    }
}
```

(Análogo para `TipoEstadoTropa` en `Meat.Domain.TiposEstadosTropas` y `TipoEstadoHacienda`
en `Meat.Domain.TiposEstadosHacienda`.)

Registrar cada DbSet en `MeatContext` y **sembrar los códigos iniciales** en la migración.

> El `EstadoHacienda` de cada ubicación determina a **qué tipo de corral** puede ir
> (ver [§7](#7-reglas-de-negocio) y [§13](#13-prerrequisitos-del-modelo-gaps-a-resolver)).

---

## 7. Reglas de negocio

| # | Regla | Dónde se aplica |
|---|---|---|
| R1 | **1 Ingreso = 1 DT-e = 1 Cliente = 1 Camión**. | Create/Update Ingreso |
| R2 | El número de tropa **no se genera en Borrador**; se genera al **Aprobar**. | Handler de aprobación |
| R3 | **Una Tropa por (Ingreso, Especie)**. Si el camión trae varias especies → varias tropas. | Handler de aprobación |
| R4 | Numeración de tropa vía `NumeradorTropa` (Cliente-Establecimiento + Especie): tomar `UltimoNumeroTropa`, incrementar y persistir. Si no existe el numerador, crearlo. **El número nunca se reutiliza** (ni tras anulación). | Handler de aprobación (transaccional) |
| R5 | `PesoNeto = Σ PesoIngreso` (suma de las pesadas del registro). No se carga bruto ni tara del camión. | Cálculo en cabecera |
| R6 | **Guardar borrador** no exige ubicaciones en corral; solo se ubican en corral los **tipos de especie del registro de pesadas**, y las ubicaciones son obligatorias al **enviar a aprobación**. | Validación de carga |
| R7 | Cantidad estimada por categoría = `PesoIngreso / TipoEspecie.PesoTeorico`, **ajustable** por el operador. | Cálculo en registro de pesadas |
| R8 | **Capacidad de corral**: `ocupación actual + cantidad a ubicar ≤ Almacen.CantidadAnimales`. **Tope duro** (bloquea). | Validación al aprobar |
| R9 | Una tropa que **no entra** en un corral se **reparte** en otro(s) corral(es) (múltiples líneas de ubicación). | Ubicación |
| R10 | Solo la hacienda **En Pie** de tropas **Recepcionadas** cuenta como **stock de faena**. | Consulta de stock |
| R11 | **Caídos/Muertos** se ubican en **corrales especiales** y **no** ocupan cupo de faena ni cuentan como stock disponible. | Validación de ubicación + stock |
| R12 | Al **anular**, las tropas pasan a `Anulada`, el ingreso a `Anulado` y el stock se descuenta. El número de tropa permanece. | Handler de anulación |
| R13 | La **aprobación** la realiza un **supervisor** desde una **pantalla separada** (rol distinto al de carga). | Autorización + UI |
| R14 | Todo se filtra por **empresa activa** (vía Establecimiento) y opera sobre el **establecimiento activo**. | Todos los handlers |

---

## 8. Cálculos

```
PesoNeto (ingreso)       = Σ PesoIngreso                                    // suma de las pesadas

CantidadEstimada (cat.)  = round( PesoIngreso / TipoEspecie.PesoTeorico )   // ajustable

PesoPromedio (ubicación) = PesoIngreso(cat.) / Σ Cantidad(cat.)             // por tipo especie
```

---

## 9. Existencia de hacienda

Consulta derivada (no es una tabla materializada). La existencia disponible para faena se
muestra **con dos unidades de medida: UN (unidades) y KG (kilogramos)**:

```sql
-- Pseudocódigo del criterio
SELECT u.AlmacenId, u.TipoEspecieId, t.ClienteEstablecimientoId, t.NumeroTropa,
       SUM(u.Cantidad)                  AS CantidadUN,   -- unidades (UN)
       SUM(u.Cantidad * u.PesoPromedio) AS PesoKG        -- kilogramos (KG)
FROM   IngresoHaciendaUbicacion u
JOIN   Tropa t            ON t.Id = u.TropaId
JOIN   IngresoHacienda i  ON i.Id = u.IngresoHaciendaId
WHERE  i.EstadoIngresoId = 'APROBADO'
  AND  t.EstadoTropaId   = 'RECEPCIONADA'
  AND  u.EstadoHaciendaId = 'EN_PIE'
  AND  i.Establecimiento.Empresa.CodigoEmpresa = @codigoEmpresa
  AND  i.EstablecimientoId = @establecimientoActivo
  -- AND la tropa aún no fue consumida por faena (cuando exista Ciclo I aguas abajo)
GROUP BY u.AlmacenId, u.TipoEspecieId, t.ClienteEstablecimientoId, t.NumeroTropa
```

**Ocupación de un corral** (para validar capacidad R8) = `SUM(Cantidad)` de las ubicaciones
En Pie de tropas Recepcionadas en ese `AlmacenId`, comparado contra `Almacen.CantidadAnimales`.

---

## 10. Filtro por empresa y establecimiento

`IngresoHacienda` no lleva `EmpresaId` directo: el filtro se hace **vía Establecimiento**.

```csharp
.Where(x => x.Establecimiento.Empresa.CodigoEmpresa == request.CodigoEmpresa)
```

Además todas las operaciones se acotan al **establecimiento activo** de la sesión. Los
corrales (`Almacen`) seleccionables son los del establecimiento activo (ver [§13](#13-prerrequisitos-del-modelo-gaps-a-resolver)).

---

## 11. Superficie de API

`IngresosHaciendaController` (CRUD de cabecera + detalle anidado: pesadas y ubicaciones):

| Verbo | Ruta | Rol | Descripción |
|---|---|---|---|
| GET | `/IngresosHacienda` | Operador/Supervisor | Listado paginado (filtro por estado, cliente, fecha) |
| GET | `/IngresosHacienda/{id}` | Operador/Supervisor | Detalle completo |
| POST | `/IngresosHacienda` | Operador | Crea en **Borrador** |
| PUT | `/IngresosHacienda/{id}` | Operador | Actualiza Borrador (documentación, pesaje, pesadas, ubicación) |
| DELETE | `/IngresosHacienda/{id}` | Operador | Soft-delete (solo Borrador) |
| POST | `/IngresosHacienda/{id}/enviar-aprobacion` | Operador | Borrador → Pendiente Aprobación |
| POST | `/IngresosHacienda/{id}/aprobar` | **Supervisor** | Pendiente → Aprobado (crea Tropas + numeración) |
| POST | `/IngresosHacienda/{id}/rechazar` | **Supervisor** | Pendiente → Borrador |
| POST | `/IngresosHacienda/{id}/anular` | **Supervisor** | Aprobado → Anulado (anula tropas, descuenta stock) |

Consultas asociadas:

| Verbo | Ruta | Descripción |
|---|---|---|
| GET | `/ExistenciaHacienda` | Existencia por corral / tipo especie / cliente / tropa, en **UN y KG** (En Pie, Aprobado) |
| GET | `/Tropas` | Tropas Recepcionadas disponibles en corrales (consumo del plan de faena) |

> Inyectar siempre `base.CurrentUser.CodigoEmpresa` y el establecimiento activo en los
> requests (nunca confiar en el body). Ver `docs/BasisCRUD.md` §4.

---

## 12. Frontend (pantallas)

**Ingreso** y **Aprobación** son módulos **separados**, con **dos opciones de menú distintas**:
**Ingreso de Hacienda** y **Aprobación de Hacienda**.

| Página | Ruta | Menú | Descripción |
|---|---|---|---|
| `IngresosHaciendaListPage` | `/ingresos-hacienda` | Ingreso de Hacienda | Bandeja de ingresos. Los **Aprobados** se ven en **solo lectura** (no se pueden modificar). |
| `IngresoHaciendaFormPage` | `/ingresos-hacienda/create`, `/:id/edit` | Ingreso de Hacienda | Alta/edición **por bloques** (ver abajo). Botón "Enviar a aprobación". |
| `AprobacionHaciendaListPage` | `/aprobacion-hacienda` | Aprobación de Hacienda | **Solo** ingresos en **Pendiente Aprobación** (los **Borradores no se muestran**). |
| `AprobacionHaciendaDetailPage` | `/aprobacion-hacienda/:id` | Aprobación de Hacienda | Revisión (solo lectura) + acciones **Aprobar** / **Rechazar**. |
| `ExistenciaHaciendaPage` | `/existencia-hacienda` | Existencia de Hacienda | Existencia por corral en **UN y KG** (ocupación vs capacidad, por tipo especie / cliente / tropa). |

### Separación Ingreso / Aprobación (reglas de visibilidad y edición)
- **Ingreso de Hacienda** (operador): crea y edita ingresos en **Borrador**. Los ingresos
  **Aprobados** se listan **solo en lectura** — **no se pueden modificar** desde este módulo.
- **Aprobación de Hacienda** (supervisor): muestra **únicamente** ingresos en
  **Pendiente Aprobación**; los **Borradores NO aparecen**. Desde aquí se **Aprueba** (crea
  tropas + numeración) o se **Rechaza** (vuelve a Borrador).

### Carga por bloques (cuadros) — `IngresoHaciendaFormPage`
El formulario se carga **por bloques/cuadros**, no todo junto. Son **tres cuadros**:

1. **Detalle de hacienda** — agrupa tres sub-bloques:
   - *Datos de ingreso*: **Fecha y hora de ingreso** → **primer dato a cargar**,
     **pre-cargado con la fecha y hora actuales**; **Establecimiento** → el
     **establecimiento activo**, **pre-cargado y deshabilitado** (no se puede cambiar); y
     **Especie** → por defecto la especie activa del establecimiento (como en el Header).
   - *Datos de origen*: N° DT-e y fecha emisión, Cliente, Procedencia
     (`ClienteEstablecimiento` → RENSPA/CUIG), **Provincia** (tabla `Provincias`) y
     **Localidad** (texto), Origen y Uso de hacienda.
   - *Datos de transporte*: transportista, chofer, patente camión, patente jaula.
   > El cuadro se titula **Detalle Ingreso** y separa visualmente las tres secciones.
2. **Registro de pesadas** — grilla con una línea por tipo especie: peso ingreso, UM (KG),
   cantidad estimada por peso teórico e **ID Pesada** (ticket de la balanza). Sin bruto ni tara.
3. **Ubicación en Corrales** — distribución por corral (cantidad, peso promedio, estado hacienda);
   solo se pueden ubicar los tipos de especie del registro de pesadas.

### Comportamiento del formulario (ver `docs/BasisCRUD.md` §3.4)
- **Fecha/hora** pre-cargadas con el momento actual.
- **Establecimiento** fijo (activo, `disabled`).
- **Especie** por defecto = especie activa del establecimiento (si hay una sola, esa); al cambiarla
  se reinicia el detalle. Los **tipos de especie** a pesar se filtran por la Especie del ingreso.
- Los **corrales** del combo se filtran por establecimiento activo y tipo apto al `EstadoHacienda`.
- **Peso promedio** y **cantidad estimada** se calculan en vivo; la cantidad es editable.
- **Guardar borrador** permite guardar sin ubicaciones en corral; **enviar a aprobación** las exige.
- Los tipos de especie de la **ubicación en corrales** se limitan a los del registro de pesadas.
- Validación de capacidad de corral antes de permitir enviar a aprobación.

Sidebar: **dos ítems separados** — **Ingreso de Hacienda** (ícono `truck`) y
**Aprobación de Hacienda** (ícono `clipboardCheck`) — más **Existencia de Hacienda**
(ícono `database` / `chartBar`).

---

## 13. Prerrequisitos del modelo (gaps a resolver)

Antes (o como parte) de implementar este proceso hay que ajustar el modelo existente:

1. **`Almacen` necesita `EstablecimientoId`.** Hoy `Almacen` es global (no tiene
   `EstablecimientoId` ni `EmpresaId`). Los corrales son físicos de un establecimiento y el
   stock se calcula por establecimiento. **Acción:** agregar `EstablecimientoId` (FK +
   navegación) a `Almacen` con su migración, y filtrar los corrales por el establecimiento
   activo. Migrar los registros existentes al establecimiento que corresponda.

2. **Clasificación de corrales para Caídos/Muertos.** El stock de faena es solo **En Pie**;
   los Caídos/Muertos van a **corrales especiales**. Hoy el catálogo `TiposAlmacenes` solo
   tiene `Generico / Corral / Camara`. **Acción:** definir cómo se marca un corral apto para
   En Pie vs Caídos/Muertos — opciones: (a) agregar códigos al catálogo `TiposAlmacenes`
   (p. ej. `CorralCaidos`, `CorralMuertos`), o (b) agregar a `Almacen` un campo de
   clasificación de `EstadoHacienda` admitido. Decidir antes de implementar la validación R11.

3. **Numerador de Ingreso.** Definir el origen del correlativo `NumeroIngreso` por
   establecimiento (numerador propio análogo a `NumeradorTropa`, o `MAX+1` transaccional).

4. **Migrar de enums a catálogos de estado** (ver [§6](#6-catálogos-de-estado)). **Acción:**
   eliminar `TipoAlmacenEnum` de `Meat.Domain.Enums`; crear las tablas `TiposEstadosIngresos`,
   `TiposEstadosTropas` y `TiposEstadosHacienda` con sus códigos sembrados.
   (Se eliminan también `AlmacenesMateriales` y `TiposAnimales`, sin uso.)

---

## 14. Checklist de implementación

> Seguir los patrones de `docs/BasisCRUD.md`. Recordar: migraciones **siempre** con
> `dotnet ef migrations add` (nunca a mano), e índices únicos **filtrados**.

### Prerrequisitos
- [ ] Agregar `EstablecimientoId` a `Almacen` (+ migración + backfill)
- [ ] Resolver clasificación de corrales para Caídos/Muertos
- [ ] Definir numerador de `NumeroIngreso`
- [ ] Eliminar `TipoAlmacenEnum` de `Meat.Domain.Enums`
- [ ] Eliminar `AlmacenesMateriales` y `TiposAnimales` (sin uso)

### Backend — Dominio
- [ ] Catálogos de estado (tablas): `TipoEstadoIngreso`, `TipoEstadoTropa`, `TipoEstadoHacienda` (+ DbSets + seed)
- [ ] Entidades: `IngresoHacienda`, `Tropa`, `IngresoHaciendaPesada`, `IngresoHaciendaUbicacion` (+ factories)
- [ ] DbSets en `MeatContext` + índices únicos filtrados
- [ ] Migración EF Core

### Backend — Application (CQRS / MediatR)
- [ ] CRUD de `IngresoHacienda` (Get/GetAll/Create/Update/Delete) con detalle anidado
- [ ] Acción **Enviar a aprobación** (Borrador → Pendiente)
- [ ] Acción **Aprobar** (crea Tropas, numeración transaccional, valida capacidad R8, liga ubicaciones)
- [ ] Acción **Rechazar** (Pendiente → Borrador)
- [ ] Acción **Anular** (anula tropas, descuenta stock)
- [ ] Query **ExistenciaHacienda** (devuelve UN y KG)
- [ ] Query **Tropas disponibles** (para faena)
- [ ] Filtro por empresa + establecimiento activo en todos los handlers

### Backend — Controllers
- [ ] `IngresosHaciendaController` (CRUD + acciones, con roles Operador/Supervisor)
- [ ] `ExistenciaHaciendaController` / endpoint de existencia (UN y KG)
- [ ] `TropasController` (consulta)

### Frontend
- [ ] Types + services (`ingresosHacienda`, `tropas`, `existenciaHacienda`)
- [ ] `IngresosHaciendaListPage` (Aprobados en solo lectura)
- [ ] `IngresoHaciendaFormPage` (3 cuadros: Detalle de hacienda, Registro de pesadas, Ubicación en Corrales)
- [ ] `AprobacionHaciendaListPage` + `AprobacionHaciendaDetailPage` (solo Pendientes; sin Borradores)
- [ ] `ExistenciaHaciendaPage` (muestra UN y KG)
- [ ] Rutas en `App.tsx` + ítems en `Sidebar.tsx`
- [ ] Verificar compilación (`dotnet build`, `npx tsc --noEmit`)
```
