import api from './axios-instance'
import type {
  PaginatedResponse,
  PaginatedRequest,
  Cliente,
  CreateClienteRequest,
  UpdateClienteRequest,
} from '@/types'

interface GetClientesParams extends PaginatedRequest {
  Estado?: boolean
}

export async function getClientes(
  params?: GetClientesParams,
): Promise<PaginatedResponse<Cliente>> {
  const response = await api.get<PaginatedResponse<Cliente>>('/Clientes', { params })
  return response.data
}

export async function getCliente(id: string): Promise<Cliente> {
  const response = await api.get<Cliente>(`/Clientes/${id}`)
  return response.data
}

export async function createCliente(
  data: CreateClienteRequest,
): Promise<{ id: string }> {
  const response = await api.post<{ id: string }>('/Clientes', data)
  return response.data
}

export async function updateCliente(
  id: string,
  data: UpdateClienteRequest,
): Promise<void> {
  await api.put(`/Clientes/${id}`, data)
}

export async function deleteCliente(id: string): Promise<void> {
  await api.delete(`/Clientes/${id}`)
}
