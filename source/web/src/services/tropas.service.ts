import api from './axios-instance'
import type { TropaDisponibleItem } from '@/types'

interface GetTropasDisponiblesParams {
  EstablecimientoId?: string
}

interface TropasDisponiblesResponse {
  data: TropaDisponibleItem[]
}

export async function getTropasDisponibles(
  params?: GetTropasDisponiblesParams,
): Promise<TropaDisponibleItem[]> {
  const response = await api.get<TropasDisponiblesResponse>('/Tropas', { params })
  return response.data.data || []
}
