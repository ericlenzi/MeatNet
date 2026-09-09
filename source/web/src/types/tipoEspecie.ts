// Catalogo global de categorias de hacienda: lo administra el SUPERADMIN desde la empresa ADM.
// La identidad es el codigo, no un Guid. Lo que cada empresa ajusta esta en empresaTipoEspecie.
export interface TipoEspecie {
  codigo: string
  nombre: string
  especieId: string
  especieNombre: string
  tipoSexoId: string
  tipoSexoNombre: string
  pesoTeoricoReferencia: number
  activo: boolean
}

export interface CreateTipoEspecieRequest {
  Codigo: string
  Nombre: string
  EspecieId: string
  TipoSexoId?: string
  PesoTeoricoReferencia?: number
}

export interface UpdateTipoEspecieRequest {
  Nombre: string
  EspecieId: string
  TipoSexoId?: string
  PesoTeoricoReferencia?: number
  Activo: boolean
}

export interface TipoSexo {
  codigo: string
  nombre: string
  activo: boolean
}
