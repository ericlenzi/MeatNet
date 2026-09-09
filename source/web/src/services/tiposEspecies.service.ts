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

// Catalogo global: la lectura es abierta (aparece en combos), la escritura es del SUPERADMIN.
// Para las categorias con las que opera la empresa, ver empresasTiposEspecies.service.
export async function getTiposEspecies(
  params?: GetTiposEspeciesParams,
): Promise<PaginatedResponse<TipoEspecie>> {
  const response = await api.get<PaginatedResponse<TipoEspecie>>('/TiposEspecies', { params })
  return response.data
}

export async function getTipoEspecie(codigo: string): Promise<TipoEspecie> {
  const response = await api.get<TipoEspecie>(`/TiposEspecies/${codigo}`)
  return response.data
}

export async function createTipoEspecie(
  data: CreateTipoEspecieRequest,
): Promise<{ codigo: string }> {
  const response = await api.post<{ codigo: string }>('/TiposEspecies', data)
  return response.data
}

export async function updateTipoEspecie(
  codigo: string,
  data: UpdateTipoEspecieRequest,
): Promise<void> {
  await api.put(`/TiposEspecies/${codigo}`, data)
}

export async function deleteTipoEspecie(codigo: string): Promise<void> {
  await api.delete(`/TiposEspecies/${codigo}`)
}

export async function getTiposSexos(): Promise<TipoSexo[]> {
  const response = await api.get<{ data: TipoSexo[] }>('/Enums/tiposSexos')
  return response.data.data || []
}
