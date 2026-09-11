import api from './axios-instance'
import type {
  PaginatedResponse,
  PaginatedRequest,
  CatalogoSimple,
  CreateCatalogoSimpleRequest,
  UpdateCatalogoSimpleRequest,
} from '@/types'

// Catalogos globales que son solo codigo + nombre: la clase de puesto (PAL, palco) y el metodo
// con el que se mide en el puesto (M manual, B balanza, A automatica). Comparten forma y reglas,
// asi que comparten servicio y pantallas; lo unico que cambia es el endpoint.
// Lectura abierta (los combos operativos los necesitan), escritura del SUPERADMIN.
export type CatalogoSimpleId = 'tipos-puestos' | 'tipos-mediciones'

const RUTAS: Record<CatalogoSimpleId, string> = {
  'tipos-puestos': '/TiposPuestos',
  'tipos-mediciones': '/TiposMediciones',
}

export const ETIQUETAS: Record<CatalogoSimpleId, { singular: string; plural: string }> = {
  'tipos-puestos': { singular: 'Tipo de Puesto', plural: 'Tipos de Puesto' },
  'tipos-mediciones': { singular: 'Tipo de Medicion', plural: 'Tipos de Medicion' },
}

interface GetCatalogoParams extends PaginatedRequest {
  Estado?: boolean
}

export async function getCatalogo(
  catalogo: CatalogoSimpleId,
  params?: GetCatalogoParams,
): Promise<PaginatedResponse<CatalogoSimple>> {
  const response = await api.get<PaginatedResponse<CatalogoSimple>>(RUTAS[catalogo], { params })
  return response.data
}

export async function getCatalogoItem(
  catalogo: CatalogoSimpleId,
  codigo: string,
): Promise<CatalogoSimple> {
  const response = await api.get<CatalogoSimple>(RUTAS[catalogo] + '/' + codigo)
  return response.data
}

export async function createCatalogoItem(
  catalogo: CatalogoSimpleId,
  data: CreateCatalogoSimpleRequest,
): Promise<{ codigo: string }> {
  const response = await api.post<{ codigo: string }>(RUTAS[catalogo], data)
  return response.data
}

export async function updateCatalogoItem(
  catalogo: CatalogoSimpleId,
  codigo: string,
  data: UpdateCatalogoSimpleRequest,
): Promise<void> {
  await api.put(RUTAS[catalogo] + '/' + codigo, data)
}

export async function deleteCatalogoItem(
  catalogo: CatalogoSimpleId,
  codigo: string,
): Promise<void> {
  await api.delete(RUTAS[catalogo] + '/' + codigo)
}

/** Opciones activas para poblar combos. */
export async function getCatalogoOptions(catalogo: CatalogoSimpleId): Promise<CatalogoSimple[]> {
  const response = await getCatalogo(catalogo, { Estado: true, PageSize: 1000 })
  return response.data || []
}
