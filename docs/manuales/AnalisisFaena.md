# Ciclo I - Paso 4b: Análisis de Faena

## 1. Objetivo y alcance

Cerrar la lectura de la jornada: **qué se planificó y qué se faenó, cuánto rindió, cómo se
tipificó y dónde quedó**. Es la mitad analítica del Paso 4, complementaria de la Liberación
(`EvaluacionFaena.md`), que es la mitad operativa.

Es **read-only**: no crea ni modifica datos, no tiene migraciones y no altera existencia. Lee lo
que dejaron el Ingreso de Hacienda (Paso 1), la Planificación (Paso 2), la Ejecución (Paso 3) y la
Liberación (Paso 4).

**Dentro de alcance:**
- **Rinde caliente** por jornada, por cliente y por tropa.
- **Rinde frío estimado** de la jornada, proyectado con la merma de oreo configurada (R-A8).
- **Plan vs. real** por renglón de la Lista de Matanza.
- **Tipificación consolidada**: participación de cada tipificación en piezas y kilos.
- **Pesos y dispersión**: promedio, mínimo, máximo y piezas fuera del rango de su tipificación.
- **Destino a cámaras**: qué materiales y kilos quedaron en cada cámara.
- **Merma sanitaria**: reses condenadas y kilos decomisados de la jornada, abiertos por motivo.
- **Producción estimada de subproductos**: cuero, sebo y menudencias, por rendimiento (R-A9).
- **Aviso de rinde fuera de rango**, cuando el número se va de la banda esperable de la especie.
- **Desglose por cliente** en todo lo anterior: el cliente es quien paga la faena, así que es el
  corte por el que se discute el resultado.

**Fuera de alcance (ver §6):** el rinde frío **medido** y el desbaste. Los dos dependen de pesadas
que hoy no se hacen, y ninguna se inventa: el peso vivo que falta no se estima nunca (R-A3), y del
frío se muestra una proyección solo cuando alguien cargó el coeficiente, siempre rotulada como
estimación (R-A8).

## 2. El rinde: definición exacta y sus supuestos

> **Decidido con el usuario (2026-09-07): en esta etapa solo se calcula rinde caliente.**

```
Rinde caliente (%) = (kg de romaneo / kg vivos de los animales faenados) × 100
```

**Numerador — kg de romaneo.** Suma de `RomaneoPieza.Peso` de los romaneos **no anulados y no
condenados** de la jornada. Es el peso de la media res / res al salir de la playa de faena, capturado por el
Tipificador. Es un peso **caliente**: todavía no perdió la merma del oreo.

> **Cuántas piezas entran en esa suma lo decide la Unidad de Faena.** El Tipificador exige
> exactamente `UnidadFaena.PiezasPorAnimal` piezas por animal, así que una unidad mal parametrizada
> infla el numerador con piezas que el operador tuvo que inventar para poder cerrar la captura.
> Desde la validación de aritmética de la UF eso ya no puede pasar (ver `EjecucionFaena.md`,
> R-E16). Es el espejo del punto 5 de más abajo: el denominador depende de un parámetro que la
> empresa configura, el numerador dependía de otro.

**Denominador — kg vivos.** No existe pesada individual del animal vivo. Lo que hay es el peso de
ingreso, y de ahí sale el promedio por ubicación:

```
kg vivos del renglón = CantidadFaenada × IngresoHaciendaUbicacion.PesoPromedio
```

La ubicación se identifica por la misma clave que el renglón de la Lista de Matanza:
(`TropaId`, `AlmacenId`, `TipoEspecieId`).

**Supuestos que hay que tener presentes al leer el número:**

1. **El peso vivo es el de ingreso, no el de faena.** Entre la descarga y el sacrificio el animal
   pierde peso (desbaste, típicamente por ayuno y descanso). Como no hay balanza en playa, ese
   descuento no se aplica: **el rinde calculado sale más bajo que el real** en la medida del
   desbaste.
2. **El peso vivo es promedio, no individual.** Se prorratea el promedio de la ubicación por la
   cantidad faenada. A nivel de tropa el promedio es representativo; a nivel de un animal puntual
   no significa nada.
