# Ciclo I - Paso 2: Planificación de Faena (Lista de Matanza)

## 1. Objetivo y alcance

Programar la faena diaria de un Establecimiento mediante la **Lista de Matanza (LM)**:
seleccionar las tropas disponibles (animales En Pie en corrales), definir la **categoría
(TipoEspecie)** y la **cantidad** a faenar por tropa/corral/categoría y la **secuencia** de
sacrificio, manteniendo la **trazabilidad** y el **historial** de cambios de la programación.

**Dentro de alcance:** creación y edición de la LM (cabecera + detalle), reserva de stock
al confirmar, historial/versionado de cambios, faena de emergencia, máquina de estados.

**Fuera de alcance (se documentan aparte):**
- **Monitor de Faena** (Ciclo I Paso 3): ejecución en línea, pesada en Tipificación,
  generación de romaneo, consumo real del stock. La LM es su insumo.
- **Evaluación de Faena** (Ciclo I Paso 4): rindes, tipificación consolidada, plan vs. real.

## 2. Glosario

| Término | Definición |
|---|---|
| **LM / Lista de Matanza** | Programación diaria de faena de un Establecimiento para una Especie. |
| **Renglón** | Línea de la LM: una tropa, en un corral, de una categoría (TipoEspecie), con cantidad y secuencia. |
| **TipoEspecie / Categoría** | Subclasificación del animal En Pie (ej. PADRILLOS, CAPONES, CACHORRO). Una misma tropa/corral puede tener varias; se planifican por separado. |
| **Secuencia** | Orden en que se faenan los renglones. **No** es clave ni identificador; es reordenable. |
| **Dividir** | Partir un renglón en dos (misma tropa/corral/categoría, distinta secuencia y cantidad). |
| **Fusionar** | Unir dos renglones de la misma tropa/corral/categoría en uno. |
| **Reserva** | Stock En Pie apartado por una LM confirmada; no disponible para otra LM. |
| **Faena de emergencia** | Renglón agregado con la LM ya en ejecución (se anexa al final de la secuencia). |
| **Tropa** | Unidad de trazabilidad generada por el Ingreso; su cantidad En Pie es el tope a planificar. |

## 3. Contexto y precondiciones (qué deja el Ingreso)

La LM se arma sobre el stock que produce el Ingreso de Hacienda:

- Solo cuentan **Tropas en estado `RECEPCIONADA`**.
- Solo cuenta hacienda **`EN_PIE`** (los `CAIDOS`/`MUERTOS` no se planifican).
- El stock disponible por **(Tropa, Corral, TipoEspecie)** sale de `IngresoHaciendaUbicacion`
  (`Cantidad`, `PesoPromedio`, `TipoEspecieId`), filtrado por Ingreso `APROBADO`.
- Todo se filtra por **empresa activa** (vía `Establecimiento.Empresa.CodigoEmpresa`)
  y por el **Establecimiento** de contexto.

> Es la misma base que la consulta `ExistenciaHacienda`.

## 4. Decisiones de diseño

1. **Granularidad de la LM:** una LM por **(Establecimiento, Fecha, Especie)**.
   La cabecera lleva `EspecieId`. La secuencia no cruza especies.
2. **Granularidad del renglón:** **Tropa + Corral + TipoEspecie**. Permite ordenar la
   faena por corral y **decidir la categoría** a faenar, validando disponibilidad a
   nivel (Tropa, Corral, TipoEspecie). Una misma tropa/corral con varias categorías
   genera un renglón por categoría.
3. **Reserva de stock:** al **Confirmar**, la LM **reserva** el En Pie planificado.
   El Borrador NO reserva. La reserva es **derivada** (no se materializa un movimiento
   de stock): la disponibilidad se calcula descontando lo planificado por LMs
   `CONFIRMADA`/`EN_EJECUCION`.
4. **Anulada:** es un **estado visible** (no soft-delete). La regla "una LM por
   día/establecimiento/especie" se valida en el handler, ignorando las `ANULADA`.

## 5. Flujo del proceso

