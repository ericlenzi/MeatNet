export interface Parametro {
  codigo: string
  nombre: string
  valor: string
  activo: boolean
}

export interface CreateParametroRequest {
  Codigo: string
  Nombre: string
  Valor?: string
}

export interface UpdateParametroRequest {
  Nombre: string
  Valor?: string
  Activo: boolean
}
