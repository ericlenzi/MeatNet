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
  color: string
}

export interface CreateSucursalRequest {
  CodigoSucursal: string
  Nombre: string
  EmpresaId: string
  Direccion?: string
  CodigoPostal?: string
  Localidad?: string
  Provincia?: string
  Zona?: string
  Pais?: string
  Erp_Codigo?: string
  Color?: string
}

export interface UpdateSucursalRequest {
  Nombre: string
  EmpresaId: string
  Activa: boolean
  Direccion?: string
  CodigoPostal?: string
  Localidad?: string
  Provincia?: string
  Zona?: string
  Pais?: string
  Erp_Codigo?: string
  Color?: string
}
