import api from './axios-instance'
import type {
  PaginatedResponse,
  PaginatedRequest,
  Establecimiento,
  CreateEstablecimientoRequest,
  UpdateEstablecimientoRequest,
} from '@/types'

export async function getEstablecimientos(
  params?: PaginatedRequest,
): Promise<PaginatedResponse<Establecimiento>> {
  const response = await api.get<PaginatedResponse<Establecimiento>>('/Establecimientos', { params })
  return response.data
}

export async function getEstablecimiento(id: string): Promise<Establecimiento> {
  const response = await api.get<Establecimiento>(`/Establecimientos/${id}`)
  return response.data
}

export async function createEstablecimiento(
  data: CreateEstablecimientoRequest,
): Promise<{ id: string }> {
  const response = await api.post<{ id: string }>('/Establecimientos', data)
  return response.data
}

export async function updateEstablecimiento(
  id: string,
  data: UpdateEstablecimientoRequest,
): Promise<void> {
  await api.put(`/Establecimientos/${id}`, data)
}

export async function deleteEstablecimiento(id: string): Promise<void> {
  await api.delete(`/Establecimientos/${id}`)
}
