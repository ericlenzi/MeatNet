import api from './axios-instance'
import type {
  PaginatedRequest,
  PaginatedResponse,
  IngresoHaciendaListItem,
  IngresoHacienda,
  CreateIngresoHaciendaRequest,
  UpdateIngresoHaciendaRequest,
  AprobarIngresoHaciendaResult,
} from '@/types'

interface GetIngresosHaciendasParams extends PaginatedRequest {
  EstablecimientoId?: string
  EstadoIngresoId?: string
}

export async function getIngresosHaciendas(
  params?: GetIngresosHaciendasParams,
): Promise<PaginatedResponse<IngresoHaciendaListItem>> {
  const response = await api.get<PaginatedResponse<IngresoHaciendaListItem>>('/IngresosHacienda', { params })
  return response.data
}

export async function getIngresoHacienda(id: string): Promise<IngresoHacienda> {
  const response = await api.get<IngresoHacienda>(`/IngresosHacienda/${id}`)
  return response.data
}

export async function createIngresoHacienda(
  data: CreateIngresoHaciendaRequest,
): Promise<{ id: string; numeroIngreso: number }> {
  const response = await api.post<{ id: string; numeroIngreso: number }>('/IngresosHacienda', data)
  return response.data
}

export async function updateIngresoHacienda(
  id: string,
  data: UpdateIngresoHaciendaRequest,
): Promise<void> {
  await api.put(`/IngresosHacienda/${id}`, data)
}

export async function deleteIngresoHacienda(id: string): Promise<void> {
  await api.delete(`/IngresosHacienda/${id}`)
}

// --- Workflow ---

export async function enviarAprobacionIngresoHacienda(id: string): Promise<void> {
  await api.post(`/IngresosHacienda/${id}/enviar-aprobacion`)
}

export async function aprobarIngresoHacienda(id: string): Promise<AprobarIngresoHaciendaResult> {
  const response = await api.post<AprobarIngresoHaciendaResult>(`/IngresosHacienda/${id}/aprobar`)
  return response.data
}

export async function rechazarIngresoHacienda(id: string): Promise<void> {
  await api.post(`/IngresosHacienda/${id}/rechazar`)
}

export async function anularIngresoHacienda(id: string): Promise<void> {
  await api.post(`/IngresosHacienda/${id}/anular`)
}
