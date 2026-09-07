import api from './axios-instance'
import type {
  PaginatedResponse,
  PaginatedRequest,
  Material,
  CreateMaterialRequest,
  UpdateMaterialRequest,
  TipoMaterial,
} from '@/types'

interface GetMaterialesParams extends PaginatedRequest {
  Estado?: boolean
  TipoMaterialId?: string
}

export async function getMateriales(
  params?: GetMaterialesParams,
): Promise<PaginatedResponse<Material>> {
  const response = await api.get<PaginatedResponse<Material>>('/Materiales', { params })
  return response.data
}

export async function getMaterial(id: string): Promise<Material> {
  const response = await api.get<Material>(`/Materiales/${id}`)
  return response.data
}

export async function createMaterial(
  data: CreateMaterialRequest,
): Promise<{ id: string }> {
  const response = await api.post<{ id: string }>('/Materiales', data)
  return response.data
}

export async function updateMaterial(id: string, data: UpdateMaterialRequest): Promise<void> {
  await api.put(`/Materiales/${id}`, data)
}

export async function deleteMaterial(id: string): Promise<void> {
  await api.delete(`/Materiales/${id}`)
}

/** Catalogo de tipos de material (solo lectura, para combos). */
export async function getTiposMateriales(): Promise<TipoMaterial[]> {
  const response = await api.get<{ data: TipoMaterial[] }>('/TiposMateriales')
  return response.data.data || []
}
