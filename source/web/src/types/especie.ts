export interface Especie {
  codigo: string
  nombre: string
  activo: boolean
}

export interface CreateEspecieRequest {
  Codigo: string
  Nombre: string
}

export interface UpdateEspecieRequest {
  Nombre: string
  Activo: boolean
}
