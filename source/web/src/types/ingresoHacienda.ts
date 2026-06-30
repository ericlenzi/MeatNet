// --- Listado ---
export interface IngresoHaciendaListItem {
  id: string
  numeroIngreso: number
  fechaHoraIngreso: string
  numeroDte: string
  establecimientoId: string
  establecimientoNombre: string
  clienteNombre: string
  estadoIngresoId: string
  estadoIngresoNombre: string
  totalCabezas: number
  pesoNeto: number
}

// --- Detalle ---
export interface IngresoHaciendaPesada {
  id: string
  tipoEspecieId: string
  tipoEspecieNombre: string
  pesoIngreso: number
  unidadMedida: string
}

export interface IngresoHaciendaUbicacion {
  id: string
  tropaId: string | null
  tipoEspecieId: string
  tipoEspecieNombre: string
  almacenId: string
  almacenNombre: string
  cantidad: number
  pesoPromedio: number
  estadoHaciendaId: string
  estadoHaciendaNombre: string
}

export interface IngresoHaciendaTropa {
  id: string
  numeroTropa: number
  especieCodigo: string
  estadoTropaId: string
}

export interface IngresoHacienda {
  id: string
  numeroIngreso: number
  establecimientoId: string
  establecimientoNombre: string
  fechaHoraIngreso: string
  numeroDte: string
  fechaEmisionDte: string
  clienteId: string
  clienteNombre: string
  clienteEstablecimientoId: string
  codigoRenspa: string
  numeroCUIG: string
  provinciaId: number
  provinciaNombre: string
  localidad: string
  origenHaciendaId: string
  origenHaciendaNombre: string
  usoHaciendaId: string
  usoHaciendaNombre: string
  transportista: string
  chofer: string
  patenteCamion: string
  patenteJaula: string
  pesoBruto: number
  tara: number
  pesoNeto: number
  estadoIngresoId: string
  estadoIngresoNombre: string
  fechaAprobacion: string | null
  pesadas: IngresoHaciendaPesada[]
  ubicaciones: IngresoHaciendaUbicacion[]
  tropas: IngresoHaciendaTropa[]
}

// --- Requests (PascalCase: matchean el backend) ---
export interface PesadaInput {
  TipoEspecieId: string
  PesoIngreso: number
}

export interface UbicacionInput {
  TipoEspecieId: string
  AlmacenId: string
  Cantidad: number
  EstadoHaciendaId: string
}

export interface CreateIngresoHaciendaRequest {
  EstablecimientoId: string
  FechaHoraIngreso: string
  NumeroDte: string
  FechaEmisionDte: string
  ClienteId: string
  ClienteEstablecimientoId: string
  ProvinciaId: number
  Localidad: string
  OrigenHaciendaId: string | null
  UsoHaciendaId: string | null
  Transportista: string
  Chofer: string
  PatenteCamion: string
  PatenteJaula: string
  PesoBruto: number
  Tara: number
  Pesadas: PesadaInput[]
  Ubicaciones: UbicacionInput[]
}

export type UpdateIngresoHaciendaRequest = Omit<CreateIngresoHaciendaRequest, 'EstablecimientoId'>

export interface TropaGenerada {
  id: string
  especieCodigo: string
  numeroTropa: number
}

export interface AprobarIngresoHaciendaResult {
  tropas: TropaGenerada[]
  advertencia: string | null
}

// --- Existencia de Hacienda ---
export interface ExistenciaCorralItem {
  almacenId: string
  almacenNombre: string
  capacidadCorral: number
  tipoEspecieId: string
  tipoEspecieNombre: string
  clienteId: string
  clienteNombre: string
  tropaId: string
  numeroTropa: number
  cantidadUN: number
  pesoKG: number
}

// --- Tropas disponibles ---
export interface TropaDisponibleItem {
  id: string
  numeroTropa: number
  especieCodigo: string
  especieNombre: string
  clienteNombre: string
  ingresoHaciendaId: string
  numeroIngreso: number
  establecimientoId: string
  establecimientoNombre: string
  cabezasEnPie: number
  pesoKGEnPie: number
}

// --- Estados (codigos del catalogo) ---
export const EstadoIngreso = {
  Borrador: 'BORRADOR',
  PendienteAprobacion: 'PENDIENTE',
  Aprobado: 'APROBADO',
  Anulado: 'ANULADO',
} as const

export const EstadoHacienda = {
  EnPie: 'EN_PIE',
  Caidos: 'CAIDOS',
  Muertos: 'MUERTOS',
} as const
