import api from './axios-instance'
import type {
  ActualizarPiezaRequest,
  LiberarJornadaResponse,
  PrevisualizacionLiberacion,
  RomaneosEvaluacionResponse,
} from '@/types/evaluacionFaena'

export async function getRomaneosEvaluacion(
  listaMatanzaId: string,
): Promise<RomaneosEvaluacionResponse> {
  const response = await api.get<RomaneosEvaluacionResponse>('/EvaluacionFaena/romaneos', {
    params: { ListaMatanzaId: listaMatanzaId },
  })
  return response.data
}

export async function previsualizarLiberacion(
  listaMatanzaId: string,
): Promise<PrevisualizacionLiberacion> {
  const response = await api.get<PrevisualizacionLiberacion>('/EvaluacionFaena/previsualizar', {
    params: { ListaMatanzaId: listaMatanzaId },
  })
  return response.data
}

export async function actualizarPieza(
  piezaId: string,
  data: ActualizarPiezaRequest,
): Promise<void> {
  await api.put(`/EvaluacionFaena/pieza/${piezaId}`, data)
}

export async function liberarJornada(listaMatanzaId: string): Promise<LiberarJornadaResponse> {
  const response = await api.post<LiberarJornadaResponse>('/EvaluacionFaena/liberar', {
    listaMatanzaId,
  })
  return response.data
}
