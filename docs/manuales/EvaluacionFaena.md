# Ciclo I - Paso 4: Evaluación de Faena (Liberación y Existencia de Cámara)

## 1. Objetivo y alcance

Cerrar el Ciclo I: tomar los **romaneos** capturados en el Paso 3 (Ejecución de Faena) y
**materializar la existencia en cámaras**, expresada en **Materiales** (media res, cuartos,
etc.), lista para que el **Ciclo II (Despostada)** la programe y consuma. Es el paso donde el
inventario **deja de clasificarse por animal** (`TipoEspecie`, en pie) y pasa a clasificarse
por **producto** (`Material`, en cámara).

El Paso 3 solo consume el **En Pie** (baja la hacienda faenada); **no crea stock de cámara**.
La existencia de cámara **nace en la Liberación** de este paso.

**Dentro de alcance:**
- **Revisión y edición** de los romaneos de la jornada antes de fijarlos.
- **Impresión** del romaneo (planilla de la tropa).
- **Liberación**: fija los romaneos como definitivos y **genera la existencia de cámara** en
  Materiales, aplicando el **cuarteo** (despiece de la media res en cuartos u otros materiales)
  cuando corresponde.
- Modelo de **existencia de cámara** (log de movimientos + saldo derivado) con trazabilidad
  hasta el romaneo/garrón de origen.
- **Catálogo de Materiales** y **reglas de despiece** (mini-BOM) como master data.

**Fuera de alcance (se documenta / implementa aparte):**
- **Ciclo II (Despostada):** consumo de la existencia de cámara para producir cortes. Este paso
  solo la **deja disponible**.
- Integración efectiva con el ERP externo (este paso deja el `ERP_Codigo` listo como puente; el
  envío/sincronización es un proyecto aparte).

> **Análisis de Faena (rindes, plan vs. real, tipificación consolidada)** *sí* forma parte del
> Paso 4, pero se **difiere a su propio manual** (`AnalisisFaena.md`) — ver §11. Es analítico
> (read-only) y va después de que la Liberación genere los datos.

## 2. Decisión de arquitectura: dos dominios de existencia y el puente al ERP

El proceso del frigorífico tiene **dos existencias físicas distintas**, y cada una se clasifica
con el catálogo natural de su dominio. **No se unifican en una sola tabla**; se las une el
patrón común y el puente al ERP.

| | **Existencia en pie** (corral) | **Existencia de cámara** (faena) |
|---|---|---|
| Qué es | Animal **vivo** | **Producto** (media res, cuarto, subproducto) |
| Almacén | `Almacen` familia **CORRAL** | `Almacen` familia **CÁMARA** |
| Clasificador | **`TipoEspecie`** (especie + sexo + categoría) | **`Material`** (SKU comercial/ERP) |
| Stock | `IngresoHaciendaUbicacion` (Cantidad, PesoPromedio, Tropa, EstadoHacienda) | Existencia de cámara (§7) |
| Nace en | Ingreso de Hacienda (Paso 1) | **Liberación (este paso)** |

**Patrón común.** Ambas responden a la misma forma:

```
Existencia = (Almacén, Artículo, Cantidad, Peso, trazabilidad → Tropa)
```

…donde **"Artículo" es polimórfico**: en el corral es un animal vivo (`TipoEspecie`), en la
cámara es un producto (`Material`).

**La faena es el límite entre los dos dominios.** El romaneo/liberación es exactamente el punto
donde el inventario deja de ser `TipoEspecie` (vivo) y pasa a ser `Material` (producto). Es el
"nacimiento del material": animal vivo → media res / cuartos.

**Puente al ERP = siempre `ERP_Codigo`.** El ERP externo ve todo como "artículos". Cada catálogo
mapea a su artículo del ERP por la columna **`ERP_Codigo`** (y **solo** por esa columna):
- `TipoEspecie.ERP_Codigo` → artículo del ERP para el animal en pie.
- `Material.ERP_Codigo` → artículo del ERP para el producto de cámara.
- `UnidadFaena.ERP_Codigo` → según necesidad de integración.

