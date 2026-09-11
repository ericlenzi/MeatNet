// --- Renglones de la LM en ejecucion (modo hibrido del Tipificador) ---
export interface RenglonEjecucionItem {
  renglonId: string
  tropaId: string
  numeroTropa: number
  almacenId: string
  almacenNombre: string
  almacenDestinoId: string | null
  almacenDestinoNombre: string | null
  tipoEspecieId: string
  tipoEspecieNombre: string
  secuencia: number
  cantidad: number
  cantidadFaenada: number
  pendiente: number
}

export interface CamaraOption {
  id: string
  nombre: string
}

export interface TipificadorOption {
  id: string
  nombre: string
  matricula: string
  porDefecto: boolean
}

export interface TipoMedicionOption {
  codigo: string
  nombre: string
}

export interface RenglonesEjecucion {
  listaMatanzaId: string
  numeroLista: number
  especieId: string
  especieNombre: string
  estadoListaMatanzaId: string
  proximoGarron: number
  renglonSugeridoId: string | null
  /** Cabecera del puesto: donde se faena la jornada. Lo trae la lista de matanza. */
  puestoId: string | null
  puestoCodigo: string | null
  puestoNombre: string | null
  /** Defaults de la cabecera: el tipificador marcado por defecto y la medicion del puesto. */
  tipificadorSugeridoId: string | null
  tipoMedicionSugeridoId: string | null
  renglones: RenglonEjecucionItem[]
  camaras: CamaraOption[]
  tipificadores: TipificadorOption[]
  tiposMediciones: TipoMedicionOption[]
}

// --- Sugerencia de tipificacion ---
export interface TipificacionCandidata {
  /** Identidad: es lo que viaja como TipificacionId al registrar el romaneo. */
  id: string
  codigo: string
  descripcion: string
  destinoComercialId: string
  destinoComercialNombre: string | null
  pesoDesde: number
  pesoHasta: number
  puntos: number
}

export interface SugerenciaTipificacion {
  propuestaId: string | null
  candidatas: TipificacionCandidata[]
}

// --- Romaneos de la jornada (grilla) ---
export interface RomaneoPiezaItem {
  letra: string | null
  peso: number
  tipoContusionNombre?: string | null
  almacenDestinoNombre: string | null
  tipificacionId: string
  tipificacionDescripcion: string | null
  pesoFueraRango: boolean
  /** Media res condenada entera: se peso, pero no va a camara. */
  decomisada?: boolean
  /** Motivo del decomiso de la pieza, sea la condena entera o el recorte de kilos. */
  motivoDecomisoNombre?: string | null
  pesoDecomisado?: number
}

export interface RomaneoJornadaItem {
  id: string
  numeroRomaneo: number
  numeroGarron: number
  numeroTropa: number
  tipoEspecieNombre: string
  unidadFaenaNombre: string
  anulado: boolean
  /** Res condenada entera: se peso, pero no va a camara. */
  decomisoTotal?: boolean
  motivoDecomisoNombre?: string | null
  fecha: string
  pesoTotal: number
  piezas: RomaneoPiezaItem[]
}

// --- Monitor de faena (read-only) ---
export interface RenglonMonitorItem {
  listaMatanzaDetalleId: string
  secuencia: number
  numeroTropa: number
  almacenNombre: string
  almacenDestinoNombre: string | null
  tipoEspecieNombre: string
  cantidad: number
  cantidadFaenada: number
  pendiente: number
  romaneoDesde: number | null
  romaneoHasta: number | null
}

export interface MonitorFaena {
  listaMatanzaId: string
  numeroLista: number
  especieNombre: string
  estadoListaMatanzaId: string
  /** Puesto (palco) de la jornada: lo declara la lista de matanza. */
  puestoCodigo: string | null
  puestoNombre: string | null
  totalPlanificado: number
  totalFaenado: number
  totalPendiente: number
  animalesRomaneados: number
  kgTotales: number
  /** Reses condenadas enteras: faenadas, pero fuera de kgTotales. */
  animalesDecomisados: number
  /** Medias reses condenadas por separado, tambien fuera de kgTotales. */
  piezasDecomisadas: number
  kgDecomisados: number
  ritmoPorHora: number
  porRenglon: RenglonMonitorItem[]
}

// --- Requests (PascalCase: matchean el backend) ---
export interface PiezaRomaneoInput {
  AlmacenDestinoId: string
  /** Vacio cuando la res esta condenada entera: esa pieza no se tipifica. */
  TipificacionId?: string
  /** Contusion de esta media res: el golpe es de la pieza, no del animal. */
  TipoContusionId?: string
  Peso: number
  ForzarFueraRango: boolean
  /** Media res condenada entera: exige motivo y deja sin pedir tipificacion ni contusion. */
  Decomisada?: boolean
  /** Motivo del decomiso de la pieza. En el recorte parcial va junto con los kilos, que no
   *  descuentan el peso de la pieza. */
  MotivoDecomisoId?: string
  PesoDecomisado?: number
}

export interface CrearRomaneoRequest {
  ListaMatanzaId: string
  ListaMatanzaDetalleId: string
  UnidadFaenaId: string
  NumeroGarron: number
  /** Quien tipifica y con que se mide. El tipificador es obligatorio cuando el establecimiento
   *  tiene tipificadores cargados para la especie. */
  TipificadorId?: string
  TipoMedicionId?: string
  /** Datos del palco, que se determinan mirando la res. Son obligatorios cuando la especie de
   *  la jornada tiene valores cargados en el catalogo; vacuno los exige, porcino no. */
  ConformacionId?: string
  GradoEngrasamientoId?: string
  DenticionId?: string
  /** Decomiso total: la res se condena entera. Exige motivo y deja sin pedir el resto. */
  DecomisoTotal?: boolean
  MotivoDecomisoId?: string
  Piezas: PiezaRomaneoInput[]
}
