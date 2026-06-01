import api from './axios-instance'
import type {
  PaginatedResponse,
  PaginatedRequest,
  Usuario,
  CreateUsuarioRequest,
  UpdateUsuarioRequest,
} from '@/types'

interface GetUsuariosParams extends PaginatedRequest {
  Rol?: string
  Estado?: number
}

export async function getUsuarios(
  params?: GetUsuariosParams,
): Promise<PaginatedResponse<Usuario>> {
  const response = await api.get<PaginatedResponse<Usuario>>('/Usuarios', {
    params,
  })
  return response.data
}

export async function getUsuario(id: string): Promise<Usuario> {
  const response = await api.get<Usuario>(`/Usuarios/${id}`)
  return response.data
}

export async function createUsuario(
  data: CreateUsuarioRequest,
): Promise<{ id: string }> {
  const response = await api.post<{ id: string }>('/Usuarios', data)
  return response.data
}

export async function updateUsuario(
  id: string,
  data: UpdateUsuarioRequest,
): Promise<void> {
  await api.put(`/Usuarios/${id}`, data)
}

export async function deleteUsuario(id: string): Promise<void> {
  await api.delete(`/Usuarios/${id}`)
}

export interface UsuarioSucursalItem {
  id: string
  sucursalId: string
  codigoSucursal: string
  nombre: string
  color: string
  esMain: boolean
}

interface UsuarioSucursalesResponse {
  data: UsuarioSucursalItem[]
}

export async function getUsuarioSucursales(
  usuarioId: string,
): Promise<UsuarioSucursalItem[]> {
  const response = await api.get<UsuarioSucursalesResponse>(
    `/Usuarios/${usuarioId}/Sucursales`,
  )
  return response.data.data || []
}

export async function addUsuarioSucursal(
  usuarioId: string,
  sucursalId: string,
  esMain: boolean,
): Promise<{ id: string }> {
  const response = await api.post<{ id: string }>(
    `/Usuarios/${usuarioId}/Sucursales`,
    { SucursalId: sucursalId, EsMain: esMain },
  )
  return response.data
}

export async function removeUsuarioSucursal(
  usuarioId: string,
  usuarioSucursalId: string,
): Promise<void> {
  await api.delete(`/Usuarios/${usuarioId}/Sucursales/${usuarioSucursalId}`)
}
