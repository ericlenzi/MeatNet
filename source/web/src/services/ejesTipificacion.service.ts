import api from './axios-instance'
import type {
  PaginatedResponse,
  PaginatedRequest,
  EjeTipificacion,
  CreateEjeTipificacionRequest,
  UpdateEjeTipificacionRequest,
} from '@/types'

// Catalogos globales: lectura abierta (el Tipificador los necesita), escritura del SUPERADMIN.
export type EjeTipificacionId = 'conformaciones' | 'grados-engrasamiento'

const RUTAS: Record<EjeTipificacionId, string> = {
  conformaciones: '/Conformaciones',
  'grados-engrasamiento': '/GradosEngrasamiento',
}

export const ETIQUETAS: Record<EjeTipificacionId, { singular: string; plural: string }> = {
  conformaciones: { singular: 'Conformacion', plural: 'Conformaciones' },
  'grados-engrasamiento': { singular: 'Grado de Engrasamiento', plural: 'Grados de Engrasamiento' },
}

interface GetEjesParams extends PaginatedRequest {
  Estado?: boolean
  EspecieId?: string
}

export async function getEjes(
  eje: EjeTipificacionId,
  params?: GetEjesParams,
): Promise<PaginatedResponse<EjeTipificacion>> {
  const response = await api.get<PaginatedResponse<EjeTipificacion>>(RUTAS[eje], { params })
  return response.data
}

export async function getEje(eje: EjeTipificacionId, codigo: string): Promise<EjeTipificacion> {
  const response = await api.get<EjeTipificacion>(RUTAS[eje] + '/' + codigo)
  return response.data
}

export async function createEje(
  eje: EjeTipificacionId,
  data: CreateEjeTipificacionRequest,
): Promise<{ codigo: string }> {
  const response = await api.post<{ codigo: string }>(RUTAS[eje], data)
  return response.data
}

export async function updateEje(
  eje: EjeTipificacionId,
  codigo: string,
  data: UpdateEjeTipificacionRequest,
): Promise<void> {
  await api.put(RUTAS[eje] + '/' + codigo, data)
}

export async function deleteEje(eje: EjeTipificacionId, codigo: string): Promise<void> {
  await api.delete(RUTAS[eje] + '/' + codigo)
}
