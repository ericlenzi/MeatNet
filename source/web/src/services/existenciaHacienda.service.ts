import api from './axios-instance'
import type { ExistenciaCorralItem } from '@/types'

interface GetExistenciaHaciendaParams {
  EstablecimientoId?: string
}

interface ExistenciaHaciendaResponse {
  data: ExistenciaCorralItem[]
}

export async function getExistenciaHacienda(
  params?: GetExistenciaHaciendaParams,
): Promise<ExistenciaCorralItem[]> {
  const response = await api.get<ExistenciaHaciendaResponse>('/ExistenciaHacienda', { params })
  return response.data.data || []
}
