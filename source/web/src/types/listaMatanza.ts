// --- Listado ---
export interface ListaMatanzaListItem {
  id: string
  numeroLista: number
  fecha: string
  establecimientoId: string
  establecimientoNombre: string
  especieId: string
  especieNombre: string
  estadoListaMatanzaId: string
  estadoListaMatanzaNombre: string
  version: number
  totalRenglones: number
  totalCabezas: number
}

// --- Disponibilidad para planificar ---
export interface DisponibilidadFaenaItem {
  tropaId: string
  numeroTropa: number
  almacenId: string
  almacenNombre: string
  clienteId: string
  clienteNombre: string
  enPie: number
  reservado: number
  disponible: number
  pesoPromedio: number
}

// --- Detalle ---
export interface ListaMatanzaRenglon {
  id: string
  tropaId: string
  numeroTropa: number
  almacenId: string
  almacenNombre: string
  secuencia: number
  cantidad: number
  cantidadFaenada: number
}

export interface ListaMatanzaMovimiento {
  id: string
  version: number
  fecha: string
  usuarioId: string
  tipoMovimiento: string
  tropaId: string | null
  almacenId: string | null
  cantidadAnterior: number | null
  cantidadNueva: number | null
  secuenciaAnterior: number | null
  secuenciaNueva: number | null
  motivo: string | null
}

export interface ListaMatanza {
  id: string
  numeroLista: number
  fecha: string
  establecimientoId: string
  establecimientoNombre: string
  especieId: string
  especieNombre: string
  estadoListaMatanzaId: string
  estadoListaMatanzaNombre: string
  version: number
  fechaConfirmacion: string | null
  fechaInicioEjecucion: string | null
  fechaFinalizacion: string | null
  renglones: ListaMatanzaRenglon[]
  movimientos: ListaMatanzaMovimiento[]
}

// --- Requests (PascalCase: matchean el backend) ---
export interface RenglonInput {
  TropaId: string
  AlmacenId: string
  Secuencia: number
  Cantidad: number
}

export interface CreateListaMatanzaRequest {
  EstablecimientoId: string
  EspecieId: string
  Fecha: string
  Renglones: RenglonInput[]
}

export type UpdateListaMatanzaRequest = Omit<CreateListaMatanzaRequest, 'EstablecimientoId'>

// --- Estados (codigos del catalogo) ---
export const EstadoListaMatanza = {
  Borrador: 'BORRADOR',
  Confirmada: 'CONFIRMADA',
  EnEjecucion: 'EN_EJECUCION',
  Finalizada: 'FINALIZADA',
  Anulada: 'ANULADA',
} as const
