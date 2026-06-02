import api from './axios-instance'
import type { TipoCliente } from '@/types'

interface TiposClientesResponse {
  data: TipoCliente[]
}

export async function getTiposClientes(): Promise<TipoCliente[]> {
  const response = await api.get<TiposClientesResponse>('/TiposClientes')
  return response.data.data || []
}
