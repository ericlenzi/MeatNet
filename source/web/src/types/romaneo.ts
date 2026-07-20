// --- Renglones de la LM en ejecucion (modo hibrido del Tipificador) ---
export interface RenglonEjecucionItem {
  renglonId: string
  tropaId: string
  numeroTropa: number
  almacenId: string
  almacenNombre: string
  tipoEspecieId: string
  tipoEspecieNombre: string
  secuencia: number
  cantidad: number
  cantidadFaenada: number
  pendiente: number
}

export interface RenglonesEjecucion {
  listaMatanzaId: string
  numeroLista: number
  especieId: string
  especieNombre: string
  estadoListaMatanzaId: string
  proximoGarron: number
  renglonSugeridoId: string | null
  renglones: RenglonEjecucionItem[]
}

// --- Sugerencia de tipificacion ---
export interface TipificacionCandidata {
  codigo: string
  descripcion: string
  destinoComercialId: string
  destinoComercialNombre: string | null
  pesoDesde: number
  pesoHasta: number
  puntos: number
}

export interface SugerenciaTipificacion {
  propuestaCodigo: string | null
  candidatas: TipificacionCandidata[]
}

// --- Romaneos de la jornada (grilla) ---
export interface RomaneoPiezaItem {
  letra: string | null
  peso: number
  tipificacionId: string
  tipificacionDescripcion: string | null
}

export interface RomaneoJornadaItem {
  id: string
  numeroRomaneo: number
  numeroGarron: number
  numeroTropa: number
  tipoEspecieNombre: string
  unidadFaenaNombre: string
  anulado: boolean
  fecha: string
  pesoTotal: number
  piezas: RomaneoPiezaItem[]
}

// --- Monitor de faena (read-only) ---
export interface RenglonMonitorItem {
  secuencia: number
  numeroTropa: number
  almacenNombre: string
  tipoEspecieNombre: string
  cantidad: number
  cantidadFaenada: number
  pendiente: number
}

export interface MonitorFaena {
  listaMatanzaId: string
  numeroLista: number
  especieNombre: string
  estadoListaMatanzaId: string
  totalPlanificado: number
  totalFaenado: number
  totalPendiente: number
  animalesRomaneados: number
  kgTotales: number
  ritmoPorHora: number
  porRenglon: RenglonMonitorItem[]
}

// --- Requests (PascalCase: matchean el backend) ---
export interface PiezaRomaneoInput {
  TipificacionId: string
  Peso: number
}

export interface CrearRomaneoRequest {
  ListaMatanzaId: string
  ListaMatanzaDetalleId: string
  UnidadFaenaId: string
  NumeroGarron: number
  Piezas: PiezaRomaneoInput[]
}
