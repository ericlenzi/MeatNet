export interface EspecieItem {
  id: string
  nombre: string
  /** Merma de oreo que declara esta planta para esta especie (%), si declaro una (R-A8). */
  mermaOreo: number | null
  /** La del catalogo de la especie: es la que vale cuando la planta no declara ninguna. */
  mermaOreoReferencia: number | null
}

/** Especie habilitada con el parametro que le cuelga la planta. */
export interface EstablecimientoEspecieInput {
  EspecieId: string
  MermaOreo: number | null
}

export interface Establecimiento {
  id: string
  codigoEstablecimiento: string
  nombre: string
  sucursalId: string
  sucursalNombre: string
  especies: EspecieItem[]
  empresaId: string
  empresaNombre: string
  numeroSenasa: string
  numeroRuca: string
  activo: boolean
}

export interface CreateEstablecimientoRequest {
  CodigoEstablecimiento: string
  Nombre: string
  SucursalId: string
  Especies: EstablecimientoEspecieInput[]
  NumeroSenasa?: string
  NumeroRuca?: string
}

export interface UpdateEstablecimientoRequest {
  Nombre: string
  SucursalId: string
  Especies: EstablecimientoEspecieInput[]
  NumeroSenasa?: string
  NumeroRuca?: string
  Activo: boolean
}
