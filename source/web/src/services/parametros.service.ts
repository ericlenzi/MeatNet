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

export async function getParametro(codigo: string): Promise<Parametro> {
  const response = await api.get<Parametro>(`/Parametros/${codigo}`)
  return response.data
}

export async function createParametro(
  data: CreateParametroRequest,
): Promise<{ codigo: string }> {
  const response = await api.post<{ codigo: string }>('/Parametros', data)
  return response.data
}

export async function updateParametro(
  codigo: string,
  data: UpdateParametroRequest,
): Promise<void> {
  await api.put(`/Parametros/${codigo}`, data)
}

export async function deleteParametro(codigo: string): Promise<void> {
  await api.delete(`/Parametros/${codigo}`)
}