```
1. Ingreso aprobado -> tropas RECEPCIONADA con stock EN_PIE en corrales
2. Crear LM [BORRADOR] para (Establecimiento, Fecha, Especie)
3. Agregar renglones: seleccionar (Tropa, Corral, TipoEspecie), cantidad y secuencia
   - editar cantidad (<= disponible), reordenar, dividir, fusionar  (libre)
4. Confirmar LM -> [CONFIRMADA] -> reserva el stock planificado
   - modificaciones controladas y AUDITADAS (historial/versiones)
5. Inicio de faena -> [EN_EJECUCION]
   - los renglones ya faenados (con romaneo) quedan congelados
   - se permite AGREGAR faena de emergencia al final
6. Cierre de jornada (finalizar) -> [FINALIZADA]
   - el consumo real (lo faenado) ya bajo el En Pie en el Monitor
   - se LIBERA el sobrante planificado no faenado (vuelve a estar disponible)
   * En cualquier punto previo a la ejecución: Anular -> [ANULADA]
```

## 6. Máquina de estados

### 6.1 Lista de Matanza (`TipoEstadoListaMatanza`)

```
[BORRADOR] --confirmar--> [CONFIRMADA] --iniciar--> [EN_EJECUCION] --cerrar/finalizar--> [FINALIZADA]
    |                          |                          |
    |--anular--> [ANULADA] <--anular--|                 |--(no se anula una vez
    |                                                          iniciada; ver R-14)
    |--(desconfirmar: CONFIRMADA -> BORRADOR, libera reserva; ver R-13)
```

| Estado | Significado | Edición |
|---|---|---|
| `BORRADOR` | En armado. | Libre (sin auditoría). |
| `CONFIRMADA` | Programación firme; stock reservado. | Controlada + auditada. |
| `EN_EJECUCION` | Faena en curso. | Solo faena de emergencia; renglones faenados congelados. |
| `FINALIZADA` | Jornada **cerrada**: se liberó el sobrante planificado no faenado. | Solo lectura. |
| `ANULADA` | Anulada antes de ejecutar. | Solo lectura. |

### 6.2 Renglón (estado derivado por avance de faena)

El renglón no tiene máquina de estados propia como catálogo, pero su
**editabilidad** depende de `CantidadFaenada`:

```
CantidadFaenada = 0      -> renglón editable (segun estado de la LM)
0 < Faenada < Cantidad   -> parcialmente faenado: NO se reduce por debajo de lo faenado
CantidadFaenada = Cantidad -> renglón completo: congelado
```

> `CantidadFaenada` la actualiza el **Monitor de Faena** (Paso 3). Aquí solo se declara
> el campo y su efecto sobre la edición.

## 7. Operaciones permitidas por estado

| Operación | BORRADOR | CONFIRMADA | EN_EJECUCION |
|---|:---:|:---:|:---:|
| Agregar tropa (renglón) | Sí | Controlado (audita) | Solo emergencia |
| Quitar renglón | Sí | Solo si no faenado (audita) | No |
| Cambiar cantidad (incremento/decremento) | Sí | Sí, >= faenado (audita) | Solo renglón no faenado |
| Cambiar secuencia | Sí | Sí, entre renglones no faenados (audita) | Solo pendientes |
| Dividir / Fusionar | Sí | Controlado (audita) | No |
| Volver a Borrador (desconfirmar, R-13) | — | Sí, solo sin faena registrada (audita) | No |
| Anular LM | Sí | Sí (audita) | No |
| Eliminar LM (soft-delete) | Sí | No | No |

> En `BORRADOR` los cambios **no** generan movimientos de historial. Desde `CONFIRMADA`
> en adelante, **todo cambio registra un `ListaMatanzaMovimiento`** e incrementa `Version`.

## 8. Modelo de datos

Tres entidades de proceso (PK `Guid Id`, Factory) + un catálogo (PK `string Codigo`).

### 8.1 `ListaMatanza` (cabecera)

