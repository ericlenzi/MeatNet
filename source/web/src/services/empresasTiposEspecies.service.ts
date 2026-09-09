import api from './axios-instance'
import type {
  PaginatedResponse,
  PaginatedRequest,
  EmpresaTipoEspecie,
  CreateEmpresaTipoEspecieRequest,
  UpdateEmpresaTipoEspecieRequest,
} from '@/types'

interface GetEmpresasTiposEspeciesParams extends PaginatedRequest {
  /** Estado efectivo: cuenta el Activo de la empresa y el del catalogo. */
  Estado?: boolean
  EspecieId?: string
}

// Categorias con las que opera la empresa activa. Es lo que hay que pedir para llenar un combo
// operativo: el catalogo global tiene todas las del rubro, no solo las de esta empresa.
export async function getEmpresasTiposEspecies(
  params?: GetEmpresasTiposEspeciesParams,
): Promise<PaginatedResponse<EmpresaTipoEspecie>> {
  const response = await api.get<PaginatedResponse<EmpresaTipoEspecie>>(
    '/EmpresasTiposEspecies',
    { params },
  )
  return response.data
}

export async function getEmpresaTipoEspecie(id: string): Promise<EmpresaTipoEspecie> {
  const response = await api.get<EmpresaTipoEspecie>(`/EmpresasTiposEspecies/${id}`)
  return response.data
}

export async function createEmpresaTipoEspecie(
  data: CreateEmpresaTipoEspecieRequest,
): Promise<{ id: string }> {
  const response = await api.post<{ id: string }>('/EmpresasTiposEspecies', data)
  return response.data
}

export async function updateEmpresaTipoEspecie(
  id: string,
  data: UpdateEmpresaTipoEspecieRequest,
): Promise<void> {
  await api.put(`/EmpresasTiposEspecies/${id}`, data)
}

export async function deleteEmpresaTipoEspecie(id: string): Promise<void> {
  await api.delete(`/EmpresasTiposEspecies/${id}`)
}
