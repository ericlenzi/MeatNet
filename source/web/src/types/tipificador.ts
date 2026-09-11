export interface Tipificador {
  id: string
  nombre: string
  matricula: string
  establecimientoId: string
  establecimientoNombre: string
  especieId: string
  especieNombre: string | null
  /** El que el Tipificador propone en la cabecera. Uno solo por establecimiento y especie. */
  porDefecto: boolean
  activo: boolean
}

export interface CreateTipificadorRequest {
  Nombre: string
  Matricula: string
  EstablecimientoId: string
  EspecieId: string
  PorDefecto: boolean
}

export interface UpdateTipificadorRequest {
  Nombre: string
  Matricula: string
  EstablecimientoId: string
  EspecieId: string
  PorDefecto: boolean
  Activo: boolean
}
