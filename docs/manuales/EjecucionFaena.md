# Ciclo I - Paso 3: Ejecución de Faena (Romaneo y Tipificador)

## 1. Objetivo y alcance

Ejecutar en línea la faena planificada por la **Lista de Matanza (LM)**: registrar, **res por
res**, lo que realmente se faena (**romaneo**), capturando **garrón**, **peso** y la
**tipificación** de cada pieza, y **consumir el stock** (baja de En Pie, avance del plan).

Es el paso que convierte la reserva de la Planificación (Paso 2) en **hecho consumado**:
lo faenado deja de estar En Pie y sube `CantidadFaenada` del renglón; lo no faenado se libera
al **cerrar** la LM (R-17, ya implementado en Planificación).

**Dentro de alcance (Fase 2 - MVP):**
- Especies **V (VACUNO)** y **P (PORCINO)** únicamente.
- Captura de romaneo: **garrón + peso (medición) + tipificación**.
- Consumo real de stock, trazabilidad de la tropa (`FAENA`) e incremento de `Tipificacion.Puntos`.
- **Tipificador** (pantalla de captura por res) y **Monitor de Faena** (tablero de supervisión en
  vivo, solo lectura).
- Anulación de romaneo (corrección de errores, devuelve el stock).

**Fuera de alcance (se documentan / implementan aparte):**
- **Fase 2b:** dentición, contusiones, decomisos y mediciones adicionales por pieza; otras especies.
- **Evaluación de Faena** (Ciclo I Paso 4): rindes, tipificación consolidada, plan vs. real. Es
  **análisis post-faena** (read/BI) sobre los datos que genera este paso; no forma parte de la Fase 2.
- Integración con balanza/hardware de puesto (en MVP el peso es carga manual).

## 2. Glosario

| Término | Definición |
|---|---|
| **Romaneo** | Registro de la faena de **un animal**: 1 garrón, sus piezas pesadas y tipificadas. Unidad que consume stock. |
| **Pieza / Romaneo Pieza** | Cada unidad física pesada del animal. **PORCINO:** 1 (RES). **VACUNO:** 2 (MEDIA RES, letras A y B). |
| **Garrón** | Número físico de gancho del que cuelga la pieza. En vacuno, las 2 medias reses del animal comparten el nº de garrón (se distinguen por letra A/B). |
| **Unidad de Faena (UF)** | `UnidadFaena` (RES / MEDIA RES / …). Define **cuántas piezas** tiene el romaneo por especie. |
| **Tipificación** | `Tipificacion` (parametrizable por empresa): clasifica la pieza por especie, categoría, UF, destino, tipificación oficial y **rango de peso**. |
| **Medición** | Valor capturado de un `TipoMedicion` del catálogo. En MVP la única medición es **`PESO`**. |
| **Tipificador** | Puesto/pantalla donde se captura el romaneo res por res. |
| **Monitor de Faena** | Tablero **read-only** con el avance de la jornada en vivo. No captura. |
| **Jornada** | La faena de una LM `EN_EJECUCION`. La LM es la cabecera; no hay entidad "Faena" aparte. |

## 3. Contexto y precondiciones (qué deja la Planificación)

- Existe una **LM en estado `EN_EJECUCION`** para el (Establecimiento, Especie) — la inició el
  Paso 2 (`POST /ListasMatanzas/{id}/iniciar`). Por **R-18**, hay a lo sumo **una** LM
  `EN_EJECUCION` por (Establecimiento, Especie): esa es la jornada activa.
- La LM tiene **renglones** (`ListaMatanzaDetalle`) con `(Tropa, Corral, TipoEspecie)`,
  `Cantidad` planificada, `Secuencia` de faena y `CantidadFaenada` (arranca en 0).
- El stock reservado ya descuenta de Existencia de Hacienda (Paso 2). El **consumo real** lo
  hace este paso.
- Master data cargado (Fase 1): `UnidadesFaenas`, `Tipificaciones` (con rango de peso),
  `Numeradores` (con un `ROMANEO` por Establecimiento+Especie), catálogo `TiposMediciones` con
  el código **`PESO`**.

## 4. Decisiones de diseño

