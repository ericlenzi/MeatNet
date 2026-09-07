import api from './axios-instance'
import type { ExistenciaCamaraItem, MovimientoCamaraItem } from '@/types/existenciaCamara'

interface GetExistenciaCamaraParams {
  EstablecimientoId?: string
  AlmacenId?: string
  MaterialId?: string
  IncluirSaldoCero?: boolean
}

interface ExistenciaCamaraResponse {
  data: ExistenciaCamaraItem[]
  totalCantidad: number
  totalPeso: number
}

export async function getExistenciaCamara(
  params?: GetExistenciaCamaraParams,
): Promise<ExistenciaCamaraResponse> {
  const response = await api.get<ExistenciaCamaraResponse>('/ExistenciaCamara', { params })
  return {
    data: response.data.data || [],
    totalCantidad: response.data.totalCantidad ?? 0,
    totalPeso: response.data.totalPeso ?? 0,
  }
}

interface GetMovimientosCamaraParams {
  AlmacenId?: string
  MaterialId?: string
  TropaId?: string
  RomaneoPiezaOrigenId?: string
}

export async function getMovimientosCamara(
  params?: GetMovimientosCamaraParams,
): Promise<MovimientoCamaraItem[]> {
  const response = await api.get<{ data: MovimientoCamaraItem[] }>('/ExistenciaCamara/movimientos', {
    params,
  })
  return response.data.data || []
}
