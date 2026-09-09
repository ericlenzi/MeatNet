// Los dos ejes de la tipificacion oficial que se determinan mirando la res: conformacion
// (desarrollo muscular) y grado de engrasamiento (cobertura de grasa). Comparten forma, asi que
// comparten tipo; lo que los distingue es el endpoint del que salen.
export interface EjeTipificacion {
  codigo: string
  nombre: string
  especieId: string
  /** Posicion en la escala. Es el orden con el que se listan, no el alfabetico. */
  orden: number
  activo: boolean
}

export interface CreateEjeTipificacionRequest {
  Codigo: string
  Nombre: string
  EspecieId: string
  Orden: number
}

export interface UpdateEjeTipificacionRequest {
  Nombre: string
  EspecieId: string
  Orden: number
  Activo: boolean
}