1. **Unidad de captura configurable por `UnidadFaena`.** El romaneo tiene 1..N piezas según la UF
   elegida: **PORCINO → RES (1 pieza)**, **VACUNO → MEDIA RES (2 piezas, A/B)**.
2. **Selección de renglón híbrida.** El Tipificador **sugiere** el renglón por `Secuencia` (el
   primero con pendiente), pero el operador puede **overridear** y elegir otro renglón de la LM.
3. **Peso como Medición.** El peso se modela como un `TipoMedicion` (`PESO`) del catálogo, no como
   un concepto ad-hoc. `RomaneoPieza.Peso` es una **caché desnormalizada** de esa medición
   (canónica para elegir tipificación por rango y para KG de stock). Deja la puerta abierta a más
   mediciones en Fase 2b sin cambiar el esquema.
4. **La LM `EN_EJECUCION` es la cabecera de la jornada.** Los romaneos cuelgan de la LM y del
   renglón; no se crea una entidad "Faena".
5. **El romaneo (animal) es la unidad que consume stock.** 1 romaneo = 1 animal = `CantidadFaenada += 1`,
   sin importar cuántas piezas tenga (vacuno: 2 medias reses = 1 animal).
6. **Anulación, no borrado.** Un romaneo mal cargado se **anula** (`Anulado = true`), lo que revierte
   el consumo. El historial de la tropa es append-only (se registra la reversa, no se borra).

## 5. Flujo del proceso (Tipificador)

```
LM EN_EJECUCION  ──►  abrir Tipificador
    │
    ▼
[1] El sistema sugiere el renglón activo (menor Secuencia con pendiente > 0).
    El operador puede elegir otro renglón (híbrido).
    │
    ▼
[2] El sistema PROPONE el Nº de Garrón (último de la jornada + 1) y la Unidad de Faena
    (default por especie: P=RES, V=MEDIA RES). La letra de cada pieza es automática
    (V: A y B; P: sin letra). El operador confirma o ajusta el garrón/UF si difiere de la playa.
    │
    ▼
[3] Destino comercial: selector con default sticky por jornada/tropa (filtra las
    tipificaciones candidatas). El operador lo ajusta al cambiar de tropa si hace falta.
    │
    ▼
[4] Captura de piezas:
    - PORCINO: 1 pieza (sin letra). Ingresa Peso.
    - VACUNO : 2 piezas (letra A y B). Ingresa Peso de cada media res.
    Por cada pieza el sistema PROPONE la Tipificacion:
      • Sticky: mantiene la última tipificación elegida MIENTRAS el peso siga dentro de su rango.
      • Si el peso cae FUERA de rango → re-propone por (especie+categoria+UF+destino+peso), orden Puntos.
    El operador puede cambiarla manualmente (y esa pasa a ser la sticky).
    │
    ▼
[5] Confirmar romaneo  ──►  CrearRomaneo:
      • asigna NumeroRomaneo (Numerador ROMANEO del Estab+Especie)
      • persiste Romaneo + Piezas + Mediciones (PESO)
      • CantidadFaenada += 1 en el renglón
      • baja el En Pie (derivado) / actualiza Existencia
      • TropaMovimiento(FAENA) según grano (§8)
      • Tipificacion.Puntos += 1 por cada tipificación usada
    │
    ▼
[6] El romaneo aparece en la grilla de la jornada (abajo). Repetir [1..5].
    Si hubo error → Anular romaneo (revierte todo).
    │
    ▼
Al terminar → volver a la LM y Cerrar (Finalizar, Paso 2) para liberar el sobrante.
```

El **Monitor de Faena** muestra en paralelo, read-only, el avance agregado de la jornada.

## 6. Reglas de negocio

- **R-E1 (especies MVP).** Solo se romanea LM de especie `V` o `P`. Otras especies quedan fuera.
- **R-E2 (piezas por UF).** El número de piezas por animal se deriva de la UF:
  `piezas = 4 / UnidadFaena.CantidadCuartos` (una res entera = 4 cuartos) → **RES (4 cuartos) = 1
  pieza sin letra; MEDIA RES (2 cuartos) = 2 piezas con letras `A`/`B`; CUARTO (1) = 4**. La letra
  la asigna el servidor. Validar que se carguen todas. (No se usa `UnidadComplementaria`: en la data
  real viene en 0.)
