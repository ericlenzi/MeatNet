export interface UnidadFaena {
  codigo: string
  especieId: string
  especieNombre: string
  nombre: string
  cantidadCuartos: number
  piezasPorAnimal: number
  porDefecto: boolean
  codigoMaterial: string
  erP_Codigo: string
  activo: boolean
}

export interface CreateUnidadFaenaRequest {
  Codigo: string
  EspecieId: string
  Nombre: string
  CantidadCuartos: number
  PiezasPorAnimal: number
  PorDefecto: boolean
  CodigoMaterial?: string
  ERP_Codigo?: string
}

export interface UpdateUnidadFaenaRequest {
  EspecieId: string
  Nombre: string
  CantidadCuartos: number
  PiezasPorAnimal: number
  PorDefecto: boolean
  CodigoMaterial?: string
  ERP_Codigo?: string
  Activo: boolean
}
