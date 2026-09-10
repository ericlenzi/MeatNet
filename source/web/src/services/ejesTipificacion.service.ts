import api from './axios-instance'
import type {
  PaginatedResponse,
  PaginatedRequest,
  EjeTipificacion,
  CreateEjeTipificacionRequest,
  UpdateEjeTipificacionRequest,
} from '@/types'

// Catalogos que el Tipificador consulta al romanear. Comparten forma (codigo, nombre, especie,
// orden) y reglas, asi que comparten servicio y pantallas.
// Los cuatro primeros son los datos del palco; el quinto son los motivos de decomiso, que no son
// una escala pero se administran igual (en ellos Orden es solo el orden de la lista del puesto).
// Catalogos globales: lectura abierta (el Tipificador los necesita), escritura del SUPERADMIN.
export type EjeTipificacionId =
  | 'conformaciones'
  | 'grados-engrasamiento'
  | 'denticiones'
  | 'tipos-contusiones'
  | 'motivos-decomisos'

const RUTAS: Record<EjeTipificacionId, string> = {
  conformaciones: '/Conformaciones',
  'grados-engrasamiento': '/GradosEngrasamiento',
  denticiones: '/Denticiones',
  'tipos-contusiones': '/TiposContusiones',
  'motivos-decomisos': '/MotivosDecomisos',
}

export const ETIQUETAS: Record<EjeTipificacionId, { singular: string; plural: string }> = {
  conformaciones: { singular: 'Conformacion', plural: 'Conformaciones' },
  'grados-engrasamiento': { singular: 'Grado de Engrasamiento', plural: 'Grados de Engrasamiento' },
  denticiones: { singular: 'Denticion', plural: 'Denticiones' },
  'tipos-contusiones': { singular: 'Tipo de Contusion', plural: 'Tipos de Contusion' },
  'motivos-decomisos': { singular: 'Motivo de Decomiso', plural: 'Motivos de Decomiso' },
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