- **R-E3 (garrón autopropuesto).** El sistema propone `NumeroGarron = último garrón de la jornada + 1`
  (primer romaneo → 1); el operador puede ajustarlo (garrón físico: puede saltear ganchos o arrancar
  en otro número), y a partir del valor confirmado la propuesta se autoincrementa. **Único por LM**
  (jornada): índice `(ListaMatanzaId, NumeroGarron)`. En vacuno el garrón identifica al animal y sus 2
  piezas lo comparten; la **letra** (`A`/`B`) es automática, no se teclea. Se reutiliza en otra jornada.
- **R-E4 (numeración de romaneo).** `NumeroRomaneo` es correlativo **por (Establecimiento, Especie)**
  vía `Numerador` con `TipoNumerador = "ROMANEO"` (get-or-create + `UltimoNumero += 1`, transaccional).
  Uno por **animal** (Romaneo), no por pieza.
- **R-E5 (tope por renglón).** `CrearRomaneo` valida `pendiente = Cantidad - CantidadFaenada > 0`
  en el renglón elegido. **No** se puede faenar por encima de lo planificado desde el Tipificador;
  para exceder, primero se amplía el renglón con la **faena de emergencia** de la LM (Paso 2, audita).
- **R-E6 (peso requerido).** Cada pieza requiere su medición `PESO` (> 0). Sin peso no hay
  tipificación posible.
- **R-E7 (tipificación — propuesta y sticky híbrido).** El default sale del filtro duro
  `Especie + TipoEspecie + UnidadFaena + DestinoComercial(seleccionado) + Activo` con
  `PesoDesde <= Peso <= PesoHasta`, ordenado por `Puntos` desc (la más usada primero). Comportamiento
  **sticky híbrido**: se mantiene la última tipificación elegida **mientras el peso de la nueva pieza
  siga dentro de su rango**; si cae **fuera**, se re-propone automáticamente por el filtro anterior.
  El operador siempre puede cambiarla manualmente (y esa pasa a ser la sticky). Al confirmar,
  `Puntos += 1` en cada tipificación usada.
- **R-E11 (destino comercial).** Es un **selector** del Tipificador con **default sticky por
  jornada/tropa** que **filtra** las tipificaciones candidatas (útil cuando toda una tropa va a un
  mismo destino, ej. exportación vs. consumo). Al cambiar de tropa/renglón, el operador puede
  ajustarlo. No se persiste como columna aparte: queda **embebido** en la `Tipificacion` elegida.
- **R-E8 (consumo de stock).** Ver §7.
- **R-E9 (anulación).** Solo el creador/roles habilitados; revierte `CantidadFaenada`, restaura el
  En Pie y registra la reversa en la trazabilidad. No se borra la fila (queda `Anulado = true`).
- **R-E10 (estado de tropa).** La tropa permanece `RECEPCIONADA` mientras le queden animales En Pie;
  pasa a `FAENADA` solo cuando se consume su último animal (ver §8).

## 7. Consumo de stock y disponibilidad

Al confirmar un romaneo **no anulado** de un animal, sobre su renglón `(Tropa, Corral, TipoEspecie)`:

```
1) ListaMatanzaDetalle.CantidadFaenada += 1     (tope R-E5)

2) En Pie baja por RESTA DERIVADA (no se muta el Ingreso):
   EnPie(T,C,Cat) = Σ IngresoHaciendaUbicacion.Cantidad  (En Pie, Ingreso aprobado, tropa Recepcionada)
                    − Σ Romaneo (animales faenados NO anulados) de (T,C,Cat)
   → extender ListaMatanzaStock.GetEnPieAsync y GetExistenciaHaciendaHandler.

3) Disponible(T,C,Cat) = EnPie − Reservado
   Reservado = Σ (Cantidad − CantidadFaenada) de LMs CONFIRMADA/EN_EJECUCION.
```