> **Nota de limpieza.** Existía una columna `CodigoMaterial` (string suelto, sin FK) en
> `TipoEspecie` y `UnidadFaena` que se solapaba con `ERP_Codigo` como supuesto puente. Se
> **eliminó** (migración 53). El puente es `ERP_Codigo`, no `CodigoMaterial`. `Material` conserva
> su `CodigoMaterial` porque ahí **no** es puente al ERP: es el **código propio** del material
> (análogo a `Almacen.CodigoAlmacen`), y `Material` también tiene su `ERP_Codigo`.

## 3. Glosario

| Término | Definición |
|---|---|
| **Liberación** | Acción de fijar los romaneos de la jornada como definitivos y generar la existencia de cámara. Irreversible en cuanto a edición del romaneo. |
| **Material** | Producto terminado del Ciclo I (`Material`, PK `Guid Id`): media res, cuarto delantero/trasero, subproducto. Es el SKU que hablan el ERP y el Ciclo II. |
| **TipoMaterial** | Catálogo (PK `string Codigo`) que clasifica el material (ej. `MEDIA_RES`, `RES`, `CUARTO`, `SUBPRODUCTO`). |
| **Despiece** | Regla de conversión de un material origen en 1..N materiales destino (mini-BOM). Ej.: MEDIA RES → CUARTO DELANTERO + CUARTO TRASERO. |
| **Cuarteo** | Transformación concreta, en la Liberación, de una media res en sus cuartos, aplicando la regla de despiece. |
| **Existencia de cámara** | Stock de materiales en un almacén familia CÁMARA. Derivada de un log de movimientos (§7). |
| **Movimiento de cámara** | Evento append-only que altera la existencia (ingreso por liberación, baja/alta por transformación, egreso a Ciclo II). |
| **Rendimiento** | Fracción del peso del material origen que va a cada material destino en el despiece (Σ ≈ 1). |

## 4. Contexto y precondiciones (qué deja la Ejecución de Faena)

- La jornada (LM `EN_EJECUCION`) tiene sus `Romaneo` con `RomaneoPieza` cargadas: cada pieza con
  **garrón**, **peso** (`RomaneoPieza.Peso`), **tipificación** (`RomaneoPieza.TipificacionId`) y
  **cámara destino** (`RomaneoPieza.AlmacenDestinoId`, obligatoria — ver R-E13 en
  `EjecucionFaena.md`).
- El **En Pie** ya está consumido por los romaneos no anulados (resta derivada, Paso 3).
- La LM debe estar **cerrada** (`FINALIZADA`, R-17 de Planificación) para liberar; el sobrante no
  faenado ya quedó liberado del reservado. *(Decisión abierta O-1: ver §12 — permitir liberar por
  jornada aún abierta o exigir LM finalizada.)*
- Master data cargado: **Materiales** activos, **TiposMateriales**, `Tipificacion.MaterialId`
  seteado en toda tipificación activa, y **reglas de despiece** para los materiales que se cuartean.

## 5. Modelo conceptual

Cada `RomaneoPieza` (media res / res) es una **instancia física de un `Material`**. La cadena es:

```
RomaneoPieza ──(Tipificacion.MaterialId)──► Material de entrada (ej. "MEDIA RES NOVILLO ESP")
                                               │
                                               ▼  (regla de Despiece del material, si existe)
                          ┌────────────────────┴───────────────────┐
                          │ sin despiece: entra tal cual            │ con despiece: CUARTEO
                          ▼                                         ▼
             1 mov. INGRESO de ese material           N mov. de TRANSFORMACION:
             (media res a su cámara destino)          − baja la media res
                                                       + alta de cada cuarto (materiales destino),
                                                         repartiendo el peso por rendimiento,
                                                         en la cámara destino de la pieza
```

- **Qué material es cada pieza** lo resuelve la **Tipificación** (`Tipificacion.MaterialId`), no la
  Unidad de Faena. La tipificación distingue categoría + destino + tipificación oficial, que es lo
  que el ERP y el Ciclo II necesitan.
