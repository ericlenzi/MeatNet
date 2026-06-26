export interface EspecieItem {
  id: string
  nombre: string
}

export interface Establecimiento {
  id: string
  codigoEstablecimiento: string
  nombre: string
  sucursalId: string
  sucursalNombre: string
  especies: EspecieItem[]
  empresaId: string
  empresaNombre: string
  numeroSenasa: string
  numeroRuca: string
  activo: boolean
}

export interface CreateEstablecimientoRequest {
  CodigoEstablecimiento: string
  Nombre: string
  SucursalId: string
  EspecieIds?: string[]
  NumeroSenasa?: string
  NumeroRuca?: string
}

export interface UpdateEstablecimientoRequest {
  Nombre: string
  SucursalId: string
  EspecieIds?: string[]
  NumeroSenasa?: string
  NumeroRuca?: string
  Activo: boolean
}