> **Sin doble conteo.** El faenado se resta de `EnPie` **y** de `Reservado` (vía `CantidadFaenada`);
> ambos términos se cancelan en `Disponible` mientras la LM corre (los animales faenados no están ni
> en pie ni reservados) y quedan correctos una vez que la LM `FINALIZA` (libera el sobrante, R-17).

## 8. Trazabilidad de la tropa

Se registra en el log append-only `TropaMovimiento` (helper `TropaMovimientos.RegistrarAsync`),
con **grano grueso** para no generar cientos de filas por tropa:

- **Primer romaneo de la tropa en la jornada** → movimiento `FAENA`, `EstadoResultante = RECEPCIONADA`
  (la tropa sigue teniendo animales En Pie), `Detalle` = "Inicio de faena, LM Nº…".
- **Último animal de la tropa consumido** (En Pie llega a 0) → movimiento `FAENA`,
  `EstadoResultante = FAENADA`, `Detalle` = "Faena completa (N animales)".
- **Anulación** que vuelve a dejar la tropa con animales En Pie → movimiento de reversa
  (append-only), reponiendo `EstadoResultante = RECEPCIONADA` si correspondía.

El detalle res-por-res vive en `Romaneo`/`RomaneoPieza`; la trazabilidad de la tropa solo marca los
hitos. Requiere:
1. Sembrar `FAENADA` en `TiposEstadosTropas` (migración 43).
2. Agregar `Faena` a `TiposMovimientoTropa`.

## 9. Modelo de datos

Tres entidades de proceso (PK `Guid Id`, Factory). La LM y sus renglones (Paso 2) son la cabecera.

### 9.1 `Romaneo` (por animal — consume stock)
```
PK: Guid Id
- ListaMatanzaId (Guid, FK)            [jornada = LM EN_EJECUCION]
- ListaMatanzaDetalleId (Guid, FK)     [renglón elegido: Tropa+Corral+TipoEspecie]
- TropaId (Guid, FK)                   [denormalizado del renglón; trazabilidad directa]
- EspecieId (string, FK)
- UnidadFaenaId (Guid, FK)             [RES / MEDIA RES; define nº de piezas]
- NumeroGarron (int)                   [físico; único por LM]
- NumeroRomaneo (long)                 [correlativo Numerador ROMANEO por Estab+Especie]
- Fecha (DateTime), UsuarioId (Guid?)
- Anulado (bool, default false)
Navegación: Piezas (ICollection<RomaneoPieza>)
```

### 9.2 `RomaneoPieza` (por pieza pesada — P:1 / V:2)
```
PK: Guid Id
- RomaneoId (Guid, FK, cascade delete)
- Letra (string?, "A"/"B"; null porcino)
- TipificacionId (string, FK a Tipificacion.Codigo)
- Peso (double)                        [caché de la medición PESO; canónico p/ tipificación y KG]
Navegación: Mediciones (ICollection<RomaneoPiezaMedicion>)
```

### 9.3 `RomaneoPiezaMedicion` (mediciones — extensible; MVP solo PESO)
```
PK: Guid Id
- RomaneoPiezaId (Guid, FK, cascade delete)
- TipoMedicionId (string, FK a TiposMediciones)   [MVP: "PESO"]
- Valor (double)
```

### 9.4 Catálogo `TiposEstadosTropas` (agregar estado)
| Codigo | Nombre |
|---|---|
| `RECEPCIONADA` | Recepcionada *(ya existe)* |
| `ANULADA` | Anulada *(ya existe)* |
| `FAENADA` | Faenada *(nuevo — seed migración 43)* |

### 9.5 Índices únicos filtrados
```csharp
// Garrón único por jornada (LM).
modelBuilder.Entity<Romaneo>()
    .HasIndex(x => new { x.ListaMatanzaId, x.NumeroGarron })
    .IsUnique()
    .HasFilter("[FechaBaja] IS NULL AND [Anulado] = 0");

// Nº de romaneo único por Establecimiento+Especie (vía la LM; validar en handler,
// e índice de apoyo por (ListaMatanzaId, NumeroRomaneo) para lecturas de jornada).
```
> La unicidad de `NumeroRomaneo` por (Establecimiento, Especie) la garantiza el `Numerador`
> transaccional; el índice físico de apoyo es por LM. Ver `[[project_unique_indexes]]`.

