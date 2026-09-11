// Catalogos globales de codigo + nombre, sin nada mas: tipos de puesto y tipos de medicion.
// Comparten forma, asi que comparten tipo, servicio y pantallas.
export interface CatalogoSimple {
  codigo: string
  nombre: string
  activo: boolean
}

export interface CreateCatalogoSimpleRequest {
  Codigo: string
  Nombre: string
}

export interface UpdateCatalogoSimpleRequest {
  Nombre: string
  Activo: boolean
}