- **El cuarteo ocurre al cerrar (dentro del Ciclo I).** Lo que el Ciclo I deja en cámara ya está en
  los materiales finales, con trazabilidad al garrón/romaneo de origen. Si un material no tiene
  despiece activo (ej. media res de exportación que sale entera), entra a cámara tal cual.
- **La cámara destino es por pieza** (`RomaneoPieza.AlmacenDestinoId`): las 2 medias reses de un
  vacuno, y por ende sus cuartos, pueden ir a cámaras distintas.

## 6. Flujo del proceso (Liberación)

```
LM FINALIZADA (jornada cerrada)  ──►  abrir Evaluación de Faena
    │
    ▼
[1] Revisar los romaneos de la jornada (grilla: garrón, piezas, peso, tipificación,
    material resultante, cámara, marcas de fuera de rango).
    │
    ▼
[2] Editar lo que haga falta (peso, tipificación, cámara destino). Solo mientras NO esté
    liberado. Editar recalcula el material/− existencia previstos.
    │
    ▼
[3] Imprimir el romaneo (planilla de la tropa) — opcional, previo o posterior a liberar.
    │
    ▼
[4] Liberar  ──►  por cada RomaneoPieza no anulada de la jornada:
      • resolver Material por Tipificacion.MaterialId (R-L1)
      • buscar Despiece activo del material (R-L4):
          - sin despiece  → 1 movimiento INGRESO (material, cámara destino, cantidad 1, peso)
          - con despiece  → cuarteo: por cada material destino, 1 movimiento TRANSFORMACION_ALTA
                            (peso = peso pieza × rendimiento) + 1 TRANSFORMACION_BAJA de la media res
      • marcar el Romaneo/pieza como LIBERADO (definitivo)
    │
    ▼
[5] La existencia de cámara queda disponible por (Cámara, Material), con trazabilidad a la pieza
    origen. Insumo del Ciclo II.
```

## 7. Modelo de datos

Se reutilizan `Material` y `TipoMaterial` (hoy huérfanos) y se agregan: `Tipificacion.MaterialId`,
`DespieceMaterial` y el log de existencia de cámara.

### 7.1 `Material` (catálogo de producto — ya existe, se pone en uso)
```
PK: Guid Id
- CodigoMaterial (string)          [código propio del material — NO es puente ERP]
- Nombre (string)
- TipoMaterialId (string, FK TipoMaterial)
- UnidadMedidaId (string, FK UnidadMedida)   [KG / UN]
- PesoTeorico (string)             [referencia]
- ERP_Codigo (string)              [PUENTE AL ERP]
- Activo (bool), FechaActualizacion
```
> Falta CRUD + seed (medias reses, reses, cuartos por especie). Ver §8.

### 7.2 `TipoMaterial` (catálogo — ya existe)
```
PK: string Codigo   (ej. MEDIA_RES, RES, CUARTO, SUBPRODUCTO)
- Nombre, Activo
```

### 7.3 `Tipificacion.MaterialId` (nuevo FK)
```
+ MaterialId (Guid, FK Material)   [el material de entrada que produce esta tipificación]
```
> Toda tipificación activa debe tener `MaterialId` (validación en su CRUD, R-L1).

### 7.4 `DespieceMaterial` (regla de conversión — mini-BOM)
```
PK: Guid Id
- MaterialOrigenId (Guid, FK Material)     [ej. MEDIA RES]
- MaterialDestinoId (Guid, FK Material)    [ej. CUARTO DELANTERO]
- Cantidad (int)                           [piezas destino por unidad de origen; normalmente 1]
- Rendimiento (double)                     [fracción del peso origen → este destino; Σ por origen ≈ 1]
- Activo (bool), FechaActualizacion

Índice único filtrado: (MaterialOrigenId, MaterialDestinoId)  filtro FechaBaja IS NULL
```
> Un material **sin** filas de despiece activas entra a cámara **tal cual** (no se cuartea).

> **Semántica de `Cantidad > 1` (decidida 2026-09-07).** Una regla con `Cantidad = N` genera
> **N movimientos de 1 unidad cada uno**, con `Peso = peso origen × Rendimiento / N` — no un solo
> movimiento agregado. Así cada pieza destino queda como línea propia, trazable y movible por
> separado, consistente con el grano por pieza del resto del sistema.
> Ej.: `RES PORCINA → MEDIA RES PORCINA` (Cantidad 2, Rendimiento 1,0) sobre una res de 90 kg
> produce 2 movimientos de 1 media res × 45 kg.

