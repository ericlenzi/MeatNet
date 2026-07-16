export interface TrazabilidadMovimiento {
  fecha: string
  fase: string
  tipoMovimiento: string
  estadoResultanteId: string | null
  detalle: string | null
  referencia: string | null
  usuarioNombre: string | null
}

export interface TrazabilidadTropa {
  tropaId: string
  numeroTropa: number
  especieCodigo: string
  especieNombre: string
  clienteNombre: string
  establecimientoNombre: string
  numeroIngreso: number
  estadoTropaId: string
  estadoTropaNombre: string
  fechaRecepcion: string
  movimientos: TrazabilidadMovimiento[]
}
