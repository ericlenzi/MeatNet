export interface AnalisisClienteItem {
  clienteId: string
  clienteNombre: string
  animalesFaenados: number
  piezas: number
  kgFaena: number
  kgVivos: number | null
  rindeCaliente: number | null
  participacionKg: number
  /** Kg condenados de este cliente: res entera o recorte parcial. */
  kgDecomisados: number
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

/** Los decomisos de la jornada agrupados por su causa sanitaria. */
export interface DecomisoMotivoItem {
  motivoCodigo: string
  motivoNombre: string
  /** Reses condenadas enteras por este motivo. */
  animales: number
  /** Medias reses condenadas enteras por este motivo. */
  piezasCondenadas: number
  /** Medias reses con recorte parcial por este motivo. */
  piezas: number
  kg: number
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
  /** Banda de rinde esperable de la especie (%); null si no está configurada (R-A7). */
  rindeMinimo: number | null
  rindeMaximo: number | null
  /** El rinde quedó fuera de esa banda: casi siempre es el peso vivo de ingreso. */
  rindeFueraDeRango: boolean
  piezasLiberadas: number
  // Merma sanitaria: el rinde no se retoca, los decomisos se informan al lado.
  animalesDecomisados: number
  kgDecomisoTotal: number
  piezasDecomisadas: number
  kgDecomisoPieza: number
  piezasConDecomisoParcial: number
  kgDecomisoParcial: number
  kgDecomisados: number
  mermaSanitaria: number | null
  porCliente: AnalisisClienteItem[]
  planVsReal: PlanVsRealItem[]
  tipificaciones: TipificacionConsolidadaItem[]
  dispersion: DispersionPesoItem[]
  camaras: DestinoCamaraItem[]
  decomisos: DecomisoMotivoItem[]
}