## 10. Superficie de API (endpoints)

Controller `RomaneosController` (patrón `MeatBaseController`, `[Authorize]`, `CodigoEmpresa` del JWT):

| Verbo | Ruta | Descripción |
|---|---|---|
| GET | `/Romaneos/renglones?listaMatanzaId=` | Renglones de la LM con Cantidad/Faenada/pendiente, el renglón sugerido (menor secuencia con pendiente) y el **próximo garrón sugerido** (último de la jornada + 1). Modo híbrido. |
| GET | `/Romaneos/sugerir-tipificacion?especieId=&tipoEspecieId=&unidadFaenaId=&destinoComercialId=&peso=` | Devuelve la `Tipificacion` propuesta (match por rango de peso, orden Puntos) y la lista de candidatas para el combo. |
| GET | `/Romaneos/jornada?listaMatanzaId=` | Romaneos de la jornada (grilla del Tipificador). |
| GET | `/Romaneos/monitor?listaMatanzaId=` | Totales en vivo: faenado/planificado global y por tropa/categoría, KG, ritmo. |
| POST | `/Romaneos` | Crea un romaneo (piezas + mediciones); aplica el consumo de stock (§7) y trazabilidad (§8). |
| POST | `/Romaneos/{id}/anular` | Anula el romaneo; revierte el consumo. |

## 11. Frontend (pantallas)

| Página | Ruta | Menú | Descripción |
|---|---|---|---|
| `TipificadorPage` | `/operaciones/ejecucion-faena/:listaMatanzaId/tipificador` | (desde detalle de LM `EN_EJECUCION`) | Captura res por res: renglón sugerido con override, garrón, UF, piezas (1 P / 2 A-B V) con peso y tipificación autopropuesta editable; grilla de romaneos de la jornada con **Anular**. |
| `MonitorFaenaPage` | `/operaciones/ejecucion-faena/:listaMatanzaId/monitor` | Ejecución de Faena | Tablero **read-only** de supervisión: totales en vivo (faenado vs planificado, por tropa/categoría, KG, ritmo). Refresco por **polling** (intervalo corto). |

Acceso desde el detalle de la LM `EN_EJECUCION` (botones "Ejecutar / Tipificar" y "Monitor").
El detalle de la LM muestra además el avance `CantidadFaenada / Cantidad` por renglón.

## 12. Prerequisitos y temas abiertos

- **Numerador `ROMANEO`:** debe existir un `Numerador` (`TipoNumerador = "ROMANEO"`) por
  (Establecimiento, Especie). Si falta, `CrearRomaneo` lo crea (get-or-create) arrancando en 0.
- **Medición `PESO`:** debe existir el `TipoMedicion` con código `PESO` (Fase 1 lo siembra por
  script; validar su presencia).
- **Tipificaciones cargadas:** el matcheo por rango de peso requiere `Tipificaciones` activas que
  cubran los pesos esperados; si no matchea ninguna, el operador la elige manualmente y se registra
  igual (no bloquea la faena).
- **Balanza:** integración de hardware de puesto fuera de MVP (peso manual).
- **Grano de trazabilidad:** adoptado grano grueso (§8); si se necesita el detalle por animal en el
  timeline, se evaluará en Evaluación de Faena (Paso 4).
- **Selector de destino comercial (a revisar):** hoy solo pre-filtra el combo de tipificaciones y el
  destino ya viene embebido en la `Tipificacion` elegida, así que solo aporta cuando una misma
  `(categoría, UF, rango de peso)` mapea a varias tipificaciones que difieren por destino. Pendiente
  decidir si el destino se elige en el puesto (res por res), se hereda de la tropa/LM (atributo
  comercial definido al planificar) o queda totalmente implícito. Por ahora se deja como está.

## 13. Fuera de alcance
- Fase 2b (dentición, contusiones, decomisos, más mediciones, otras especies).
- Evaluación de Faena (Paso 4): rindes, tipificación consolidada, plan vs. real.
- Integración con balanza / captura automática en puesto.