### 7.5 Existencia de cámara — log de movimientos + saldo derivado

**`MovimientoCamara`** (append-only; fuente de verdad):
```
PK: Guid Id
- AlmacenId (Guid, FK Almacen)             [cámara; se valida Familia = CAMARA]
- MaterialId (Guid, FK Material)
- TipoMovimiento (string, FK catálogo)     [INGRESO / TRANSF_BAJA / TRANSF_ALTA / EGRESO]
- Cantidad (int)                           [+ entra, − sale, según el tipo]
- Peso (double)
- RomaneoPiezaOrigenId (Guid?, FK)         [trazabilidad a la media res de origen]
- TransformacionId (Guid?)                 [agrupa la BAJA + las ALTAS de un mismo cuarteo]
- TropaId (Guid?), EspecieId (string?), TipoEspecieId (string?)   [trazabilidad denormalizada]
- Fecha (DateTime), UsuarioId (Guid?)
- Referencia (string)                      [ej. "Liberación LM Nº…"]
```

**Existencia (saldo)**: derivada por `Σ Cantidad` / `Σ Peso` agrupando por (`AlmacenId`,
`MaterialId`) — mismo patrón que el **En Pie derivado** y el log **`TropaMovimiento`**. Se expone
por un handler `GetExistenciaCamara`. Si el Ciclo II necesita consultas/reservas de alta frecuencia,
se agrega un saldo cacheado por (cámara, material) encima del log (no reemplaza el log).

> **Grano:** cada movimiento lleva su `RomaneoPiezaOrigenId`, de modo que cada cuarto se puede
> trazar hasta su media res/garrón. El saldo consultable es agregado por (cámara, material).

### 7.6 Catálogo `TiposMovimientoCamara` (nuevo)
| Codigo | Nombre |
|---|---|
| `INGRESO` | Ingreso por liberación |
| `TRANSF_BAJA` | Baja por transformación (cuarteo) |
| `TRANSF_ALTA` | Alta por transformación (cuarteo) |
| `EGRESO` | Egreso a Ciclo II *(lo usará Despostada)* |

### 7.7 Estado de liberación del romaneo
```
Romaneo + RomaneoPieza: agregar marca de liberación (bool Liberado / FechaLiberacion),
o un estado. Una pieza liberada no admite edición (R-L3) y ya generó su existencia.
```

## 8. Master data a preparar (Fase previa)

1. **CRUD + seed de `TipoMaterial`** (MEDIA_RES, RES, CUARTO, SUBPRODUCTO…).
2. **CRUD + seed de `Material`** (por especie: media res, res, cuarto delantero, cuarto trasero…),
   con su `UnidadMedida`, `PesoTeorico` y `ERP_Codigo`.
3. **`Tipificacion.MaterialId`**: migración + cablear el CRUD de Tipificaciones + validar que toda
   tipificación activa tenga material.
4. **CRUD + seed de `DespieceMaterial`** para los materiales que se cuartean (rendimientos por
   especie; ej. bovino MEDIA RES → CUARTO DELANTERO ~0,52 + CUARTO TRASERO ~0,48).

## 9. Reglas de negocio

- **R-L1 (material por tipificación).** El material de cada pieza sale de `Tipificacion.MaterialId`.
  Toda tipificación activa debe tenerlo; si falta, la Liberación de esa pieza se bloquea con aviso.
- **R-L2 (cámara = destino de la pieza).** La existencia se crea en `RomaneoPieza.AlmacenDestinoId`.
  Se valida que sea una cámara (`Familia = CAMARA`) activa del establecimiento de la LM.
- **R-L3 (liberación definitiva).** Al liberar, el romaneo/pieza queda **inmutable** (no admite
  edición). La corrección posterior requiere un contramovimiento explícito (fuera de MVP) o anular
  antes de liberar.
