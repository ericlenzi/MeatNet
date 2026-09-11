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
- **Plan vs. real** por renglón de la Lista de Matanza.
- **Tipificación consolidada**: participación de cada tipificación en piezas y kilos.
- **Pesos y dispersión**: promedio, mínimo, máximo y piezas fuera del rango de su tipificación.
- **Destino a cámaras**: qué materiales y kilos quedaron en cada cámara.
- **Merma sanitaria**: reses condenadas y kilos decomisados de la jornada, abiertos por motivo.
- **Aviso de rinde fuera de rango**, cuando el número se va de la banda esperable de la especie.
- **Desglose por cliente** en todo lo anterior: el cliente es quien paga la faena, así que es el
  corte por el que se discute el resultado.

**Fuera de alcance (ver §6):** rinde frío y desbaste. Los dos dependen de pesadas que hoy **no se
hacen**; no se estiman ni se inventan.

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
   alto (la merma de oreo ronda el 2%, pero eso se mide, no se supone).
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
| Banda de rinde esperable | `Especie.RindeMinimo` / `Especie.RindeMaximo` |
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
- **O-A2 (rinde frío).** Requiere una segunda pesada tras el oreo: un `TipoMagnitud` nuevo (hoy
  `TiposMagnitudes` solo tiene `PESO`) y la pantalla para capturarlo en cámara. Habilitaría además
  la **merma de oreo** como indicador propio.
- **O-A3 (desbaste).** Requiere balanza en playa previa al sacrificio. Con ese dato el rinde pasaría
  a calcularse sobre el peso real de faena y dejaría de estar subestimado.
- **O-A5 (decomiso de vísceras).** Hoy la merma sanitaria cubre la carne. El hígado decomisado, que
  en la práctica es el decomiso más frecuente, no se registra: depende del dominio de subproductos
  (O-3 en `EvaluacionFaena.md`).
- **O-A4 (comparativo entre jornadas).** Hoy el análisis es de una jornada. Una vista de evolución
  (rinde por fecha, por cliente, por categoría) es el paso natural siguiente, cuando haya volumen.

## 7. Fuera de alcance
- Costeo y facturación del servicio de faena.
- Integración con el ERP (el puente sigue siendo `ERP_Codigo`).
- Ciclo II (Despostada).
