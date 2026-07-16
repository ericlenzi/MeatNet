import api from './axios-instance'
import type { TrazabilidadTropa } from '@/types'

interface GetTrazabilidadTropaParams {
  NumeroTropa: number
  EstablecimientoId?: string
}

interface TrazabilidadTropaResponse {
  data: TrazabilidadTropa[]
}

export async function getTrazabilidadTropa(
  params: GetTrazabilidadTropaParams,
): Promise<TrazabilidadTropa[]> {
  const response = await api.get<TrazabilidadTropaResponse>('/Tropas/trazabilidad', { params })
  return response.data.data || []
}
