export interface Cliente {
  id: string
  codigoCliente: string
  nombre: string
  tipoClienteId: string
  tipoClienteNombre: string
  numeroCuit: string
  numeroIngresosBrutos: string
  erP_Codigo: string
  activo: boolean
}

export interface CreateClienteRequest {
  CodigoCliente: string
  Nombre: string
  TipoClienteId: string
  NumeroCuit?: string
  NumeroIngresosBrutos?: string
  ERP_Codigo?: string
}

export interface UpdateClienteRequest {
  Nombre: string
  TipoClienteId: string
  NumeroCuit?: string
  NumeroIngresosBrutos?: string
  ERP_Codigo?: string
  Activo: boolean
}

export interface TipoCliente {
  codigo: string
  nombre: string
  activo: boolean
}
