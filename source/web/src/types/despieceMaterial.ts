export interface DespieceMaterial {
  id: string
  materialOrigenId: string
  materialOrigenCodigo: string
  materialOrigenNombre: string
  materialDestinoId: string
  materialDestinoCodigo: string
  materialDestinoNombre: string
  cantidad: number
  rendimiento: number
  activo: boolean
}

export interface CreateDespieceMaterialRequest {
  MaterialOrigenId: string
  MaterialDestinoId: string
  Cantidad: number
  Rendimiento: number
}

export interface UpdateDespieceMaterialRequest {
  MaterialOrigenId: string
  MaterialDestinoId: string
  Cantidad: number
  Rendimiento: number
  Activo: boolean
}
