export interface Categoria {
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

export interface CreateCategoriaRequest {
  Id: string
  Nombre: string
  EspecieId: string
  TipoSexoId?: string
  CodigoMaterialDesde?: string
  CodigoMaterialHasta?: string
}

export interface UpdateCategoriaRequest {
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
