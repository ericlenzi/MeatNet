import api from './axios-instance'
import type {
  PaginatedResponse,
  PaginatedRequest,
  Categoria,
  CreateCategoriaRequest,
  UpdateCategoriaRequest,
  TipoSexo,
} from '@/types'

interface GetCategoriasParams extends PaginatedRequest {
  Estado?: boolean
  EspecieId?: string
}

export async function getCategorias(
  params?: GetCategoriasParams,
): Promise<PaginatedResponse<Categoria>> {
  const response = await api.get<PaginatedResponse<Categoria>>('/Categorias', { params })
  return response.data
}

export async function getCategoria(id: string): Promise<Categoria> {
  const response = await api.get<Categoria>(`/Categorias/${id}`)
  return response.data
}

export async function createCategoria(
  data: CreateCategoriaRequest,
): Promise<{ id: string }> {
  const response = await api.post<{ id: string }>('/Categorias', data)
  return response.data
}

export async function updateCategoria(
  id: string,
  data: UpdateCategoriaRequest,
): Promise<void> {
  await api.put(`/Categorias/${id}`, data)
}

export async function deleteCategoria(id: string): Promise<void> {
  await api.delete(`/Categorias/${id}`)
}

export async function getTiposSexos(): Promise<TipoSexo[]> {
  const response = await api.get<{ data: TipoSexo[] }>('/Enums/tiposSexos')
  return response.data.data || []
}