- **R-L4 (despiece condicional).** Si el material tiene despiece activo → se cuartea (transformación);
  si no → entra tal cual. El reparto de peso usa `DespieceMaterial.Rendimiento`; se valida que la Σ de
  rendimientos por material origen sea ≈ 1 (tolerancia configurable) para no perder ni inventar kilos.
- **R-L5 (trazabilidad).** Todo movimiento generado por la liberación lleva `RomaneoPiezaOrigenId` y
  la tropa/especie denormalizadas, para reconstruir el origen de cada material en cámara.
- **R-L6 (no liberar anulados).** Las piezas de romaneos anulados no generan existencia.
- **R-L7 (idempotencia).** Liberar una jornada ya liberada no duplica movimientos (se libera una vez;
  las piezas liberadas se saltean).
- **R-L8 (piezas sin cuartear que igual cambian de material).** El despiece admite `MaterialDestino`
  distinto sin ser "cuarto" (ej. reclasificación); el mecanismo es el mismo (transformación 1→1).

## 10. Superficie de API (borrador)

Controller `EvaluacionFaenaController` (o extender el de Romaneos):

| Verbo | Ruta | Descripción |
|---|---|---|
| GET | `/EvaluacionFaena/romaneos?listaMatanzaId=` | Romaneos de la jornada con material resultante y previsualización de existencia. |
| PUT | `/EvaluacionFaena/pieza/{id}` | Editar peso/tipificación/cámara de una pieza no liberada. |
| GET | `/EvaluacionFaena/imprimir?listaMatanzaId=` | Datos de la planilla para impresión. |
| POST | `/EvaluacionFaena/liberar` | Libera la jornada: genera los movimientos de cámara y fija los romaneos. |
| GET | `/ExistenciaCamara?almacenId=&materialId=` | Saldo de existencia de cámara (derivado del log). |

## 11. Análisis de Faena (diferido a su propio manual)

Parte del Paso 4, pero se desarrolla en **`AnalisisFaena.md`** cuando se implemente. Es análisis
**read-only** (rindes, plan vs. real, tipificación consolidada, mermas) sobre los datos del Paso 3
y de la Liberación; no altera datos y va **después** de generar la existencia.

**Precondición no técnica:** antes de especificarlo hay que **definir las reglas de negocio del
rinde** — rinde caliente vs. frío, si el peso vivo de referencia es el de ingreso o el de balanza
en playa (desbaste), e imputación de mermas/decomisos. Fuentes ya trazadas: peso vivo
(`IngresoHacienda`), peso de faena (`RomaneoPieza.Peso`), peso en cámara (existencia), enlace por
`Romaneo.TropaId`.

## 12. Temas abiertos

- **O-1 (momento de liberar) — RESUELTA (2026-09-07): exigir LM `FINALIZADA`.** Solo se libera con
  la jornada cerrada; no hay liberación progresiva. Evita los casos de borde de anular un romaneo ya
  liberado. Si la operación en planta pide cargar la cámara sobre la marcha, se reevalúa.
- **O-2 (saldo materializado).** ¿Alcanza el saldo derivado del log, o el Ciclo II necesita una tabla
  de saldo cacheada con reservas? Se decide al diseñar el consumo de Despostada.
- **O-3 (subproductos y decomisos) — RESUELTA (2026-09-07): fuera del MVP.** El MVP de liberación
  cubre solo la carne (media res / res / cuartos). Los materiales de subproducto (Cuero Vacuno, Sebo,
  Menudencias) ya están cargados en el catálogo y quedan listos para una fase posterior, que deberá
  definir de dónde sale su peso (no sale de un despiece por rendimiento de la media res) y a qué
  almacén ingresan.
- **O-4 (reverso post-liberación).** Contramovimiento para corregir una liberación equivocada
  (hoy: anular antes de liberar). A evaluar cuando aparezca la necesidad operativa.

## 13. Fuera de alcance
- Ciclo II (Despostada): consumo de la existencia de cámara.
- Envío/sincronización efectiva con el ERP (el `ERP_Codigo` queda como puente listo).
- Subproductos/decomisos como existencia (O-3), reverso de liberación (O-4).
