import api from './axios-instance'
import type {
  PaginatedResponse,
  PaginatedRequest,
  Tipificacion,
  CreateTipificacionRequest,
  UpdateTipificacionRequest,
  CatalogoFaenaOption,
  TipificacionOficialOption,
} from '@/types'

interface GetTipificacionesParams extends PaginatedRequest {
  Estado?: boolean
  EspecieId?: string
  TipoEspecieId?: string
}

export async function getTipificaciones(
  params?: GetTipificacionesParams,
): Promise<PaginatedResponse<Tipificacion>> {
  const response = await api.get<PaginatedResponse<Tipificacion>>('/Tipificaciones', { params })
  return response.data
}

export async function getTipificacion(codigo: string): Promise<Tipificacion> {
  const response = await api.get<Tipificacion>(`/Tipificaciones/${codigo}`)
  return response.data
}

export async function createTipificacion(
  data: CreateTipificacionRequest,
): Promise<{ codigo: string }> {
  const response = await api.post<{ codigo: string }>('/Tipificaciones', data)
  return response.data
}

export async function updateTipificacion(
  codigo: string,
  data: UpdateTipificacionRequest,
): Promise<void> {
  await api.put(`/Tipificaciones/${codigo}`, data)
}

export async function deleteTipificacion(codigo: string): Promise<void> {
  await api.delete(`/Tipificaciones/${codigo}`)
}

// --- Combos de referencia (endpoints de solo lectura) ---

export async function getDestinosComerciales(): Promise<CatalogoFaenaOption[]> {
  const response = await api.get<{ data: CatalogoFaenaOption[] }>('/DestinosComerciales')
  return response.data.data || []
}

export async function getTipificacionesOficiales(
  especieId?: string,
): Promise<TipificacionOficialOption[]> {
  const response = await api.get<{ data: TipificacionOficialOption[] }>('/TipificacionesOficiales', {
    params: { EspecieId: especieId },
  })
  return response.data.data || []
}

export async function getUnidadesMedidas(): Promise<CatalogoFaenaOption[]> {
  const response = await api.get<{ data: CatalogoFaenaOption[] }>('/UnidadesMedidas')
  return response.data.data || []
}
