# Almacenes (Corrales y Cámaras)

## 1. Objetivo y alcance

Un **Almacén** es una **ubicación física dentro de un Establecimiento** con capacidad,
código, nombre y estado. Hoy el sistema usa almacenes para dos propósitos distintos del
Ciclo I, que se distinguen por la **familia** de su tipo:

- **CORRAL** — ubicación de recepción/espera de la hacienda **en pie** (entrada del proceso).
- **CÁMARA** — cámara de enfriamiento de faena, destino de la **media res** (salida del proceso).

Ambos son la **misma entidad** `Almacen`; no se duplica el modelo. La distinción va por el
catálogo `TipoAlmacen`, al que se le agrega el eje **`Familia`**.

**Dentro de alcance de este documento:**
- Familias de `TipoAlmacen` (`CORRAL` / `CAMARA`) y código explícito para el corral común.
- Rename `Almacen.CantidadAnimales → Capacidad`.
- Filtrado de almacenes por familia según la pantalla.
- **Destino de faena** (cámara) a nivel de renglón en la Lista de Matanza.

**Fuera de alcance (futuro):**
- Ocupación/capacidad real de la cámara durante la faena (lo escribirá el **Monitor de Faena**).
- Unidad de capacidad diferenciada por familia (cabezas vs. ganchos); por ahora `Capacidad`
  es un entero neutro que cada familia interpreta según su contexto.

## 2. Glosario

| Término | Definición |
|---|---|
| **Almacén** | Ubicación física en un Establecimiento (`Almacen`, PK `Guid Id`). |
| **TipoAlmacen** | Catálogo (PK `string Codigo`) que clasifica el almacén. Ahora con `Familia`. |
| **Familia** | Eje de agrupación del tipo: `CORRAL` (recepción, en pie) o `CAMARA` (faena, media res). |
| **Corral común** | Corral del flujo normal de faena (`CORRAL_COMUN`). Antes era el corral "sin tipo" (null). |
| **Corral de caídos / muertos** | Corrales de segregación sanitaria (`CORRAL_CAIDOS`, `CORRAL_MUERTOS`). |
| **Cámara de faena** | Cámara de enfriamiento destino de la faena (`CAMARA_FAENA`, familia `CAMARA`). |
| **Destino** | Cámara a la que va la media res de un renglón de la Lista de Matanza. |

## 3. Modelo de datos

### 3.1 `TipoAlmacen` (catálogo, PK `string Codigo`)

Se agrega la columna **`Familia`** (string, requerida). Sigue siendo tabla de catálogo
(sin Guid, sin filtro por empresa).

| Codigo | Nombre | Familia | Origen |
|---|---|---|---|
| `CORRAL_COMUN` | Corral Común | `CORRAL` | **nuevo** (seed) |
| `CORRAL_CAIDOS` | Corral de Caídos | `CORRAL` | ya existe (se le setea Familia) |
| `CORRAL_MUERTOS` | Corral de Muertos | `CORRAL` | ya existe (se le setea Familia) |
| `CAMARA_FAENA` | Cámara de Faena | `CAMARA` | **nuevo** (seed) |

**Decisión — por qué `CORRAL_COMUN` y no `CORRAL_ENPIE`:** `EN_PIE/CAIDOS/MUERTOS` ya existen
como catálogo propio `EstadoHacienda` (condición del **animal**). Tipificar el corral por el
estado del animal duplicaría ese eje y ataría el tipo del corral a algo que no le pertenece.
`CORRAL_COMUN` nombra el **rol** del corral (flujo normal de faena); la regla "qué estado de
hacienda va a qué tipo de corral" es negocio en la Recepción, no parte del nombre del tipo.

**Data fix:** los corrales existentes con `TipoAlmacenId = null` se reasignan a `CORRAL_COMUN`
en la migración.

### 3.2 `Almacen` (proceso, PK `Guid Id`)

