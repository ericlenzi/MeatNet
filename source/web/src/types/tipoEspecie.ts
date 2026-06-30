export interface TipoEspecie {
  id: string
  nombre: string
  especieId: string
  especieNombre: string
  tipoSexoId: string
  tipoSexoNombre: string
  codigoMaterial: string
  erP_Codigo: string
  pesoTeorico: number
  activo: boolean
}

export interface CreateTipoEspecieRequest {
  Id: string
  Nombre: string
  EspecieId: string
  TipoSexoId?: string
  CodigoMaterial?: string
  ERP_Codigo?: string
  PesoTeorico?: number
}

export interface UpdateTipoEspecieRequest {
  Nombre: string
  EspecieId: string
  TipoSexoId?: string
  CodigoMaterial?: string
  ERP_Codigo?: string
  PesoTeorico?: number
  Activo: boolean
}

export interface TipoSexo {
  codigo: string
  nombre: string
  activo: boolean
}
