export interface ExistenciaCamaraItem {
  almacenId: string
  almacenNombre: string
  materialId: string
  materialCodigo: string
  materialNombre: string
  tipoMaterialId: string
  tipoMaterialNombre: string
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
  especieId: string | null
  tipoEspecieId: string | null
  referencia: string | null
}
