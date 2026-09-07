/** Corte por el que se agrupa la existencia. Cada uno responde una pregunta distinta. */
export const AgrupacionExistencia = {
  /** Por cámara y material: cuánto hay de cada producto y dónde. Inventario. */
  Material: 'MATERIAL',
  /** Por cliente y material: qué tiene cada proveedor. */
  Proveedor: 'PROVEEDOR',
  /** Por cámara y cliente: de quién es lo que hay en cada cámara. */
  Camara: 'CAMARA',
} as const

export type Agrupacion = (typeof AgrupacionExistencia)[keyof typeof AgrupacionExistencia]

export interface ExistenciaCamaraItem {
  almacenId: string | null
  almacenNombre: string | null
  materialId: string | null
  materialCodigo: string | null
  materialNombre: string | null
  tipoMaterialId: string | null
  tipoMaterialNombre: string | null
  clienteId: string | null
  clienteNombre: string | null
  cantidad: number
  peso: number
  ultimoMovimiento: string | null
}

export interface MovimientoCamaraItem {
  id: string
  fecha: string
  tipoMovimientoId: string
  tipoMovimientoNombre: string
  almacenId: string
  almacenNombre: string
  materialId: string
  materialCodigo: string
  materialNombre: string
  cantidad: number
  peso: number
  transformacionId: string | null
  romaneoPiezaOrigenId: string | null
  numeroRomaneo: number | null
  numeroGarron: number | null
  letra: string | null
  tropaId: string | null
  numeroTropa: number | null
  clienteId: string | null
  clienteNombre: string | null
  especieId: string | null
  tipoEspecieId: string | null
  referencia: string | null
}
