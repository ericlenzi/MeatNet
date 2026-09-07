export interface AnalisisClienteItem {
  clienteId: string
  clienteNombre: string
  animalesFaenados: number
  piezas: number
  kgFaena: number
  kgVivos: number | null
  rindeCaliente: number | null
  participacionKg: number
}

export interface PlanVsRealItem {
  numeroTropa: number
  clienteId: string
  clienteNombre: string
  corralNombre: string
  tipoEspecieNombre: string
  secuencia: number
  planificado: number
  faenado: number
  diferencia: number
  cumplimiento: number
  pesoPromedioVivo: number | null
}

export interface TipificacionConsolidadaItem {
  tipificacionId: string
  descripcion: string
  materialNombre: string | null
  piezas: number
  kgFaena: number
  pesoPromedio: number
  participacionKg: number
}

export interface DispersionPesoItem {
  tipoEspecieId: string
  tipoEspecieNombre: string
  piezas: number
  pesoPromedio: number
  pesoMinimo: number
  pesoMaximo: number
  piezasFueraRango: number
}

export interface DestinoCamaraItem {
  almacenNombre: string
  materialNombre: string
  cantidad: number
  peso: number
}

export interface AnalisisFaenaResponse {
  listaMatanzaId: string
  numeroLista: number
  fecha: string
  especieId: string
  estadoListaMatanzaId: string
  establecimientoNombre: string
  animalesFaenados: number
  piezas: number
  kgFaena: number
  kgVivos: number | null
  rindeCaliente: number | null
  animalesSinPesoVivo: number
  piezasLiberadas: number
  porCliente: AnalisisClienteItem[]
  planVsReal: PlanVsRealItem[]
  tipificaciones: TipificacionConsolidadaItem[]
  dispersion: DispersionPesoItem[]
  camaras: DestinoCamaraItem[]
}
