import api from './axios-instance'
import type {
  PaginatedResponse,
  PaginatedRequest,
  DespieceMaterial,
  CreateDespieceMaterialRequest,
  UpdateDespieceMaterialRequest,
} from '@/types'

interface GetDespiecesMaterialesParams extends PaginatedRequest {
  Estado?: boolean
  MaterialOrigenId?: string
}

export async function getDespiecesMateriales(
  params?: GetDespiecesMaterialesParams,
): Promise<PaginatedResponse<DespieceMaterial>> {
  const response = await api.get<PaginatedResponse<DespieceMaterial>>('/DespiecesMateriales', { params })
  return response.data
}

export async function getDespieceMaterial(id: string): Promise<DespieceMaterial> {
  const response = await api.get<DespieceMaterial>(`/DespiecesMateriales/${id}`)
  return response.data
}

export async function createDespieceMaterial(
  data: CreateDespieceMaterialRequest,
): Promise<{ id: string }> {
  const response = await api.post<{ id: string }>('/DespiecesMateriales', data)
  return response.data
}

export async function updateDespieceMaterial(
  id: string,
  data: UpdateDespieceMaterialRequest,
): Promise<void> {
  await api.put(`/DespiecesMateriales/${id}`, data)
}

export async function deleteDespieceMaterial(id: string): Promise<void> {
  await api.delete(`/DespiecesMateriales/${id}`)
}
