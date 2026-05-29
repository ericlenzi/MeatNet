export interface Usuario {
  id: string
  userName: string
  nombre: string
  apellido: string
  email: string
  legajo: string
  rolId: string
  empresaId: string
  activo: boolean
}

export interface CreateUsuarioRequest {
  UserName: string
  Nombre: string
  Apellido: string
  Email?: string
  Legajo?: string
  RolId: string
  EmpresaId: string
  Activo: boolean
}

export interface UpdateUsuarioRequest {
  UserName?: string
  Nombre: string
  Apellido: string
  Email?: string
  Legajo?: string
  RolId: string
  EmpresaId: string
  Activo: boolean
}

export interface Rol {
  codigo: string
  nombre: string
  activo: boolean
}
