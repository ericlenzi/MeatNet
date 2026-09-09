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
- **Desglose por cliente** en todo lo anterior: el cliente es quien paga la faena, así que es el
  corte por el que se discute el resultado.

**Fuera de alcance (ver §6):** rinde frío, desbaste y decomisos. Los tres dependen de datos que
hoy **no se capturan**; no se estiman ni se inventan.

## 2. El rinde: definición exacta y sus supuestos

> **Decidido con el usuario (2026-09-07): en esta etapa solo se calcula rinde caliente.**

```
Rinde caliente (%) = (kg de romaneo / kg vivos de los animales faenados) × 100
```

**Numerador — kg de romaneo.** Suma de `RomaneoPieza.Peso` de los romaneos **no anulados** de la
jornada. Es el peso de la media res / res al salir de la playa de faena, capturado por el
Tipificador. Es un peso **caliente**: todavía no perdió la merma del oreo.

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
4. **No descuenta decomisos.** El numerador es lo efectivamente romaneado; si una media res se
   decomisó, no entró al romaneo y el rinde baja sin explicar por qué (ver §6).
5. **Si nadie ajustó la cantidad en el Ingreso, el denominador es el peso teórico configurado.**
   `PesoPromedio` sale de `PesoIngreso / Cantidad`, y esa `Cantidad` viene precargada con la
   estimación `PesoIngreso / EmpresaTipoEspecie.PesoTeorico`. Si el operador la acepta sin contar
   los animales, el promedio queda pegado al peso teórico que la empresa tiene configurado para esa
   categoría, y el rinde termina midiendo el parámetro en vez de la hacienda. El número gana sentido
   recién cuando la cantidad de la ubicación es la contada, no la estimada.

> El punto 5 tiene una consecuencia práctica: **cambiar el peso teórico de una categoría no
> reescribe el histórico**, porque `PesoPromedio` se calculó y se guardó en el momento del ingreso.
> Dos jornadas de la misma categoría pueden estar calculadas contra pesos teóricos distintos si
> alguien editó la configuración en el medio. Al comparar rindes entre períodos largos, conviene
> mirar si el parámetro se movió.

> Estos supuestos se muestran **en la pantalla**, junto al número. Un rinde sin su definición al
> lado invita a comparar peras con manzanas entre jornadas o contra el rinde de otra planta.

## 3. Secciones del análisis

### 3.1 Resumen de la jornada
Animales faenados, piezas, kg vivos, kg de faena, rinde caliente y estado de liberación.

### 3.2 Por cliente
La misma foto, abierta por cliente (`IngresoHacienda.ClienteId`, alcanzado desde el romaneo por
`Romaneo.TropaId` → `Tropa.IngresoHaciendaId`). Es el corte de facturación del servicio de faena:
animales, kg vivos, kg de faena, rinde y participación en la jornada.

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

## 5. Reglas

- **R-A1 (solo lectura).** El análisis nunca modifica datos. Sin migraciones ni escrituras.
- **R-A2 (sin anulados).** Los romaneos anulados quedan fuera de todos los cálculos: no son carne.
- **R-A3 (rinde sin peso vivo).** Si una tropa no tiene peso de ingreso cargado, su rinde no se
  calcula y se informa como **no disponible**. No se estima ni se reemplaza por un promedio ajeno.
- **R-A4 (disponible desde En Ejecución).** El análisis se puede ver con la jornada abierta, con lo
  faenado hasta el momento. La sección de cámaras queda vacía hasta liberar.
- **R-A5 (definición a la vista).** La pantalla muestra los supuestos del rinde (§2) junto al valor.

## 6. Temas abiertos

- **O-A1 (decomisos) — PENDIENTE, acordado con el usuario que se hará más adelante.**
  `MotivoDecomiso` existe como catálogo pero está **huérfano**: no hay captura ni uso en handlers.
  Falta decidir el grano (por animal o por pieza, total o parcial), dónde se registra (Tipificador,
  o un puesto sanitario propio) y cómo impacta el rinde (¿el decomiso baja el numerador, o se
  informa aparte como merma sanitaria?). Hasta entonces el rinde no los descuenta y la jornada con
  decomisos muestra un rinde bajo sin explicación visible.
- **O-A2 (rinde frío).** Requiere una segunda pesada tras el oreo: un `TipoMedicion` nuevo (hoy
  `TiposMediciones` solo tiene `PESO`) y la pantalla para capturarlo en cámara. Habilitaría además
  la **merma de oreo** como indicador propio.
- **O-A3 (desbaste).** Requiere balanza en playa previa al sacrificio. Con ese dato el rinde pasaría
  a calcularse sobre el peso real de faena y dejaría de estar subestimado.
- **O-A4 (comparativo entre jornadas).** Hoy el análisis es de una jornada. Una vista de evolución
  (rinde por fecha, por cliente, por categoría) es el paso natural siguiente, cuando haya volumen.

## 7. Fuera de alcance
- Costeo y facturación del servicio de faena.
- Integración con el ERP (el puente sigue siendo `ERP_Codigo`).
- Ciclo II (Despostada).
