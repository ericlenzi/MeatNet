import api from './axios-instance'
import type { Agrupacion, ExistenciaCamaraItem, MovimientoCamaraItem } from '@/types/existenciaCamara'

interface GetExistenciaCamaraParams {
  EstablecimientoId?: string
  AlmacenId?: string
  MaterialId?: string
  ClienteId?: string
  AgruparPor?: Agrupacion
  IncluirSaldoCero?: boolean
}

interface ExistenciaCamaraResponse {
  data: ExistenciaCamaraItem[]
  agruparPor: Agrupacion
  totalCantidad: number
  totalPeso: number
}

export async function getExistenciaCamara(
  params?: GetExistenciaCamaraParams,
): Promise<ExistenciaCamaraResponse> {
  const response = await api.get<ExistenciaCamaraResponse>('/ExistenciaCamara', { params })
  return {
    data: response.data.data || [],
    agruparPor: response.data.agruparPor,
    totalCantidad: response.data.totalCantidad ?? 0,
    totalPeso: response.data.totalPeso ?? 0,
  }
}

interface GetMovimientosCamaraParams {
  AlmacenId?: string
  MaterialId?: string
  TropaId?: string
  ClienteId?: string
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
