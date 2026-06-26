export interface TipoEspecie {
  id: string
  nombre: string
  especieId: string
  especieNombre: string
  tipoSexoId: string
  tipoSexoNombre: string
  codigoMaterialDesde: string
  codigoMaterialHasta: string
  activo: boolean
}

export interface CreateTipoEspecieRequest {
  Id: string
  Nombre: string
  EspecieId: string
  TipoSexoId?: string
  CodigoMaterialDesde?: string
  CodigoMaterialHasta?: string
}

export interface UpdateTipoEspecieRequest {
  Nombre: string
  EspecieId: string
  TipoSexoId?: string
  CodigoMaterialDesde?: string
  CodigoMaterialHasta?: string
  Activo: boolean
}

export interface TipoSexo {
  codigo: string
  nombre: string
  activo: boolean
}
