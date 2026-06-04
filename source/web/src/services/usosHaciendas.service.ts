import api from './axios-instance'
import type { UsoHacienda } from '@/types'

interface UsosHaciendasResponse {
  data: UsoHacienda[]
}

export async function getUsosHaciendas(): Promise<UsoHacienda[]> {
  const response = await api.get<UsosHaciendasResponse>('/UsosHaciendas')
  return response.data.data || []
}
