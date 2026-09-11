import api from './axios-instance'
import type {
  PaginatedResponse,
  PaginatedRequest,
  RendimientoSubproducto,
  CreateRendimientoSubproductoRequest,
  UpdateRendimientoSubproductoRequest,
} from '@/types'

interface GetRendimientosParams extends PaginatedRequest {
  EspecieId?: string
  Estado?: boolean
}

export async function getRendimientosSubproductos(
  params?: GetRendimientosParams,
): Promise<PaginatedResponse<RendimientoSubproducto>> {
  const response = await api.get<PaginatedResponse<RendimientoSubproducto>>(
    '/RendimientosSubproductos',
    { params },
  )
  return response.data
}

export async function getRendimientoSubproducto(id: string): Promise<RendimientoSubproducto> {
  const response = await api.get<RendimientoSubproducto>(`/RendimientosSubproductos/${id}`)
  return response.data
}

export async function createRendimientoSubproducto(
  data: CreateRendimientoSubproductoRequest,
): Promise<{ id: string }> {
  const response = await api.post<{ id: string }>('/RendimientosSubproductos', data)
  return response.data
}

export async function updateRendimientoSubproducto(
  id: string,
  data: UpdateRendimientoSubproductoRequest,
): Promise<void> {
  await api.put(`/RendimientosSubproductos/${id}`, data)
}

export async function deleteRendimientoSubproducto(id: string): Promise<void> {
  await api.delete(`/RendimientosSubproductos/${id}`)
}