```
PK: Guid Id

Contexto:
- EstablecimientoId (FK)            [da el filtro por empresa]
- EspecieId (string, FK a Especie)
- Fecha (date)                      [dia de faena]
- NumeroLista (long)                [correlativo por Establecimiento, MAX+1 transaccional]

Control:
- EstadoListaMatanzaId (string, FK a TipoEstadoListaMatanza)
- Version (int)                     [se incrementa con cada cambio post-confirmacion]
- FechaConfirmacion (DateTime?)
- UsuarioConfirmacionId (Guid?)
- FechaInicioEjecucion (DateTime?)
- FechaFinalizacion (DateTime?)
- FechaActualizacion (DateTime)

Navegaciones:
- Renglones   (ICollection<ListaMatanzaDetalle>)
- Movimientos (ICollection<ListaMatanzaMovimiento>)
```

### 8.2 `ListaMatanzaDetalle` (renglón)

```
PK: Guid Id

- ListaMatanzaId (Guid, FK, cascade delete)
- TropaId (Guid, FK)
- AlmacenId (Guid, FK)              [corral de origen]
- TipoEspecieId (string, FK, requerido) [categoria a faenar]
- Secuencia (int)                   [orden de faena; reordenable; no es clave]
- Cantidad (int)                    [animales a faenar de esta tropa/corral/categoria]
- CantidadFaenada (int, default 0)  [lo actualiza el Monitor; congela el renglon]
```

> **No** lleva índice único por `(ListaMatanzaId, TropaId, AlmacenId, TipoEspecieId)`: una
> tropa/corral/categoría puede aparecer en varios renglones (resultado de "dividir"). La
> integridad se cuida por reglas de negocio (R-05, R-06), no por índice.

### 8.3 `ListaMatanzaMovimiento` (historial, append-only)

```
PK: Guid Id

- ListaMatanzaId (Guid, FK, cascade delete)
- Version (int)                     [version resultante tras el cambio]
- Fecha (DateTime)
- UsuarioId (Guid)
- TipoMovimiento (string)           [ALTA_TROPA, BAJA_TROPA, INCREMENTO, DECREMENTO,
                                     CAMBIO_SECUENCIA, DIVISION, FUSION, FAENA_EMERGENCIA,
                                     CONFIRMACION, DESCONFIRMACION, CANCELACION,
                                     INICIO, FINALIZACION, LIBERACION]
- TropaId (Guid?)
- AlmacenId (Guid?)
- CantidadAnterior (int?)
- CantidadNueva (int?)
- SecuenciaAnterior (int?)
- SecuenciaNueva (int?)
- Motivo (string?)
```

> Registro **inmutable**: nunca se edita ni se borra. Es la fuente de verdad del
> historial. `ListaMatanza.Version` da el número de versión visible (v1, v2, ...).

### 8.4 `TipoEstadoListaMatanza` (catálogo, PK `string Codigo`)

Tabla `TiposEstadosListasMatanzas`. Se siembra en la migración.

| Codigo | Nombre |
|---|---|
| `BORRADOR` | Borrador |
| `CONFIRMADA` | Confirmada |
| `EN_EJECUCION` | En Ejecución |
| `FINALIZADA` | Finalizada |
| `ANULADA` | Anulada |

### 8.5 Índices únicos filtrados

```csharp
// Una LM activa (no anulada) por Establecimiento+Fecha+Especie.
modelBuilder.Entity<ListaMatanza>()
    .HasIndex(x => new { x.EstablecimientoId, x.Fecha, x.EspecieId })
    .IsUnique()
    .HasFilter("[FechaBaja] IS NULL AND [EstadoListaMatanzaId] <> 'ANULADA'");

// Numero de lista unico por Establecimiento.
modelBuilder.Entity<ListaMatanza>()
    .HasIndex(x => new { x.EstablecimientoId, x.NumeroLista })
    .IsUnique()
    .HasFilter("[FechaBaja] IS NULL");
```

> La validación de unicidad también se hace en el handler (mensaje amigable HTTP 400),
> el índice es la red de seguridad (HTTP 500 crudo). Se mantienen ambos.

## 9. Reglas de negocio

