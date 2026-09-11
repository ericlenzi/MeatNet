import api from './axios-instance'
import type {
  PaginatedResponse,
  PaginatedRequest,
  Tipificador,
  CreateTipificadorRequest,
  UpdateTipificadorRequest,
} from '@/types'

interface GetTipificadoresParams extends PaginatedRequest {
  EstablecimientoId?: string
  EspecieId?: string
  Estado?: boolean
}

export async function getTipificadores(
  params?: GetTipificadoresParams,
): Promise<PaginatedResponse<Tipificador>> {
  const response = await api.get<PaginatedResponse<Tipificador>>('/Tipificadores', { params })
  return response.data
}

export async function getTipificador(id: string): Promise<Tipificador> {
  const response = await api.get<Tipificador>(`/Tipificadores/${id}`)
  return response.data
}

export async function createTipificador(data: CreateTipificadorRequest): Promise<{ id: string }> {
  const response = await api.post<{ id: string }>('/Tipificadores', data)
  return response.data
}

export async function updateTipificador(id: string, data: UpdateTipificadorRequest): Promise<void> {
  await api.put(`/Tipificadores/${id}`, data)
}

export async function deleteTipificador(id: string): Promise<void> {
  await api.delete(`/Tipificadores/${id}`)
}
