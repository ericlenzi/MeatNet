import api from './axios-instance'
import type { PaginatedResponse, Rol } from '@/types'

export async function getRoles(): Promise<PaginatedResponse<Rol>> {
  const response = await api.get<PaginatedResponse<Rol>>('/Roles')
  return response.data
}
