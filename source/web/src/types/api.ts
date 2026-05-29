export interface PaginatedResponse<T> {
  data: T[]
  totalRows: number
}

export interface PaginatedRequest {
  Filter?: string
  PageIndex?: number
  PageSize?: number
}
