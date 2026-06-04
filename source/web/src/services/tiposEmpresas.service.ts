import api from './axios-instance'
import type { TipoEmpresa } from '@/types'

interface TiposEmpresasResponse {
  data: TipoEmpresa[]
}

export async function getTiposEmpresas(): Promise<TipoEmpresa[]> {
  const response = await api.get<TiposEmpresasResponse>('/TiposEmpresas')
  return response.data.data || []
}
