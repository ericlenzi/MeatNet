import api from './axios-instance'
import type {
  PaginatedResponse,
  PaginatedRequest,
  Parametro,
  CreateParametroRequest,
  UpdateParametroRequest,
} from '@/types'

interface GetParametrosParams extends PaginatedRequest {
  Estado?: boolean
}

export async function getParametros(
  params?: GetParametrosParams,
): Promise<PaginatedResponse<Parametro>> {
  const response = await api.get<PaginatedResponse<Parametro>>('/Parametros', { params })
  return response.data
}

export async function getParametro(id: string): Promise<Parametro> {
  const response = await api.get<Parametro>(`/Parametros/${id}`)
  return response.data
}

export async function createParametro(
  data: CreateParametroRequest,
): Promise<{ id: string }> {
  const response = await api.post<{ id: string }>('/Parametros', data)
  return response.data
}

export async function updateParametro(
  id: string,
  data: UpdateParametroRequest,
): Promise<void> {
  await api.put(`/Parametros/${id}`, data)
}

export async function deleteParametro(id: string): Promise<void> {
  await api.delete(`/Parametros/${id}`)
}