3. **El peso de faena es caliente.** Comparado contra un rinde frío de referencia, este da más
   alto. La pantalla puede mostrar al lado un **rinde frío estimado**, que no es una medición sino
   el caliente menos la merma de oreo configurada (R-A8). El único rinde medido es el caliente.
4. **El decomiso hace bajar el rinde, y la merma sanitaria dice cuánto.** Lo condenado sale del
   numerador (esa carne no llega a la cámara), sea la res entera o una media res, pero el animal
   sigue en el denominador, porque se faenó. Eso hace caer el rinde a propósito, y la **merma
   sanitaria** que se informa al lado es la que explica la caída. El recorte parcial no toca
   ninguno de los dos: el peso de la pieza es el que entró a la cámara, y los kilos retirados se
   suman a la merma (R-A6).
5. **Si nadie ajustó la cantidad en el Ingreso, el denominador es el peso teórico configurado.**
   `PesoPromedio` sale de `PesoIngreso / Cantidad`, y esa `Cantidad` viene precargada con la
   estimación `PesoIngreso / EmpresaTipoEspecie.PesoTeorico`. Si el operador la acepta sin contar
   los animales, el promedio queda pegado al peso teórico que la empresa tiene configurado para esa
   categoría, y el rinde termina midiendo el parámetro en vez de la hacienda. El número gana sentido
   recién cuando la cantidad de la ubicación es la contada, no la estimada. Cuando ese dato está
   directamente mal cargado, el rinde se dispara y la pantalla lo avisa (R-A7).

> El punto 5 tiene una consecuencia práctica: **cambiar el peso teórico de una categoría no
> reescribe el histórico**, porque `PesoPromedio` se calculó y se guardó en el momento del ingreso.
> Dos jornadas de la misma categoría pueden estar calculadas contra pesos teóricos distintos si
> alguien editó la configuración en el medio. Al comparar rindes entre períodos largos, conviene
> mirar si el parámetro se movió.

> Estos supuestos se muestran **en la pantalla**, junto al número. Un rinde sin su definición al
> lado invita a comparar peras con manzanas entre jornadas o contra el rinde de otra planta.

### 2.1 Por qué el caliente se mide y el frío se estima

La diferencia entre los dos rindes no es de fórmula, es de **cuándo se pesa la media res**:

| | Cuándo | Quién lo captura hoy |
|---|---|---|
| **Caliente** | Al salir de la playa de faena, con la res todavía caliente | El Tipificador, res por res (paso 3) |
| **Frío** | Después del oreo, cuando la res perdió agua en la cámara | **Nadie**: esa pesada no existe en la planta |

El sistema **no inventa pesadas**. Si el rinde caliente es el único medido, es porque es el único
cuyo dato se captura: el peso de cada pieza entra en el romaneo porque hace falta para elegir la
tipificación por rango y para dar de alta el stock. Nada pide, hoy, volver a pesar la media res
después del oreo, así que ese número no existe en ninguna tabla.

**Entonces por qué el frío se estima y el peso vivo que falta no.** Parece la misma licencia, y no
lo es:

- El **peso vivo de una tropa** es un hecho de esa tropa. Si nadie lo cargó, cualquier número que
  se ponga es una invención sobre un caso concreto, y el peso de un animal varía tanto que el
  promedio de otra tropa no dice nada. Por eso R-A3 lo deja en **no disponible** y cuenta aparte
  los animales afectados.
- La **merma de oreo** es un parámetro del proceso, no un hecho de una res: es el agua que se
  evapora en la cámara, ronda el 2% y se mueve poco dentro de una misma planta. Aplicarlo es una
  **proyección declarada**, con el porcentaje y su origen a la vista (R-A8), y no reemplaza ningún
  dato faltante de una jornada puntual.

La regla de fondo es la misma en los dos casos: **lo que no se midió no se disfraza de medición.**
El frío se muestra rotulado como estimación y el caliente se muestra sin adjetivos.

