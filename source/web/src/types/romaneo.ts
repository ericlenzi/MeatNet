// --- Renglones de la LM en ejecucion (modo hibrido del Tipificador) ---
export interface RenglonEjecucionItem {
  renglonId: string
  tropaId: string
  numeroTropa: number
  almacenId: string
  almacenNombre: string
  almacenDestinoId: string | null
  almacenDestinoNombre: string | null
  tipoEspecieId: string
  tipoEspecieNombre: string
  secuencia: number
  cantidad: number
  cantidadFaenada: number
  pendiente: number
}

export interface CamaraOption {
  id: string
  nombre: string
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
  camaras: CamaraOption[]
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
  almacenDestinoNombre: string | null
  tipificacionId: string
  tipificacionDescripcion: string | null
  pesoFueraRango: boolean
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
  listaMatanzaDetalleId: string
  secuencia: number
  numeroTropa: number
  almacenNombre: string
  almacenDestinoNombre: string | null
  tipoEspecieNombre: string
  cantidad: number
  cantidadFaenada: number
  pendiente: number
  romaneoDesde: number | null
  romaneoHasta: number | null
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
  AlmacenDestinoId: string
  TipificacionId: string
  Peso: number
  ForzarFueraRango: boolean
}

export interface CrearRomaneoRequest {
  ListaMatanzaId: string
  ListaMatanzaDetalleId: string
  UnidadFaenaId: string
  NumeroGarron: number
  Piezas: PiezaRomaneoInput[]
}
