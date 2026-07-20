import api from './axios-instance'
import type {
  RenglonesEjecucion,
  SugerenciaTipificacion,
  RomaneoJornadaItem,
  MonitorFaena,
  CrearRomaneoRequest,
} from '@/types'

export async function getRenglonesEjecucion(listaMatanzaId: string): Promise<RenglonesEjecucion> {
  const response = await api.get<RenglonesEjecucion>('/Romaneos/renglones', {
    params: { ListaMatanzaId: listaMatanzaId },
  })
  return response.data
}

interface SugerirTipificacionParams {
  EspecieId: string
  TipoEspecieId: string
  UnidadFaenaId: string
  DestinoComercialId?: string
  Peso?: number
}

export async function sugerirTipificacion(
  params: SugerirTipificacionParams,
): Promise<SugerenciaTipificacion> {
  const response = await api.get<SugerenciaTipificacion>('/Romaneos/sugerir-tipificacion', { params })
  return response.data
}

export async function getRomaneosJornada(listaMatanzaId: string): Promise<RomaneoJornadaItem[]> {
  const response = await api.get<{ data: RomaneoJornadaItem[] }>('/Romaneos/jornada', {
    params: { ListaMatanzaId: listaMatanzaId },
  })
  return response.data.data || []
}

export async function getMonitorFaena(listaMatanzaId: string): Promise<MonitorFaena> {
  const response = await api.get<MonitorFaena>('/Romaneos/monitor', {
    params: { ListaMatanzaId: listaMatanzaId },
  })
  return response.data
}

export async function crearRomaneo(
  data: CrearRomaneoRequest,
): Promise<{ id: string; numeroRomaneo: number; numeroGarron: number }> {
  const response = await api.post<{ id: string; numeroRomaneo: number; numeroGarron: number }>('/Romaneos', data)
  return response.data
}

export async function anularRomaneo(id: string, motivo?: string): Promise<void> {
  await api.post(`/Romaneos/${id}/anular`, { Motivo: motivo ?? '' })
}