| # | Regla |
|---|---|
| R-01 | Una LM por **(Establecimiento, Fecha, Especie)**, ignorando las `ANULADA`. Validado en handler + índice único filtrado. |
| R-02 | `NumeroLista` = MAX+1 por Establecimiento, calculado transaccionalmente al crear. No se reutiliza. |
| R-03 | Solo se pueden seleccionar **Tropas `RECEPCIONADA`** con hacienda **`EN_PIE`** de la **Especie** de la LM, en corrales del **Establecimiento** de contexto. |
| R-04 | `Cantidad` de un renglón siempre `> 0`. |
| R-05 | Σ `Cantidad` de todos los renglones de una **(Tropa, Corral, TipoEspecie)** en la LM ≤ **disponible** de esa (Tropa, Corral, TipoEspecie). Ver §10. |
| R-06 | **Dividir/Fusionar** operan a nivel de renglón; la entidad `Tropa` **nunca** se modifica. |
| R-07 | La **Secuencia** solo ordena; se puede renumerar libremente. No identifica el renglón ni es clave de negocio. |
| R-08 | En `BORRADOR` la edición es libre y **no** genera historial. |
| R-09 | Al **Confirmar**: la LM valida disponibilidad (R-05) y **reserva** el stock planificado. Requiere ≥ 1 renglón. |
| R-10 | Desde `CONFIRMADA`, **todo** cambio genera un `ListaMatanzaMovimiento` e incrementa `Version`. No se reemplazan datos sin registrar. |
| R-11 | Un renglón con `CantidadFaenada > 0` no puede reducir su `Cantidad` por debajo de lo faenado, ni cambiar de tropa/corral/categoría. |
| R-12 | Un renglón con `CantidadFaenada = Cantidad` está **congelado** (sin cambios). |
| R-13 | Volver de `CONFIRMADA` a `BORRADOR` (desconfirmar) libera la reserva; solo si ningún renglón tiene `CantidadFaenada > 0`. Registra movimiento `DESCONFIRMACION` e incrementa `Version` (la re-confirmación no resetea el contador). |
| R-14 | En `EN_EJECUCION` solo se permite **faena de emergencia** (nuevo renglón anexado con secuencia al final). No se reordena ni edita lo ya faenado. No se cancela la LM. |
| R-15 | La `CANCELACION` libera la reserva. La LM anulada queda visible (auditoría), no se borra. |
| R-16 | Todo filtrado por empresa activa (vía Establecimiento) y por Establecimiento de contexto. |
| R-17 | **Cierre de la lista** (acción *Finalizar*, `EN_EJECUCION → FINALIZADA`): por cada renglón con sobrante (`Cantidad − CantidadFaenada > 0`) registra un movimiento `LIBERACION` y, al pasar a `FINALIZADA`, ese sobrante deja de reservar (liberación por estado) y vuelve a estar **disponible** para futuras LM. El renglón conserva su `Cantidad` planificada (insumo del plan vs. real). El motivo del cierre es opcional. Todo el cierre es una única `Version`. |
| R-18 | **Un solo ciclo de faena En Ejecución por (Establecimiento, Especie).** Solo se puede *Iniciar* (`CONFIRMADA → EN_EJECUCION`) si **ninguna otra** LM del mismo establecimiento y especie está en estado `EN_EJECUCION`, sin importar el día. A estos efectos, "abierta" = **`EN_EJECUCION` únicamente**: los `BORRADOR` y `CONFIRMADA` de otros días **no** bloquean, para permitir la **pre-planificación** a futuro. En la práctica: hay que *finalizar* la faena en curso antes de iniciar otra. Validado en el handler de iniciar; el frontend además deshabilita el botón y explica cuál lista está En Ejecución. |

## 10. Reserva de stock y disponibilidad

La reserva es **derivada** (no se materializa un movimiento). Para una (Tropa, Corral, TipoEspecie):

