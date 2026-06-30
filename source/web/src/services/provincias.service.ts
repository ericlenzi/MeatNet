import api from './axios-instance'

export interface ProvinciaItem {
  id: number
  nombre: string
}

interface ProvinciasResponse {
  data: ProvinciaItem[]
}

export async function getProvincias(): Promise<ProvinciaItem[]> {
  const response = await api.get<ProvinciasResponse>('/Provincias')
  return response.data.data || []
}
