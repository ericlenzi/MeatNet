# Trazabilidad de la Tropa (Ciclo I)

## 1. Objetivo

La **Tropa** es la unidad de trazabilidad del Ciclo I. Este documento describe cómo se
registra y se consulta el **ciclo de vida completo** de una tropa: desde su recepción
hasta su faena/anulación, atravesando la ubicación en corrales y la planificación de faena.

El número de tropa (`NumeroTropa`) es **correlativo y no reutilizable** por
`ClienteEstablecimiento + Especie`. **No es único global**: un mismo número puede
existir para distintos cliente/especie, por lo que la consulta por número puede devolver
más de una tropa.

## 2. Modelo de trazabilidad (opción "log de eventos")

La línea de tiempo de una tropa se arma **unificando dos fuentes append-only**, ordenadas
por fecha:

| Fuente | Entidad | Qué aporta |
|---|---|---|
| **Log propio de la tropa** | `TropaMovimiento` | Recepción, ubicación física en corral, anulación (y a futuro faena/cierre) |
| **Historial de la planificación** | `ListaMatanzaMovimiento` (filtrado por `TropaId`) | Alta/baja/edición de la tropa en las Listas de Matanza |

> No se duplica el historial de la planificación dentro de `TropaMovimiento`: ya vive en
> `ListaMatanzaMovimiento` (append-only, con `TropaId`). La consulta de trazabilidad hace
> el **merge** de ambas fuentes.

### 2.1 `TropaMovimiento` (log append-only propio)

`Meat.Domain/Tropas/TropaMovimiento.cs`. Nunca se edita ni se borra.

| Campo | Descripción |
|---|---|
| `Id` (Guid) | PK |
| `TropaId` (FK) | Tropa a la que pertenece el evento |
| `Secuencia` (int) | Orden del evento dentro de la tropa (1..n). Índice único `(TropaId, Secuencia)` |
| `Fecha` / `UsuarioId` | Cuándo y quién |
| `TipoMovimiento` | Ver `TiposMovimientoTropa` |
| `EstadoResultanteId` (FK) | Estado de la tropa **entera** luego del evento. **FK a `TiposEstadosTropas`**: todo estado resultante debe existir en el catálogo |
| `Detalle` | Texto legible (snapshot) del evento |
| `ReferenciaTipo` / `ReferenciaId` | Documento que originó el evento (ej: `"INGRESO"`) |

## 3. Estados de la Tropa (`TiposEstadosTropas`)

`TiposEstadosTropas` es un catálogo (PK `Codigo`). Contiene **solo estados de la tropa
entera**:

| Código | Nombre | Cuándo |
|---|---|---|
| `RECEPCIONADA` | Recepcionada | Al aprobar el ingreso (la tropa nace) |
| `ANULADA` | Anulada | Al anular el ingreso |

### Por qué la Planificación **no** es un estado de la tropa

Una tropa se planifica **parcialmente** (por cabezas, repartida en varios renglones y
corrales de una Lista de Matanza). Un estado `PLANIFICADA` sobre la tropa entera sería
incorrecto: una tropa con 50 de 100 cabezas planificadas **sigue teniendo 50 disponibles**
y debe seguir apareciendo en las consultas de stock/disponibilidad, que filtran
`EstadoTropaId == RECEPCIONADA`. Por eso la planificación se modela como **evento** en la
trazabilidad (fuente `ListaMatanzaMovimiento`), sin cambiar el estado de la tropa.

### Estados futuros

El Monitor de Faena y la Evaluación de Faena (Ciclo I pasos 3 y 4) introducirán los estados
de consumo real (ej. faena en curso / faenada). **Regla:** cada estado nuevo del ciclo de
vida debe **sembrarse en `TiposEstadosTropas`** (en su migración) antes de usarse como
`EstadoResultanteId`; la FK lo garantiza a nivel de base.

## 4. Fases del ciclo de vida y su registro

| Fase | Estado de tropa | Evento (fuente) | Estado |
|---|---|---|---|
| Recepción / origen | `RECEPCIONADA` | `RECEPCION` (`TropaMovimiento`) | Implementado |
| Ubicación física | `RECEPCIONADA` | `UBICACION` por corral (`TropaMovimiento`) | Implementado |
| Planificación de faena | *sigue `RECEPCIONADA`* | `ALTA_TROPA` / `BAJA_TROPA` / `INCREMENTO` / … (`ListaMatanzaMovimiento`) | Implementado (merge) |
| Ejecución (Monitor) | *(futuro)* | `FAENA` (`TropaMovimiento`) | Pendiente |
| Cierre (Evaluación) | *(futuro)* | `CIERRE` (`TropaMovimiento`) | Pendiente |
| Anulación | `ANULADA` | `ANULACION` (`TropaMovimiento`) | Implementado |

### Tipos de movimiento (`TiposMovimientoTropa`)

`Meat.Application/Tropas/TiposMovimientoTropaConstantes.cs`: `RECEPCION`, `UBICACION`,
`ANULACION` (la planificación no se duplica aquí; futuros: `FAENA`, `CIERRE`).

## 5. Cómo se registra un evento

Los procesos escriben en el log con el helper
`Meat.Application/Tropas/TropaMovimientos.RegistrarAsync(...)`, que resuelve la `Secuencia`
(considerando lo ya persistido y lo agregado en la misma unidad de trabajo) y agrega el
movimiento al contexto. El `SaveChanges` lo hace el handler que llama, para participar de
su transacción.

Enganchado hoy en:
- `AprobarIngresoHaciendaHandler` → `RECEPCION` + `UBICACION` (una por corral).
- `AnularIngresoHaciendaHandler` → `ANULACION`.

**Para extender (Monitor/Evaluación u otro proceso):**
1. Sembrar el/los estado(s) nuevo(s) en `TiposEstadosTropas` (migración).
2. Agregar el código en `TiposMovimientoTropa`.
3. Llamar a `TropaMovimientos.RegistrarAsync(...)` en el handler del proceso.

## 6. Cómo se consulta

- **API:** `GET /Tropas/trazabilidad?numeroTropa={n}&establecimientoId={id?}`
  (`GetTrazabilidadTropa`). Devuelve **todas** las tropas con ese número (por la no-unicidad),
  cada una con su línea de tiempo unificada y ordenada por fecha.
- **Frontend:** página **Trazabilidad de Tropas**
  (`/operaciones/trazabilidad-tropas`). El usuario ingresa **solo el número**; se muestra una
  tarjeta por tropa (número, especie, cliente, establecimiento, ingreso, estado) con la
  línea de tiempo de eventos. La búsqueda se acota al establecimiento activo si hay uno
  seleccionado.

## 7. Nota sobre datos históricos

El log arranca desde la puesta en producción de esta funcionalidad (migración
`37_TropaMovimiento`). Las tropas ya existentes **no** tienen eventos de recepción/ubicación
retroactivos; los nuevos ingresos aprobados/anulados y toda planificación (que ya usaba
`ListaMatanzaMovimiento`) sí quedan trazados.
