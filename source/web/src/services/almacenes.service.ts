import api from './axios-instance'

export interface AlmacenItem {
  id: string
  codigoAlmacen: string
  nombre: string
  capacidad: number
  tipoAlmacenId: string
  tipoAlmacenFamilia: string
  establecimientoId: string
  activo: boolean
}

export interface AlmacenDetail {
  id: string
  codigoAlmacen: string
  nombre: string
  capacidad: number
  tipoAlmacenId: string
  tipoAlmacenNombre: string
  tipoAlmacenFamilia: string
  establecimientoId: string
  establecimientoNombre: string
  activo: boolean
  erP_Codigo: string
}

export interface CreateAlmacenRequest {
  CodigoAlmacen: string
  Nombre: string
  Capacidad: number
  TipoAlmacenId: string
  ERP_Codigo?: string
  EstablecimientoId: string
}

export interface UpdateAlmacenRequest {
  Nombre: string
  Capacidad: number
  TipoAlmacenId: string
  ERP_Codigo?: string
  Activo: boolean
}

export interface TipoAlmacenOption {
  codigo: string
  nombre: string
  familia: string
}

/** Familias de tipo de almacen. */
export const FamiliaAlmacen = {
  Corral: 'CORRAL',
  Camara: 'CAMARA',
} as const

interface AlmacenesResponse {
  data: AlmacenItem[]
}

interface GetAlmacenesParams {
  EstablecimientoId?: string
  Estado?: boolean
  Familia?: string
}

export async function getAlmacenes(params?: GetAlmacenesParams): Promise<AlmacenItem[]> {
  const response = await api.get<AlmacenesResponse>('/Almacenes', { params })
  return response.data.data || []
}

export async function getAlmacen(id: string): Promise<AlmacenDetail> {
  const response = await api.get<AlmacenDetail>(`/Almacenes/${id}`)
  return response.data
}

export async function createAlmacen(data: CreateAlmacenRequest): Promise<{ id: string }> {
  const response = await api.post<{ id: string }>('/Almacenes', data)
  return response.data
}

export async function updateAlmacen(id: string, data: UpdateAlmacenRequest): Promise<void> {
  await api.put(`/Almacenes/${id}`, data)
}

export async function deleteAlmacen(id: string): Promise<void> {
  await api.delete(`/Almacenes/${id}`)
}

export async function getTiposAlmacenes(): Promise<TipoAlmacenOption[]> {
  const response = await api.get<{ data: TipoAlmacenOption[] }>('/TiposAlmacenes')
  return response.data.data || []
}
