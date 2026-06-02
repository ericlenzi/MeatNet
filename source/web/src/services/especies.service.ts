import api from './axios-instance'
import type {
  PaginatedResponse,
  PaginatedRequest,
  Especie,
  CreateEspecieRequest,
  UpdateEspecieRequest,
} from '@/types'

interface GetEspeciesParams extends PaginatedRequest {
  Estado?: boolean
}

export async function getEspecies(
  params?: GetEspeciesParams,
): Promise<PaginatedResponse<Especie>> {
  const response = await api.get<PaginatedResponse<Especie>>('/Especies', { params })
  return response.data
}

export async function getEspecie(codigo: string): Promise<Especie> {
  const response = await api.get<Especie>(`/Especies/${codigo}`)
  return response.data
}

export async function createEspecie(
  data: CreateEspecieRequest,
): Promise<{ codigo: string }> {
  const response = await api.post<{ codigo: string }>('/Especies', data)
  return response.data
}

export async function updateEspecie(
  codigo: string,
  data: UpdateEspecieRequest,
): Promise<void> {
  await api.put(`/Especies/${codigo}`, data)
}

export async function deleteEspecie(codigo: string): Promise<void> {
  await api.delete(`/Especies/${codigo}`)
}