**Los dos sesgos que quedan, y por qué no se compensan a propósito.** El peso vivo es el de ingreso
y no descuenta el desbaste, así que el denominador está inflado y el rinde sale **más bajo** que el
real. El peso de faena es caliente, así que el numerador está inflado y el rinde sale **más alto**
que un frío de referencia. Los dos errores apuntan en sentidos opuestos y en la práctica se comen
entre sí, que es la razón por la que el número cae en una banda creíble. **Eso es una casualidad
aritmética, no un diseño:** si mañana se agrega la pesada de playa sin la de cámara, el rinde va a
subir de golpe sin que la planta haya cambiado nada. De ahí que la pantalla muestre los supuestos
al lado del valor (R-A5) y que la banda de la especie compare siempre **caliente contra caliente**
(R-A7).

## 3. Secciones del análisis

### 3.1 Resumen de la jornada
Animales faenados, piezas, kg vivos, kg de faena, rinde caliente y estado de liberación. Cuando la
jornada tuvo decomisos, al lado del rinde aparece la **merma sanitaria** (§3.7); cuando el rinde se
va de la banda esperable de la especie, arriba de todo aparece el aviso de R-A7.

### 3.2 Por cliente
La misma foto, abierta por cliente (`IngresoHacienda.ClienteId`, alcanzado desde el romaneo por
`Romaneo.TropaId` → `Tropa.IngresoHaciendaId`). Es el corte de facturación del servicio de faena:
animales, kg vivos, kg de faena, rinde, **kilos condenados** y participación en la jornada.

El cliente cuyos animales se condenaron enteros aparece igual, con sus kilos vivos y su merma: si
se agrupara solo por la carne, desaparecería de la lista justo la jornada en que peor le fue.

### 3.3 Plan vs. real
Por renglón: tropa, corral, categoría, planificado (`Cantidad`), faenado (`CantidadFaenada`),
diferencia y cumplimiento. El sobrante ya fue liberado del reservado al cerrar la lista (R-17 de
Planificación); acá solo se lee.

### 3.4 Tipificación consolidada
Por tipificación: piezas, kilos, peso promedio y participación porcentual sobre la jornada. Es la
foto comercial: qué proporción de la faena cayó en cada categoría comercial.

### 3.5 Pesos y dispersión
Por categoría (`TipoEspecie`): peso promedio, mínimo, máximo y **piezas fuera del rango** de su
tipificación (`RomaneoPieza.PesoFueraRango`). Sirve para detectar tipificaciones mal parametrizadas,
problemas de balanza o desvíos de la hacienda recibida.

El nombre de la categoría se resuelve contra el **catálogo global** `TiposEspecies`, no contra la
configuración de la empresa. Es lo correcto para un análisis histórico: una jornada de hace seis
meses tiene que seguir mostrando sus categorías con nombre aunque la empresa después haya dejado de
operar con alguna. Vale el mismo criterio que en la disponibilidad de faena
(`docs/manuales/PlanificacionFaena.md` §3).

### 3.6 Destino a cámaras
Qué quedó en cada cámara, por material, en piezas y kilos. Cierra el circuito con la existencia que
generó la Liberación. Antes de liberar, la sección está vacía.

### 3.7 Merma sanitaria
Los decomisos de la jornada, **abiertos por motivo**, que es el informe que mira la inspección. Las
tres formas se cuentan distinto y por eso van en columnas separadas: la **res condenada entera** se
cuenta en animales y aporta todos sus kilos, la **media res condenada** se cuenta en medias reses y
aporta su peso entero, y el **recorte parcial** también se cuenta en medias reses pero aporta solo
los kilos retirados. La sección no aparece si la jornada no tuvo decomisos.

Lo condenado no figura en la tipificación consolidada (§3.4) ni en la dispersión de pesos (§3.5):
no se tipificó, así que no hay rango contra el cual compararlo. Ver R-A6 y, para cómo se capturan,
R-E23, R-E27 y R-E24 en `EjecucionFaena.md`.

