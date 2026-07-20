export interface UnidadFaena {
  id: string
  especieId: string
  especieNombre: string
  numero: number
  nombre: string
  cantidadCuartos: number
  piezasPorAnimal: number
  porDefecto: boolean
  codigoMaterial: string
  erP_Codigo: string
  activo: boolean
}

export interface CreateUnidadFaenaRequest {
  EspecieId: string
  Numero: number
  Nombre: string
  CantidadCuartos: number
  PiezasPorAnimal: number
  PorDefecto: boolean
  CodigoMaterial?: string
  ERP_Codigo?: string
}

export interface UpdateUnidadFaenaRequest {
  EspecieId: string
  Numero: number
  Nombre: string
  CantidadCuartos: number
  PiezasPorAnimal: number
  PorDefecto: boolean
  CodigoMaterial?: string
  ERP_Codigo?: string
  Activo: boolean
}
