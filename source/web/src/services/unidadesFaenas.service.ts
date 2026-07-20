import api from './axios-instance'
import type {
  PaginatedResponse,
  PaginatedRequest,
  UnidadFaena,
  CreateUnidadFaenaRequest,
  UpdateUnidadFaenaRequest,
} from '@/types'

interface GetUnidadesFaenasParams extends PaginatedRequest {
  Estado?: boolean
  EspecieId?: string
}

export async function getUnidadesFaenas(
  params?: GetUnidadesFaenasParams,
): Promise<PaginatedResponse<UnidadFaena>> {
  const response = await api.get<PaginatedResponse<UnidadFaena>>('/UnidadesFaenas', { params })
  return response.data
}

export async function getUnidadFaena(id: string): Promise<UnidadFaena> {
  const response = await api.get<UnidadFaena>(`/UnidadesFaenas/${id}`)
  return response.data
}

export async function createUnidadFaena(
  data: CreateUnidadFaenaRequest,
): Promise<{ id: string }> {
  const response = await api.post<{ id: string }>('/UnidadesFaenas', data)
  return response.data
}

export async function updateUnidadFaena(
  id: string,
  data: UpdateUnidadFaenaRequest,
): Promise<void> {
  await api.put(`/UnidadesFaenas/${id}`, data)
}

export async function deleteUnidadFaena(id: string): Promise<void> {
  await api.delete(`/UnidadesFaenas/${id}`)
}

/** Opciones para poblar combos (trae todas las activas, opcionalmente de una especie). */
export async function getUnidadesFaenasOptions(especieId?: string): Promise<UnidadFaena[]> {
  const response = await getUnidadesFaenas({ Estado: true, EspecieId: especieId, PageSize: 1000 })
  return response.data || []
}
