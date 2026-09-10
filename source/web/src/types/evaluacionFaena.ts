export interface PiezaEvaluacionItem {
  id: string
  letra: string | null
  peso: number
  pesoFueraRango: boolean
  liberado: boolean
  almacenDestinoId: string
  almacenDestinoNombre: string | null
  tipificacionId: string
  tipificacionDescripcion: string | null
  materialId: string | null
  materialCodigo: string | null
  materialNombre: string | null
  /** Decomiso parcial de esta media res: kilos retirados por la inspeccion. */
  motivoDecomisoNombre: string | null
  pesoDecomisado: number
}

export interface RomaneoEvaluacionItem {
  id: string
  numeroRomaneo: number
  numeroGarron: number
  tropaId: string
  numeroTropa: number
  tipoEspecieId: string
  tipoEspecieNombre: string
  unidadFaenaNombre: string
  fecha: string
  anulado: boolean
  liberado: boolean
  fechaLiberacion: string | null
  /** Res condenada entera: se pesa y se libera, pero no entra a camara. */
  decomisoTotal: boolean
  motivoDecomisoNombre: string | null
  pesoTotal: number
  piezas: PiezaEvaluacionItem[]
}

export interface CamaraOpcion {
  id: string
  nombre: string
}

export interface RomaneosEvaluacionResponse {
  listaMatanzaId: string
  numeroLista: number
  fecha: string
  especieId: string
  estadoListaMatanzaId: string
  establecimientoNombre: string
  totalRomaneos: number
  totalPiezas: number
  totalKg: number
  piezasLiberadas: number
  totalDecomisosTotales: number
  totalKgDecomisados: number
  data: RomaneoEvaluacionItem[]
  camaras: CamaraOpcion[]
}

export interface ResumenExistenciaItem {
  almacenId: string
  almacenNombre: string
  materialId: string
  materialCodigo: string
  materialNombre: string
  cantidad: number
  peso: number
}

export interface ProblemaItem {
  piezaId: string
  numeroRomaneo: number
  numeroGarron: number
  letra: string | null
  motivo: string
}

export interface PrevisualizacionLiberacion {
  listaMatanzaId: string
  numeroLista: number
  estadoListaMatanzaId: string
  jornadaFinalizada: boolean
  puedeLiberar: boolean
  motivoBloqueo: string | null
  piezasAProcesar: number
  piezasYaLiberadas: number
  kilosAIngresar: number
  /** Reses condenadas: se liberan sin generar existencia. */
  piezasDecomisadas: number
  kilosDecomisados: number
  resumen: ResumenExistenciaItem[]
  problemas: ProblemaItem[]
}

export interface LiberarJornadaResponse {
  movimientosGenerados: number
  piezasLiberadas: number
  romaneosLiberados: number
  piezasYaLiberadas: number
  kilosIngresados: number
  piezasDecomisadas: number
  kilosDecomisados: number
}

export interface ActualizarPiezaRequest {
  Peso: number
  TipificacionId: string
  AlmacenDestinoId: string
  ForzarFueraRango: boolean
}
