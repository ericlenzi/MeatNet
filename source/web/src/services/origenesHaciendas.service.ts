import api from './axios-instance'
import type { OrigenHacienda } from '@/types'

interface OrigenesHaciendasResponse {
  data: OrigenHacienda[]
}

export async function getOrigenesHaciendas(): Promise<OrigenHacienda[]> {
  const response = await api.get<OrigenesHaciendasResponse>('/OrigenesHaciendas')
  return response.data.data || []
}
