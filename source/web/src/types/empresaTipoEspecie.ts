// Configuracion de la empresa sobre el catalogo global de categorias. Es la fuente de los
// combos operativos: el catalogo lista todas las categorias del rubro, esto lista las suyas.
export interface EmpresaTipoEspecie {
  id: string
  /** Codigo de la categoria en el catalogo global. Es lo que guardan las tablas de operacion. */
  tipoEspecieId: string
  nombre: string
  especieId: string
  especieNombre: string
  tipoSexoId: string
  tipoSexoNombre: string
  /** El que usan los calculos. */
  pesoTeorico: number
  /** Sugerido del catalogo, para ver el desvio. */
  pesoTeoricoReferencia: number
  erP_Codigo: string
  activo: boolean
  /** Estado en el catalogo global: si es false la categoria no se puede usar. */
  tipoEspecieActivo: boolean
}

export interface CreateEmpresaTipoEspecieRequest {
  TipoEspecieId: string
  /** Si no se manda, la API copia el peso de referencia del catalogo. */
  PesoTeorico?: number
  ERP_Codigo?: string
}

export interface UpdateEmpresaTipoEspecieRequest {
  PesoTeorico: number
  ERP_Codigo?: string
  Activo: boolean
}
