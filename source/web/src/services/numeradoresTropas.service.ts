import api from './axios-instance'
import type { NumeradorTropa, CreateNumeradorTropaRequest, UpdateNumeradorTropaRequest } from '@/types'

interface GetNumeradoresTropasResponse {
  data: NumeradorTropa[]
}

export async function getNumeradoresTropas(): Promise<NumeradorTropa[]> {
  const response = await api.get<GetNumeradoresTropasResponse>('/NumeradoresTropas')
  return response.data.data || []
}

export async function getNumeradorTropa(id: string): Promise<NumeradorTropa> {
  const response = await api.get<NumeradorTropa>(`/NumeradoresTropas/${id}`)
  return response.data
}

export async function createNumeradorTropa(
  data: CreateNumeradorTropaRequest,
): Promise<{ id: string }> {
  const response = await api.post<{ id: string }>('/NumeradoresTropas', data)
  return response.data
}

export async function updateNumeradorTropa(
  id: string,
  data: UpdateNumeradorTropaRequest,
): Promise<void> {
  await api.put(`/NumeradoresTropas/${id}`, data)
}

export async function deleteNumeradorTropa(id: string): Promise<void> {
  await api.delete(`/NumeradoresTropas/${id}`)
}
