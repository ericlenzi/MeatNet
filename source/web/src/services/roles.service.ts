import api from './axios-instance'
import type { PaginatedResponse, Rol, CreateRolRequest, UpdateRolRequest } from '@/types'

export async function getRoles(): Promise<PaginatedResponse<Rol>> {
  const response = await api.get<PaginatedResponse<Rol>>('/Roles')
  return response.data
}

export async function getRol(codigo: string): Promise<Rol> {
  const response = await api.get<Rol>(`/Roles/${codigo}`)
  return response.data
}

export async function createRol(data: CreateRolRequest): Promise<{ codigo: string }> {
  const response = await api.post<{ codigo: string }>('/Roles', data)
  return response.data
}

export async function updateRol(codigo: string, data: UpdateRolRequest): Promise<void> {
  await api.put(`/Roles/${codigo}`, data)
}

export async function deleteRol(codigo: string): Promise<void> {
  await api.delete(`/Roles/${codigo}`)
}
