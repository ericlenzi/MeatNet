export interface Especie {
  codigo: string
  nombre: string
  /** Banda de rinde caliente esperable (%). Sin ella, el Análisis no avisa nada (R-A7). */
  rindeMinimo: number | null
  rindeMaximo: number | null
  /** Merma de oreo de referencia (%), base del rinde frio estimado (R-A8). */
  mermaOreoReferencia: number | null
  activo: boolean
}

export interface CreateEspecieRequest {
  Codigo: string
  Nombre: string
  RindeMinimo: number | null
  RindeMaximo: number | null
  MermaOreoReferencia: number | null
}

export interface UpdateEspecieRequest {
  Nombre: string
  RindeMinimo: number | null
  RindeMaximo: number | null
  MermaOreoReferencia: number | null
  Activo: boolean
}
