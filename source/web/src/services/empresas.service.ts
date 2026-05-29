import api from './axios-instance'
import type {
  PaginatedResponse,
  PaginatedRequest,
  Empresa,
  CreateEmpresaRequest,
  UpdateEmpresaRequest,
} from '@/types'

interface GetEmpresasParams extends PaginatedRequest {
  Estado?: boolean
}

export async function getEmpresas(
  params?: GetEmpresasParams,
): Promise<PaginatedResponse<Empresa>> {
  const response = await api.get<PaginatedResponse<Empresa>>('/Empresas', {
    params,
  })
  return response.data
}

export async function getEmpresa(id: string): Promise<Empresa> {
  const response = await api.get<Empresa>(`/Empresas/${id}`)
  return response.data
}

export async function createEmpresa(
  data: CreateEmpresaRequest,
): Promise<{ id: string }> {
  const response = await api.post<{ id: string }>('/Empresas', data)
  return response.data
}

export async function updateEmpresa(
  id: string,
  data: UpdateEmpresaRequest,
): Promise<void> {
  await api.put(`/Empresas/${id}`, data)
}

export async function deleteEmpresa(id: string): Promise<void> {
  await api.delete(`/Empresas/${id}`)
}
