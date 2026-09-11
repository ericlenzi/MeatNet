export interface Puesto {
  id: string
  codigoPuesto: string
  nombre: string
  establecimientoId: string
  establecimientoNombre: string
  especieId: string
  especieNombre: string | null
  tipoPuestoId: string
  tipoPuestoNombre: string | null
  tipoMedicionId: string
  tipoMedicionNombre: string | null
  activo: boolean
}

export interface CreatePuestoRequest {
  CodigoPuesto: string
  Nombre: string
  EstablecimientoId: string
  EspecieId: string
  TipoPuestoId: string
  TipoMedicionId: string
}

export interface UpdatePuestoRequest {
  Nombre: string
  EstablecimientoId: string
  EspecieId: string
  TipoPuestoId: string
  TipoMedicionId: string
  Activo: boolean
}