> **Por dónde se entra.** El listado de jornadas es el mismo que usa la Evaluación de Faena (la
> pantalla se comparte con una prop), así que su orden, sus filtros y su tope de filas están
> documentados una sola vez, en `EvaluacionFaena.md` §10.

### 3.8 Producción estimada de subproductos

Cuánto cuero, sebo y menudencias dejó la jornada, estimado con el rendimiento configurado de cada
subproducto. Es **producción informada, no existencia**: estos kilos no se pesaron y no entran al
stock de cámara. La sección no aparece si la empresa no cargó rendimientos para la especie.

Muestra el rendimiento aplicado, los kilos de cada subproducto y el total, sobre la base visible:
los kilos que fueron a cámara. Ver R-A9.

## 4. Fuentes de datos

| Dato | Origen |
|---|---|
| Peso vivo | `IngresoHaciendaUbicacion.PesoPromedio` × `ListaMatanzaDetalle.CantidadFaenada` |
| Peso de faena | `RomaneoPieza.Peso` (romaneos no anulados) |
| Plan | `ListaMatanzaDetalle.Cantidad` |
| Real | `ListaMatanzaDetalle.CantidadFaenada` |
| Cliente | `Romaneo.TropaId` → `Tropa.IngresoHaciendaId` → `IngresoHacienda.ClienteId` |
| Tipificación | `RomaneoPieza.TipificacionId` |
| Fuera de rango | `RomaneoPieza.PesoFueraRango` |
| Existencia en cámara | `MovimientoCamara` (saldo derivado) |
| Puesto de la jornada | `ListaMatanza.PuestoId` (ver R-E28 en `EjecucionFaena.md`) |
| Banda de rinde esperable | `Especie.RindeMinimo` / `Especie.RindeMaximo` |
| Rendimiento de subproductos | `RendimientoSubproducto.Porcentaje` (por especie y material) |
| Base de los subproductos | Los mismos kg de faena que van a cámara (sin lo condenado) |
| Merma de oreo de la planta | `EstablecimientoEspecie.MermaOreo` |
| Merma de oreo de referencia | `Especie.MermaOreoReferencia` |
| Res condenada | `Romaneo.DecomisoTotal` + `Romaneo.MotivoDecomisoId` |
| Media res condenada | `RomaneoPieza.Decomisada` + `RomaneoPieza.MotivoDecomisoId` |
| Recorte parcial | `RomaneoPieza.PesoDecomisado` + `RomaneoPieza.MotivoDecomisoId` |

## 5. Reglas

- **R-A1 (solo lectura).** El análisis nunca modifica datos. Sin migraciones ni escrituras.
- **R-A2 (sin anulados).** Los romaneos anulados quedan fuera de todos los cálculos: no son carne.
- **R-A3 (rinde sin peso vivo).** Si una tropa no tiene peso de ingreso cargado, su rinde no se
  calcula y se informa como **no disponible**. No se estima ni se reemplaza por un promedio ajeno.
- **R-A4 (disponible desde En Ejecución).** El análisis se puede ver con la jornada abierta, con lo
  faenado hasta el momento. La sección de cámaras queda vacía hasta liberar.
- **R-A5 (definición a la vista).** La pantalla muestra los supuestos del rinde (§2) junto al valor.
- **R-A6 (el decomiso se informa, no se maquilla).** El rinde conserva su definición: no se le
  suman los kilos condenados para "arreglarlo", ni se saca del denominador el animal que se
  condenó. Lo que se agrega es un indicador propio:

  ```
  Merma sanitaria (%) = (kg condenados / kg vivos) × 100
  ```

  …donde los kg condenados son los de las **reses condenadas enteras**, más los de las **medias
  reses condenadas**, más los **kilos retirados** en los recortes. Se muestra al lado del rinde y
  se abre **por motivo**, que es el informe que mira la inspección.

  Las tres formas se cuentan distinto y por eso van en columnas separadas: la res condenada se
  cuenta en **animales** y aporta todos sus kilos; la media res condenada se cuenta en **medias
  reses** y aporta su peso entero; el recorte también se cuenta en medias reses pero aporta solo
  los kilos retirados. Nada de lo condenado entra en la tipificación consolidada ni en la
  dispersión de pesos: no se tipificó, y no hay rango contra el cual compararlo.