- **Rename** `CantidadAnimales → Capacidad` (mismo tipo `int`). Es capacidad de la ubicación;
  su unidad depende de la familia (cabezas en corral, ganchos/reses en cámara), pero el campo
  es neutro.
- El resto (`CodigoAlmacen`, `Nombre`, `TipoAlmacenId`, `EstablecimientoId`, `Activo`,
  `ERP_Codigo`, `FechaActualizacion`) no cambia.

### 3.3 `ListaMatanzaDetalle` — destino de faena

Se agrega **`AlmacenDestinoId`** (FK a `Almacen`, familia `CAMARA`):

- **Nullable en BORRADOR:** se puede planificar el renglón sin destino todavía.
- **Requerido para CONFIRMAR:** al confirmar la LM, todos los renglones deben tener destino
  (misma exigencia que el stock). Validado en el handler de Confirmar.
- El `AlmacenId` existente (corral de **origen**) no cambia; se suma el destino.

## 4. Filtrado por familia según pantalla

`GetAlmacenes` acepta un filtro por familia/tipo para no mezclar propósitos:

| Pantalla | Muestra |
|---|---|
| Recepción / Ingreso de Hacienda | Almacenes de familia `CORRAL` (por estado de hacienda: común / caídos / muertos). |
| Existencia de Hacienda | Corrales (familia `CORRAL`). |
| Lista de Matanza — selector de **destino** | Almacenes de familia `CAMARA`. |
| ABM de Almacenes (datos maestros) | Todos (todas las familias). |

## 5. Reglas de negocio

- **R-A1** El corral común es el único destino de la hacienda `EN_PIE` en la Recepción; caídos
  y muertos van a su corral de segregación. (Regla ya vigente; ahora el corral común tiene
  código explícito en vez de null.)
- **R-A2** El destino de un renglón de LM debe ser un almacén de familia `CAMARA` activo del
  mismo Establecimiento.
- **R-A3** Una LM no puede confirmarse si algún renglón no tiene `AlmacenDestinoId`.
- **R-A4** En el alta de renglones de la LM, el destino por defecto es el **último destino
  seleccionado** en esa sesión de edición (conveniencia de carga; el usuario puede cambiarlo).

## 6. Nomenclatura (frontend)

- El ABM de datos maestros se llama **"Almacenes"** (no "Corrales"): sidebar, rutas (`/almacenes`),
  títulos, botones y toasts.
- Las pantallas **operativas** (Ingreso de Hacienda, Existencia de Hacienda) siguen hablando de
  **"corral"**, porque operan específicamente sobre corrales, no sobre el concepto genérico.

## 7. Impacto / checklist de implementación

**Backend**
- [ ] `TipoAlmacen`: propiedad `Familia` + config EF.
- [ ] Migración: columna `Familia`, seed `CORRAL_COMUN`/`CAMARA_FAENA`, `Familia` en caídos/muertos,
      data fix corrales null → `CORRAL_COMUN`.
- [ ] `Almacen`: `CantidadAnimales → Capacidad` (entidad + migración de rename).
- [ ] CQRS Almacenes (Create/Update/Get/GetAlmacenes): renombrar campo, exponer `Familia` del tipo,
      filtro por familia en `GetAlmacenes`.
- [ ] `GetTiposAlmacenes`: exponer `Familia` (para filtrar el selector en el form).
- [ ] `ListaMatanzaDetalle.AlmacenDestinoId` + config EF + migración.
- [ ] Handlers LM: agregar/editar renglón acepta destino; Confirmar valida R-A3.
- [ ] `GetExistenciaHaciendaHandler` / `AprobarIngresoHaciendaHandler`: ajustar lectura de `Capacidad`.

**Frontend**
- [ ] Rename visible Corrales → Almacenes (sidebar, `App.tsx` rutas, list/form pages).
- [ ] `CantidadAnimales → Capacidad` en types/service/form.
- [ ] Selector de tipo en el form mostrando todas las familias.
- [ ] LM: selector de destino (familia `CAMARA`) por renglón, default último seleccionado.
