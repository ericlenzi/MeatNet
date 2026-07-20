import api from './axios-instance'
import type { PaginatedResponse } from '@/types'

export interface PuestoOption {
  id: string
  codigoPuesto: string
  nombre: string
  establecimientoId: string
  tipoPuestoId: string | null
  activo: boolean
}

/** Puestos (palcos de faena) de un establecimiento. */
export async function getPuestos(establecimientoId: string): Promise<PuestoOption[]> {
  const response = await api.get<PaginatedResponse<PuestoOption>>('/Puestos', {
    params: { EstablecimientoId: establecimientoId, PageSize: 1000 },
  })
  return response.data.data || []
}