- **R-A8 (rinde frío: se estima, y se dice que es estimado).** No hay segunda pesada tras el oreo,
  así que el frío no se mide: se proyecta.

  ```
  Kg frío estimados      = kg de faena × (1 − merma de oreo / 100)
  Rinde frío estimado (%) = (kg frío estimados / kg vivos) × 100
  ```

  **De dónde sale la merma, en orden de precedencia:**

  | Nivel | Dónde | Por qué ahí |
  |---|---|---|
  | 1 | Peso frío **medido** de la pieza | Cuando exista balanza en cámara, lo medido manda. Hoy no existe (O-A2). |
  | 2 | `EstablecimientoEspecie.MermaOreo` (migración 78) | La merma depende de la cámara, del tiempo de oreo y de la cobertura de grasa: es de la **planta**, no de la empresa. Dos plantas de la misma empresa pueden tener números distintos. |
  | 3 | `Especie.MermaOreoReferencia` (migración 78, sembrada en 2% para V y P) | El valor del rubro, que el SUPERADMIN mantiene en el catálogo. Es el que se propone mientras la planta no mida el suyo. |
  | 4 | Nada configurado | **No se muestra rinde frío.** Misma regla que la banda de rinde: sin parámetro, la pantalla no inventa nada. |

  **Lo que la pantalla dice junto al número:** que es una estimación, el porcentaje aplicado, si
  salió de la planta o de la referencia de la especie, y los kilos que implica. Un coeficiente
  único por planta y especie no distingue tiempo de oreo ni tipo de cámara, que es de donde viene
  la mayor parte de la variación: sirve para dimensionar la merma, no para discutir una jornada
  puntual.

  **Lo que el rinde frío estimado NO hace:** no reemplaza al caliente, no entra en la comparación
  contra la banda de la especie (que es de rinde caliente, R-A7), y **no ajusta la existencia de
  cámara**, que sigue registrada con el peso caliente que le dio la Liberación. Ajustar el stock
  con una merma supuesta sería inventar kilos en el depósito; el día que haya pesada real, ese
  ajuste es un movimiento de cámara y no un cálculo de pantalla (O-A6).

  Un coeficiente fuera de (0, 100) se ignora como si no estuviera: no es un dato de oreo, es un
  error de carga.
- **R-A9 (los subproductos se estiman, y no son existencia).** El animal no produce solo carne: el
  cuero, el sebo y las menudencias son el resto del rendimiento. Hoy **ninguno se pesa**, así que
  la producción se estima con un porcentaje por especie y subproducto
  (`RendimientoSubproducto.Porcentaje`), que el ADMIN carga en Datos Maestros.

  ```
  Kg estimados del subproducto = kg que fueron a cámara × rendimiento / 100
  ```

  **Tres decisiones que sostienen esto, y conviene no revertir sin leerlas:**

  1. **No es un `DespieceMaterial`, y no puede serlo.** El despiece reparte el peso de un material
     entre sus destinos y la suma por origen cierra en 100%: es lo que hace que el cuarteo controle
     masa (987,50 kg que entran, 987,50 que salen). El cuero **nunca estuvo** en el peso de la media
     res, así que agregarlo como un destino más obligaría a inflar o desinflar la carne para hacerle
     lugar. Por eso el rendimiento vive en su propia tabla.
  2. **La base es el peso de la res, no el peso vivo, y sin lo condenado.** Lo correcto de manual
     sería el vivo, pero es justo el dato que a veces falta y que la regla R-A3 prohíbe inventar;
     el de faena está siempre y es medido. De esa base se **excluye lo condenado**: la res que la
     inspección condenó se va entera al digestor, vísceras incluidas, así que estimar su menudencia
     sería informar producción que no existe. El cuero de esa res en la práctica sí se recupera,
     pero distinguirlo pide marcar subproducto por subproducto si se recupera o no, y eso recién
     vale la pena cuando los subproductos se pesen (O-3 en `EvaluacionFaena.md`). Mientras tanto se
     subestima, que es el lado seguro. La pantalla muestra la base, así que el número es auditable.
  3. **Lo estimado no entra al stock.** La existencia de cámara es el número contra el que se
     despacha y se hace inventario; mezclar kilos calculados con kilos pesados en el mismo log
     llevaría a vender un cuero que nadie pesó. Es la misma disciplina que la merma de oreo (R-A8):
     se estima donde informa, nunca donde se convierte en existencia.

  El CRUD valida que el material sea de tipo subproducto (`SUB_PROD` o `MENUD`) — la carne se pesa
  en el romaneo, no se estima —, que el porcentaje esté entre 0 y 100, que no haya dos rendimientos
  para el mismo par especie/subproducto, y que la suma de la especie no pase de 100% del peso de la
  res, que sería una carga imposible.

  **El día que se pese**, el subproducto pesado entra al log como existencia real y la estimación
  pasa a ser el contraste. Eso es el tema O-3 de `EvaluacionFaena.md`, y de él depende el decomiso
  de vísceras (O-A5): no se puede decomisar lo que todavía no es stock.
