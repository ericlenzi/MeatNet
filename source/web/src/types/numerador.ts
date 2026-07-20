export interface Numerador {
  id: string
  establecimientoId: string
  establecimientoNombre: string
  especieCodigo: string
  especieNombre: string
  codigo: string
  descripcion: string
  tipoNumerador: string
  ultimoNumero: number
  activo: boolean
}

export interface CreateNumeradorRequest {
  EstablecimientoId: string
  EspecieCodigo: string
  Codigo: string
  Descripcion?: string
  TipoNumerador: string
  UltimoNumero: number
}

export interface UpdateNumeradorRequest {
  Descripcion?: string
  TipoNumerador: string
  UltimoNumero: number
  Activo: boolean
}
