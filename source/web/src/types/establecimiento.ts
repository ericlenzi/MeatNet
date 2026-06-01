export interface Establecimiento {
  id: string
  codigoEstablecimiento: string
  nombre: string
  sucursalId: string
  sucursalNombre: string
  especieId: string
  especieNombre: string
  numeroSenasa: string
  numeroOncca: string
  activo: boolean
}

export interface CreateEstablecimientoRequest {
  CodigoEstablecimiento: string
  Nombre: string
  SucursalId: string
  EspecieId?: string
  NumeroSenasa?: string
  NumeroOncca?: string
}

export interface UpdateEstablecimientoRequest {
  Nombre: string
  SucursalId: string
  EspecieId?: string
  NumeroSenasa?: string
  NumeroOncca?: string
  Activo: boolean
}
