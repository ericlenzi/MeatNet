import api from './axios-instance'

export interface Especie {
  codigo: string
  nombre: string
  activo: boolean
}

interface EspeciesResponse {
  data: Especie[]
}

export async function getEspecies(): Promise<Especie[]> {
  const response = await api.get<EspeciesResponse>('/Especies')
  return response.data.data || []
}
