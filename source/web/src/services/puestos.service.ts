import api from './axios-instance'
import type {
  PaginatedResponse,
  PaginatedRequest,
  Puesto,
  CreatePuestoRequest,
  UpdatePuestoRequest,
} from '@/types'

interface GetPuestosParams extends PaginatedRequest {
  EstablecimientoId?: string
  EspecieId?: string
  Estado?: boolean
}

export async function getPuestos(params?: GetPuestosParams): Promise<PaginatedResponse<Puesto>> {
  const response = await api.get<PaginatedResponse<Puesto>>('/Puestos', { params })
  return response.data
}

export async function getPuesto(id: string): Promise<Puesto> {
  const response = await api.get<Puesto>(`/Puestos/${id}`)
  return response.data
}

export async function createPuesto(data: CreatePuestoRequest): Promise<{ id: string }> {
  const response = await api.post<{ id: string }>('/Puestos', data)
  return response.data
}

export async function updatePuesto(id: string, data: UpdatePuestoRequest): Promise<void> {
  await api.put(`/Puestos/${id}`, data)
}

export async function deletePuesto(id: string): Promise<void> {
  await api.delete(`/Puestos/${id}`)
}

/**
 * Puestos activos para poblar combos. La especie es parte de la identidad del puesto, asi que
 * el alta de una lista de matanza pide los de su especie: el palco vacuno no faena porcinos.
 */
export async function getPuestosOptions(
  establecimientoId: string,
  especieId?: string,
): Promise<Puesto[]> {
  const response = await getPuestos({
    EstablecimientoId: establecimientoId,
    EspecieId: especieId,
    Estado: true,
    PageSize: 1000,
  })
  return response.data || []
}