- **R-A7 (rinde fuera de rango: se avisa, no se corrige).** Cada especie puede declarar la banda
  de rinde caliente que le es esperable, en `Especie.RindeMinimo` / `RindeMaximo`. Si el rinde de
  la jornada queda afuera, la pantalla lo dice arriba de todo.

  **El número no se toca:** no se acota, no se recalcula y no se oculta. Un rinde de 160% es
  aritmética correcta sobre un dato de entrada imposible, y taparlo sería peor que mostrarlo.

  El aviso apunta a la causa habitual, que es el **peso vivo de ingreso**: el denominador sale de
  `PesoPromedio × CantidadFaenada`, así que una tropa con el peso o la cantidad mal cargados
  deforma el rinde entero (es el punto 5 de §2). Cuando la jornada además tuvo decomisos, el aviso
  lo dice, porque ahí la caída puede ser legítima y explicarla la merma sanitaria.

  **La banda la decide el catálogo, no el código.** La mantiene el SUPERADMIN desde la pantalla de
  Especies, y la especie que la deje vacía no dispara ningún aviso: es la misma regla que gobierna
  los datos del palco (R-E22 en `EjecucionFaena.md`). Las bandas iniciales de la migración 74 son
  holgadas a propósito — vacuno 45% a 65%, porcino 65% a 85% — porque el aviso tiene que señalar
  el dato roto, no discutir una jornada floja: el rinde de referencia ronda 55-58% en bovino y
  75-80% en porcino.

## 6. Temas abiertos

- **O-A1 (decomisos) — RESUELTA (2026-09-10): implementada.** Las cuatro decisiones que estaban
  pendientes quedaron así:

  | Pregunta | Decisión |
  |---|---|
  | Grano | **Total por animal** (`Romaneo.DecomisoTotal`), **condena de una media res** (`RomaneoPieza.Decomisada`, R-E27) y **recorte parcial** (`RomaneoPieza.PesoDecomisado`). |
  | Dónde se registra | En el **Tipificador**, en el mismo momento en que se ve la res. Un puesto sanitario propio escribiría en las mismas columnas (O-E2 en `EjecucionFaena.md`). |
  | Impacto en el rinde | **Merma sanitaria aparte** (R-A6): la definición del rinde no cambia. |
  | Peso de la res condenada | **Se pesa.** Sin kilos, la pérdida quedaría en cabezas y no se podría comparar con el rinde. |

  Falta todavía el decomiso de **vísceras y subproductos**, que depende de abrir ese dominio
  (O-3 en `EvaluacionFaena.md`).
