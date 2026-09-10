export interface Especie {
  codigo: string
  nombre: string
  /** Banda de rinde caliente esperable (%). Sin ella, el Análisis no avisa nada (R-A7). */
  rindeMinimo: number | null
  rindeMaximo: number | null
  activo: boolean
}

export interface CreateEspecieRequest {
  Codigo: string
  Nombre: string
  RindeMinimo: number | null
  RindeMaximo: number | null
}

export interface UpdateEspecieRequest {
  Nombre: string
  RindeMinimo: number | null
  RindeMaximo: number | null
  Activo: boolean
}
