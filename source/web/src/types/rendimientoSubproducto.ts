export interface RendimientoSubproducto {
  id: string
  especieId: string
  especieNombre: string | null
  materialId: string
  materialCodigo: string
  materialNombre: string
  /** Porcentaje sobre los kg de res faenada. */
  porcentaje: number
  activo: boolean
}

export interface CreateRendimientoSubproductoRequest {
  EspecieId: string
  MaterialId: string
  Porcentaje: number
}

export interface UpdateRendimientoSubproductoRequest {
  EspecieId: string
  MaterialId: string
  Porcentaje: number
  Activo: boolean
}