- **O-A2 (rinde frío medido): qué hacer si una empresa decide pesar en frío.** La **estimación** ya
  está (R-A8); lo que falta es la medición. El modelo se dejó preparado para que sea un agregado y
  no una reescritura: el peso de una pieza **ya** es una medición tipada, así que el frío es otra
  magnitud de la misma tabla.

  **Lo que hay que construir, en orden:**

  1. **La magnitud.** Sembrar `PESO_FRIO` en `TiposMagnitudes`, al lado de `PESO`. Es una fila, no
     una tabla: `RomaneoPiezaMedicion` ya guarda (pieza, magnitud, valor).
  2. **La caché en la pieza.** `RomaneoPieza.PesoFrio` (nullable) más la fecha y el usuario de esa
     pesada, con el mismo criterio que `Peso`: la medición es el registro canónico y la columna es
     lo que leen las consultas.
  3. **El punto de captura.** Una pantalla de pesada en frío por jornada, que liste las piezas con
     su peso caliente y un campo para el frío. **Tiene que funcionar después de la Liberación**,
     porque el oreo termina cuando la jornada ya está liberada y las piezas son inmutables
     (R-L3): la excepción es que el peso frío no toca nada de lo liberado, solo agrega su propia
     medición.
  4. **El análisis.** Preferir el medido sobre el coeficiente. La pregunta que el handler ya se
     hace es "¿hay peso frío?", así que el cambio es responderla con datos en vez de con el
     parámetro, e informar la **merma de oreo real** (caliente menos frío) como indicador propio
     en lugar de la proyectada. Conviene exponer la **cobertura**: sobre cuántas piezas de la
     jornada se midió, porque un frío calculado sobre la mitad de las piezas no es el de la
     jornada.
  5. **La banda de la especie.** Hoy `RindeMinimo` / `RindeMaximo` son de rinde **caliente**. Con
     el frío medido hacen falta dos bandas, o la aclaración explícita de a cuál aplica cada una.

  **Las decisiones que hay que tomar con el usuario antes de escribir código:**

  | Pregunta | Por qué importa |
  |---|---|
  | ¿Se pesa **cada pieza** o el total de una tanda? | Por pieza da merma individual y cuesta tiempo de operación; por tanda alcanza para el rinde de la jornada pero no permite mirar una media res. |
  | ¿La pesada en frío **ajusta el stock** de cámara? | Es O-A6. Con pesada real el ajuste es legítimo y va como movimiento de cámara, no como cálculo de pantalla. Hay que decidir tipo de movimiento y si se hace automático al cargar el peso. |
  | ¿El peso frío puede **re-tipificar** la pieza? | La tipificación se eligió por rango de peso caliente. Si el frío la saca de rango, la respuesta razonable es no mover la tipificación histórica y solo informarlo. |
  | ¿Qué pasa con las piezas ya cuarteadas? | Si la Liberación las despiezó, el frío se mide sobre los cuartos y no sobre la media res: hay que definir contra qué material se registra. |

  Mientras nada de eso exista, el coeficiente cubre la necesidad de dimensionar la merma, y la
  pantalla deja claro que es una estimación.
- **O-A6 (la merma de oreo y el stock de cámara).** La existencia queda registrada con el peso
  caliente de la Liberación, y el rinde frío estimado no la toca. Reconocer la merma en el
  depósito pide un movimiento de cámara propio, y para eso hace falta la pesada real: un ajuste
  contra un coeficiente movería kilos que nadie pesó.
- **O-A3 (desbaste).** Requiere balanza en playa previa al sacrificio. Con ese dato el rinde pasaría
  a calcularse sobre el peso real de faena y dejaría de estar subestimado.
- **O-A5 (decomiso de vísceras).** Hoy la merma sanitaria cubre la carne. El hígado decomisado, que
  en la práctica es el decomiso más frecuente, no se registra, y la **estimación de subproductos
  (R-A9) no lo habilita**: no se puede decomisar lo que no es existencia. Depende de que los
  subproductos se pesen (O-3 en `EvaluacionFaena.md`).
- **O-A4 (comparativo entre jornadas).** Hoy el análisis es de una jornada. Una vista de evolución
  (rinde por fecha, por cliente, por categoría) es el paso natural siguiente, cuando haya volumen.

## 7. Fuera de alcance
- Costeo y facturación del servicio de faena.
- Integración con el ERP (el puente sigue siendo `ERP_Codigo`).
- Ciclo II (Despostada).