```
EnPie(Tropa, Corral, TipoEspecie)   = SUM(IngresoHaciendaUbicacion.Cantidad)
                         WHERE TropaId, AlmacenId, TipoEspecieId, EstadoHacienda = EN_PIE,
                               Ingreso APROBADO
                         menos lo ya faenado historicamente (Monitor).

Reservado(Tropa, Corral, TipoEspecie) = SUM(ListaMatanzaDetalle.Cantidad - CantidadFaenada)
                           de renglones cuya LM este en { CONFIRMADA, EN_EJECUCION }.

Disponible(...) = EnPie(...) - Reservado(...)   [por Tropa, Corral, TipoEspecie]
```

- Al **planificar** (armar el Borrador), el selector de tropas ofrece `Disponible` por categoría.
- Al **confirmar**, se revalida `Σ Cantidad (por Tropa,Corral,TipoEspecie) <= Disponible` (la propia
  LM en Borrador aún no reserva, así que se compara contra el disponible de terceros).
- La **misma LM** no se descuenta a sí misma hasta que pasa a `CONFIRMADA`.

> El consumo **real** del stock (bajar `EN_PIE`) ocurre en el **Monitor** al pesar en
> Tipificación. Ahí `CantidadFaenada` sube y la reserva de ese renglón se convierte en
> consumo. Este manual solo declara el mecanismo; el Monitor lo implementa.

> **Exactitud del stock — las dos mitades.** (1) Lo faenado deja de ser stock: lo baja
> el Monitor vía `CantidadFaenada`. (2) Lo planificado **no** faenado se libera en el
> **cierre** (R-17): al finalizar, esos animales dejan de reservar y vuelven a estar
> disponibles para otra LM. Como la reserva es derivada del estado, la liberación es
> automática al pasar a `FINALIZADA`; el movimiento `LIBERACION` la deja auditada.

> **Impacto en Existencia de Hacienda.** La consulta `ExistenciaHacienda` descuenta el
> `Reservado` **exacto por (Tropa, Corral, TipoEspecie)** de las LM `CONFIRMADA`/`EN_EJECUCION`
> (cada fila de existencia ya es una categoría, así que la reserva se asigna directo, sin
> reparto heurístico) y expone `Reservado`, `Disponible` (UN) y `DisponibleKG` además del
> `En Pie`. Así, apenas se confirma una LM, esa categoría de esos animales deja de figurar
> como disponible en corrales.

## 11. Historial y versionado

- El **estado vivo** de la LM es siempre `ListaMatanza` + `ListaMatanzaDetalle`
  (consulta directa, sin filtrar versiones).
- El **historial** vive en `ListaMatanzaMovimiento` (append-only). Cada cambio
  post-confirmación agrega una fila con el antes/después y el `TipoMovimiento`.
- `ListaMatanza.Version` es el contador visible (v1, v2, ...), que arranca en 1 al
  confirmar y sube con cada movimiento.
- **No** se guardan snapshots completos por versión. Si en el futuro se necesita
  reconstruir la lista "tal como estaba en la vN", se evaluará agregar tablas de
  snapshot; por ahora el log de movimientos cubre la trazabilidad y auditoría.

## 12. Superficie de API (endpoints)

Controller `ListasMatanzasController` (patrón `MeatBaseController`, `[Authorize]`,
`CodigoEmpresa` inyectado del JWT en todos los endpoints):

