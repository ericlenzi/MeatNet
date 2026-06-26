export interface NumeradorTropa {
  id: string
  clienteEstablecimientoId: string
  codigoCliente: string
  nombreCliente: string
  codigoEstablecimiento: string
  nombreEstablecimiento: string
  especieCodigo: string
  especieNombre: string
  ultimoNumeroTropa: number
}

export interface CreateNumeradorTropaRequest {
  ClienteEstablecimientoId: string
  EspecieCodigo: string
  UltimoNumeroTropa: number
}

export interface UpdateNumeradorTropaRequest {
  UltimoNumeroTropa: number
}
