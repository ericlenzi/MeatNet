export interface Empresa {
  id: string
  codigoEmpresa: string
  nombre: string
  tipoEmpresaId: string
  numeroCuit: string
  numeroIngresosBrutos: string
  numeroInscripcionRuca: string
  codigoActividad: string
  erP_Codigo: string
  activo: boolean
}

export interface CreateEmpresaRequest {
  CodigoEmpresa: string
  Nombre: string
  TipoEmpresaId: string
  NumeroCuit?: string
  NumeroIngresosBrutos?: string
  NumeroInscripcionRuca?: string
  CodigoActividad?: string
  ERP_Codigo?: string
}

export interface UpdateEmpresaRequest {
  CodigoEmpresa: string
  Nombre: string
  TipoEmpresaId: string
  NumeroCuit?: string
  NumeroIngresosBrutos?: string
  NumeroInscripcionRuca?: string
  CodigoActividad?: string
  ERP_Codigo?: string
  Activo: boolean
}
