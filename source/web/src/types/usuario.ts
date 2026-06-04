export interface Usuario {
  id: string
  userName: string
  nombre: string
  apellido: string
  email: string
  legajo: string
  rolId: string
  activo: boolean
}

export interface CreateUsuarioRequest {
  UserName: string
  Nombre: string
  Apellido: string
  Email?: string
  Legajo?: string
  RolId: string
  Activo: boolean
}

export interface UpdateUsuarioRequest {
  UserName?: string
  Nombre: string
  Apellido: string
  Email?: string
  Legajo?: string
  RolId: string
  Activo: boolean
}

export interface Rol {
  codigo: string
  nombre: string
  activo: boolean
}

export interface CreateRolRequest {
  Codigo: string
  Nombre: string
  Activo: boolean
}

export interface UpdateRolRequest {
  Nombre: string
  Activo: boolean
}
