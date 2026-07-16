import api from './axios-instance'
import type {
  PaginatedRequest,
  PaginatedResponse,
  ListaMatanzaListItem,
  ListaMatanza,
  DisponibilidadFaenaItem,
  CreateListaMatanzaRequest,
  UpdateListaMatanzaRequest,
} from '@/types'

interface GetListasMatanzasParams extends PaginatedRequest {
  EstablecimientoId?: string
  EspecieId?: string
  EstadoListaMatanzaId?: string
  Fecha?: string
}

export async function getListasMatanzas(
  params?: GetListasMatanzasParams,
): Promise<PaginatedResponse<ListaMatanzaListItem>> {
  const response = await api.get<PaginatedResponse<ListaMatanzaListItem>>('/ListasMatanzas', { params })
  return response.data
}

export async function getListaMatanza(id: string): Promise<ListaMatanza> {
  const response = await api.get<ListaMatanza>(`/ListasMatanzas/${id}`)
  return response.data
}

interface GetDisponibilidadParams {
  EstablecimientoId: string
  EspecieId: string
  ExcludeListaId?: string
}

export async function getDisponibilidadFaena(
  params: GetDisponibilidadParams,
): Promise<DisponibilidadFaenaItem[]> {
  const response = await api.get<{ data: DisponibilidadFaenaItem[] }>('/ListasMatanzas/disponibilidad', { params })
  return response.data.data || []
}

export async function createListaMatanza(
  data: CreateListaMatanzaRequest,
): Promise<{ id: string; numeroLista: number }> {
  const response = await api.post<{ id: string; numeroLista: number }>('/ListasMatanzas', data)
  return response.data
}

export async function updateListaMatanza(
  id: string,
  data: UpdateListaMatanzaRequest,
): Promise<void> {
  await api.put(`/ListasMatanzas/${id}`, data)
}

export async function deleteListaMatanza(id: string): Promise<void> {
  await api.delete(`/ListasMatanzas/${id}`)
}

// --- Edicion controlada post-confirmacion (auditada) ---

export interface AgregarRenglonRequest {
  TropaId: string
  AlmacenId: string
  TipoEspecieId: string
  Cantidad: number
  Secuencia?: number
  Motivo?: string
}

export async function agregarRenglonListaMatanza(
  id: string,
  data: AgregarRenglonRequest,
): Promise<{ renglonId: string }> {
  const response = await api.post<{ renglonId: string }>(`/ListasMatanzas/${id}/renglones`, data)
  return response.data
}

export interface EditarRenglonRequest {
  Cantidad: number
  Secuencia: number
  Motivo?: string
}

export async function editarRenglonListaMatanza(
  id: string,
  renglonId: string,
  data: EditarRenglonRequest,
): Promise<void> {
  await api.put(`/ListasMatanzas/${id}/renglones/${renglonId}`, data)
}

export async function quitarRenglonListaMatanza(id: string, renglonId: string): Promise<void> {
  await api.delete(`/ListasMatanzas/${id}/renglones/${renglonId}`)
}

export interface FaenaEmergenciaRequest {
  TropaId: string
  AlmacenId: string
  TipoEspecieId: string
  Cantidad: number
  Motivo?: string
}

export async function faenaEmergenciaListaMatanza(
  id: string,
  data: FaenaEmergenciaRequest,
): Promise<{ renglonId: string; secuencia: number }> {
  const response = await api.post<{ renglonId: string; secuencia: number }>(`/ListasMatanzas/${id}/emergencia`, data)
  return response.data
}

// --- Workflow ---

export async function confirmarListaMatanza(id: string): Promise<void> {
  await api.post(`/ListasMatanzas/${id}/confirmar`)
}

export async function desconfirmarListaMatanza(id: string): Promise<void> {
  await api.post(`/ListasMatanzas/${id}/desconfirmar`)
}

export async function iniciarListaMatanza(id: string): Promise<void> {
  await api.post(`/ListasMatanzas/${id}/iniciar`)
}

export interface CierreListaMatanzaResult {
  totalLiberado: number
  renglonesConSobrante: number
}

export async function finalizarListaMatanza(
  id: string,
  motivo?: string,
): Promise<CierreListaMatanzaResult> {
  const response = await api.post<CierreListaMatanzaResult>(`/ListasMatanzas/${id}/finalizar`, {
    Motivo: motivo ?? '',
  })
  return response.data
}

export async function cancelarListaMatanza(id: string, motivo?: string): Promise<void> {
  await api.post(`/ListasMatanzas/${id}/cancelar`, { Motivo: motivo ?? '' })
}
