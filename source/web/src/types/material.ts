export interface Material {
  id: string
  codigoMaterial: string
  nombre: string
  tipoMaterialId: string
  tipoMaterialNombre: string
  unidadMedidaId: string
  unidadMedidaNombre: string
  pesoTeorico: string
  erP_Codigo: string
  activo: boolean
}

export interface CreateMaterialRequest {
  CodigoMaterial: string
  Nombre: string
  TipoMaterialId?: string
  UnidadMedidaId?: string
  PesoTeorico?: string
  ERP_Codigo?: string
}

export interface UpdateMaterialRequest {
  Nombre: string
  TipoMaterialId?: string
  UnidadMedidaId?: string
  PesoTeorico?: string
  ERP_Codigo?: string
  Activo: boolean
}

export interface TipoMaterial {
  codigo: string
  nombre: string
}
