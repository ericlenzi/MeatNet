export interface UnidadFaena {
  id: string
  especieId: string
  especieNombre: string
  numero: number
  nombre: string
  cantidadCuartos: number
  unidadComplementaria: number
  codigoMaterial: string
  erP_Codigo: string
  activo: boolean
}

export interface CreateUnidadFaenaRequest {
  EspecieId: string
  Numero: number
  Nombre: string
  CantidadCuartos: number
  UnidadComplementaria: number
  CodigoMaterial?: string
  ERP_Codigo?: string
}

export interface UpdateUnidadFaenaRequest {
  EspecieId: string
  Numero: number
  Nombre: string
  CantidadCuartos: number
  UnidadComplementaria: number
  CodigoMaterial?: string
  ERP_Codigo?: string
  Activo: boolean
}
