export interface TipoEmpresa {
  codigo: string
  nombre: string
  activo: boolean
}

export interface Empresa {
  /** Codigo de negocio de la empresa: es su clave primaria. */
  id: string
  nombre: string
  tipoEmpresaId: string
  numeroCuit: string
  numeroIngresosBrutos: string
  numeroInscripcionRuca: string
  codigoActividad: string
  erP_Codigo: string
  color: string | null
  logo: string | null
  activo: boolean
}

export interface CreateEmpresaRequest {
  Id: string
  Nombre: string
  TipoEmpresaId: string
  NumeroCuit?: string
  NumeroIngresosBrutos?: string
  NumeroInscripcionRuca?: string
  CodigoActividad?: string
  ERP_Codigo?: string
}

export interface UpdateEmpresaRequest {
  Nombre: string
  TipoEmpresaId: string
  NumeroCuit?: string
  NumeroIngresosBrutos?: string
  NumeroInscripcionRuca?: string
  CodigoActividad?: string
  ERP_Codigo?: string
  Activo: boolean
}
