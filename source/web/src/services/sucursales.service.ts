import api from './axios-instance'
import type {
  PaginatedResponse,
  PaginatedRequest,
  Sucursal,
  CreateSucursalRequest,
  UpdateSucursalRequest,
} from '@/types'

interface GetSucursalesParams extends PaginatedRequest {
  Estado?: boolean
}

export async function getSucursales(
  params?: GetSucursalesParams,
): Promise<PaginatedResponse<Sucursal>> {
  const response = await api.get<PaginatedResponse<Sucursal>>('/Sucursales', {
    params,
  })
  return response.data
}

export async function getSucursal(id: string): Promise<Sucursal> {
  const response = await api.get<Sucursal>(`/Sucursales/${id}`)
  return response.data
}

export async function createSucursal(
  data: CreateSucursalRequest,
): Promise<{ id: string }> {
  const response = await api.post<{ id: string }>('/Sucursales', data)
  return response.data
}

export async function updateSucursal(
  id: string,
  data: UpdateSucursalRequest,
): Promise<void> {
  await api.put(`/Sucursales/${id}`, data)
}

export async function deleteSucursal(id: string): Promise<void> {
  await api.delete(`/Sucursales/${id}`)
}
