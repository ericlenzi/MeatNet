import api from './axios-instance'
import type { Numerador, CreateNumeradorRequest, UpdateNumeradorRequest } from '@/types'

interface GetNumeradoresParams {
  EstablecimientoId?: string
  Estado?: boolean
}

export async function getNumeradores(params?: GetNumeradoresParams): Promise<Numerador[]> {
  const response = await api.get<{ data: Numerador[] }>('/Numeradores', { params })
  return response.data.data || []
}

export async function getNumerador(id: string): Promise<Numerador> {
  const response = await api.get<Numerador>(`/Numeradores/${id}`)
  return response.data
}

export async function createNumerador(data: CreateNumeradorRequest): Promise<{ id: string }> {
  const response = await api.post<{ id: string }>('/Numeradores', data)
  return response.data
}

export async function updateNumerador(id: string, data: UpdateNumeradorRequest): Promise<void> {
  await api.put(`/Numeradores/${id}`, data)
}

export async function deleteNumerador(id: string): Promise<void> {
  await api.delete(`/Numeradores/${id}`)
}
