import api from './axios-instance'
import type {
  PaginatedResponse,
  PaginatedRequest,
  TipoEspecie,
  CreateTipoEspecieRequest,
  UpdateTipoEspecieRequest,
  TipoSexo,
} from '@/types'

interface GetTiposEspeciesParams extends PaginatedRequest {
  Estado?: boolean
  EspecieId?: string
}

export async function getTiposEspecies(
  params?: GetTiposEspeciesParams,
): Promise<PaginatedResponse<TipoEspecie>> {
  const response = await api.get<PaginatedResponse<TipoEspecie>>('/TiposEspecies', { params })
  return response.data
}

export async function getTipoEspecie(id: string): Promise<TipoEspecie> {
  const response = await api.get<TipoEspecie>(`/TiposEspecies/${id}`)
  return response.data
}

export async function createTipoEspecie(
  data: CreateTipoEspecieRequest,
): Promise<{ id: string }> {
  const response = await api.post<{ id: string }>('/TiposEspecies', data)
  return response.data
}

export async function updateTipoEspecie(
  id: string,
  data: UpdateTipoEspecieRequest,
): Promise<void> {
  await api.put(`/TiposEspecies/${id}`, data)
}

export async function deleteTipoEspecie(id: string): Promise<void> {
  await api.delete(`/TiposEspecies/${id}`)
}

export async function getTiposSexos(): Promise<TipoSexo[]> {
  const response = await api.get<{ data: TipoSexo[] }>('/Enums/tiposSexos')
  return response.data.data || []
}
