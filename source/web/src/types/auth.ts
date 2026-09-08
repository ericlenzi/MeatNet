export interface LoginRequest {
  Usuario: string
  Contraseña: string
}

export interface CurrentUser {
  id: string
  userName: string
  nombreCompleto: string
  rolId: string
  empresaId: string
  nombreEmpresa: string
  colorEmpresa: string | null
  logoEmpresa: string | null
  codigoSucursal: string
}

export interface LoginResponse {
  token: string
  currentUser: CurrentUser
  debeCambiarContrasena: boolean
}