| Verbo | Ruta | Descripción |
|---|---|---|
| GET | `/ListasMatanzas` | Listado paginado (filtros: Fecha, EspecieId, Estado). |
| GET | `/ListasMatanzas/{id}` | Detalle completo (cabecera + renglones + movimientos). |
| POST | `/ListasMatanzas` | Crea en `BORRADOR`. |
| PUT | `/ListasMatanzas/{id}` | Actualiza cabecera/renglones en `BORRADOR`. |
| DELETE | `/ListasMatanzas/{id}` | Soft-delete (solo `BORRADOR`). |
| POST | `/ListasMatanzas/{id}/renglones` | Alta controlada de renglón en `CONFIRMADA` (audita `ALTA_TROPA`). |
| PUT | `/ListasMatanzas/{id}/renglones/{renglonId}` | Cambio de cantidad/secuencia en `CONFIRMADA`/`EN_EJECUCION` (audita `INCREMENTO`/`DECREMENTO`/`CAMBIO_SECUENCIA`). |
| DELETE | `/ListasMatanzas/{id}/renglones/{renglonId}` | Baja de renglón sin faena en `CONFIRMADA` (audita `BAJA_TROPA`). |
| POST | `/ListasMatanzas/{id}/emergencia` | Faena de emergencia en `EN_EJECUCION`: renglón anexado al final (audita `FAENA_EMERGENCIA`). |
| POST | `/ListasMatanzas/{id}/confirmar` | `BORRADOR` → `CONFIRMADA` (reserva stock). |
| POST | `/ListasMatanzas/{id}/desconfirmar` | `CONFIRMADA` → `BORRADOR` (libera reserva; solo sin faena registrada, R-13). |
| POST | `/ListasMatanzas/{id}/iniciar` | `CONFIRMADA` → `EN_EJECUCION`. Rechaza si ya hay otra LM `EN_EJECUCION` del mismo establecimiento+especie (R-18). |
| POST | `/ListasMatanzas/{id}/finalizar` | **Cierre**: `EN_EJECUCION` → `FINALIZADA`; libera y audita (`LIBERACION`) el sobrante no faenado (R-17). Body opcional `{ Motivo }`. |
| POST | `/ListasMatanzas/{id}/cancelar` | → `ANULADA` (libera reserva). La UI lo presenta como "Anular"; la ruta conserva el nombre `cancelar`. |
| GET | `/ListasMatanzas/disponibilidad` | Tropas/corrales disponibles para planificar (§10). |

> **Estado de implementación (v1).** La edición en **Borrador** (agregar/quitar renglón,
> dividir, fusionar, resecuenciar) se resuelve **en el cliente** y se persiste con el
> `PUT` (reemplazo total del detalle, sin historial — R-08). Desde **Confirmada** en
> adelante se usan los endpoints granulares de renglones (`POST/PUT/DELETE
> …/renglones`, `POST …/emergencia`): cada uno registra su `ListaMatanzaMovimiento` con
> el antes/después y sube `Version`, igual que las transiciones
> `confirmar`/`desconfirmar`/`iniciar`/`finalizar`/`cancelar`.

## 13. Frontend (pantallas)

| Página | Ruta | Menú | Descripción |
|---|---|---|---|
| `PlanificacionFaenaListPage` | `/planificacion-faena` | Planificación de Faena | Bandeja de LMs por fecha/estado. |
| `ListaMatanzaFormPage` | `/planificacion-faena/create`, `/:id/edit` | Planificación de Faena | Armado de la LM: selector de tropas disponibles, grilla de renglones con secuencia, cantidad, dividir/fusionar; botón Confirmar. |
| `ListaMatanzaDetailPage` | `/planificacion-faena/:id` | Planificación de Faena | Vista de la LM con **edición controlada** en Confirmada/En Ejecución (cantidad/secuencia por renglón, quitar, agregar tropa, faena de emergencia — todo auditado), acciones de workflow e **Historial** (movimientos/versiones). |

Reemplaza el `PlaceholderPage` actual del ítem "Planificación de Faena" en el Sidebar.

## 14. Prerequisitos y temas abiertos

- **Puente con Monitor:** `CantidadFaenada` la escribe el Monitor al pesar en
  Tipificación. Definir el contrato exacto (evento/handler) al documentar el Monitor.
- **Numeración:** se asume `NumeroLista` por Establecimiento (MAX+1). Confirmar si debe
  ser por (Establecimiento, Especie) o global.
- **Faena de emergencia (resuelto en v1):** la emergencia siempre sale de una **tropa
  existente En Pie** (misma validación de disponibilidad que un alta normal). Si a
  futuro aparece el caso de emergencia sin tropa en corral, se evaluará con el Monitor.

## 15. Fuera de alcance

Monitor de Faena (ejecución, romaneo, consumo real de stock) y Evaluación de Faena
(rindes, tipificación, plan vs. real). Se documentan en manuales separados.
