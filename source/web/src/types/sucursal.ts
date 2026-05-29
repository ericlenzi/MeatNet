export interface Sucursal {
  id: string
  codigoSucursal: string
  nombre: string
  direccion: string
  erp_Codigo: string
  empresaId: string
  activo: boolean
  codigoPostal: string
  localidad: string
  provincia: string
  zona: string
  pais: string
}

export interface CreateSucursalRequest {
  NumeroSucursal: string
  Nombre: string
  Direccion?: string
}

export interface UpdateSucursalRequest {
  Nombre: string
  EmpresaId: string
  Activa: boolean
}
