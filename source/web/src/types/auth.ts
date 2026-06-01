export interface LoginRequest {
  Usuario: string
  Contraseña: string
}

export interface CurrentUser {
  id: string
  userName: string
  nombreCompleto: string
  rolId: string
  codigoEmpresa: string
  nombreEmpresa: string
  codigoSucursal: string
}

export interface LoginResponse {
  token: string
  currentUser: CurrentUser
}
