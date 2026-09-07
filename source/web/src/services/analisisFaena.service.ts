import api from './axios-instance'
import type { AnalisisFaenaResponse } from '@/types/analisisFaena'

export async function getAnalisisFaena(listaMatanzaId: string): Promise<AnalisisFaenaResponse> {
  const response = await api.get<AnalisisFaenaResponse>('/AnalisisFaena', {
    params: { ListaMatanzaId: listaMatanzaId },
  })
  return response.data
}
