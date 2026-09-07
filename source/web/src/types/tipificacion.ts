export interface Tipificacion {
  codigo: string
  descripcion: string
  especieId: string
  especieNombre: string
  tipoEspecieId: string
  tipoEspecieNombre: string
  unidadFaenaId: string
  unidadFaenaNombre: string
  destinoComercialId: string
  destinoComercialNombre: string
  tipificacionOficialId: string
  tipificacionOficialNombre: string
  pesoDesde: number
  pesoHasta: number
  unidadMedidaId: string
  unidadMedidaNombre?: string
  materialId?: string | null
  materialCodigo?: string
  materialNombre?: string
  puntos: number
  activo: boolean
}

export interface CreateTipificacionRequest {
  Codigo: string
  Descripcion: string
  EspecieId: string
  TipoEspecieId?: string
  UnidadFaenaId: string
  DestinoComercialId?: string
  TipificacionOficialId?: string
  PesoDesde: number
  PesoHasta: number
  UnidadMedidaId?: string
  MaterialId?: string
}

export interface UpdateTipificacionRequest {
  Descripcion: string
  EspecieId: string
  TipoEspecieId?: string
  UnidadFaenaId: string
  DestinoComercialId?: string
  TipificacionOficialId?: string
  PesoDesde: number
  PesoHasta: number
  UnidadMedidaId?: string
  MaterialId?: string
  Activo: boolean
}

export interface CatalogoFaenaOption {
  codigo: string
  nombre: string
  favorito?: boolean
}

export interface TipificacionOficialOption {
  codigo: string
  nombre: string
  especieId: string
}
