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
  resumen: ResumenExistenciaItem[]
  problemas: ProblemaItem[]
}

export interface LiberarJornadaResponse {
  movimientosGenerados: number
  piezasLiberadas: number
  romaneosLiberados: number
  piezasYaLiberadas: number
  kilosIngresados: number
}

export interface ActualizarPiezaRequest {
  Peso: number
  TipificacionId: string
  AlmacenDestinoId: string
  ForzarFueraRango: boolean
}
