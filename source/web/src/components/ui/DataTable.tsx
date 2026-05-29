import type { ReactNode } from 'react'
import Spinner from './Spinner'

export interface Column<T> {
  key: string
  header: string
  render?: (value: unknown, row: T) => ReactNode
  width?: string
}

interface DataTableProps<T> {
  columns: Column<T>[]
  data: T[]
  totalRows: number
  pageIndex: number
  pageSize: number
  onPageChange: (page: number) => void
  onPageSizeChange: (size: number) => void
  isLoading: boolean
  onRowClick?: (row: T) => void
}

// eslint-disable-next-line @typescript-eslint/no-explicit-any
export default function DataTable<T extends Record<string, any>>({
  columns,
  data,
  totalRows,
  pageIndex,
  pageSize,
  onPageChange,
  onPageSizeChange,
  isLoading,
  onRowClick,
}: DataTableProps<T>) {
  const totalPages = Math.max(1, Math.ceil(totalRows / pageSize))
  const startRow = pageIndex * pageSize + 1
  const endRow = Math.min((pageIndex + 1) * pageSize, totalRows)

  return (
    <div className="overflow-hidden rounded-lg border border-border bg-surface shadow-sm">
      {/* Table */}
      <div className="overflow-x-auto">
        <table className="w-full text-sm">
          <thead>
            <tr className="border-b border-border bg-gray-50">
              {columns.map((col) => (
                <th
                  key={col.key}
                  className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-text-light"
                  style={col.width ? { width: col.width } : undefined}
                >
                  {col.header}
                </th>
              ))}
            </tr>
          </thead>
          <tbody className="divide-y divide-border">
            {isLoading ? (
              <tr>
                <td colSpan={columns.length} className="px-4 py-12 text-center">
                  <div className="flex items-center justify-center gap-3">
                    <Spinner size="md" />
                    <span className="text-text-light">Cargando...</span>
                  </div>
                </td>
              </tr>
            ) : data.length === 0 ? (
              <tr>
                <td colSpan={columns.length} className="px-4 py-12 text-center text-text-light">
                  No se encontraron registros
                </td>
              </tr>
            ) : (
              data.map((row, rowIndex) => (
                <tr
                  key={(row['id'] as string) || rowIndex}
                  onClick={() => onRowClick?.(row)}
                  className={`transition-colors hover:bg-gray-50 ${
                    onRowClick ? 'cursor-pointer' : ''
                  } ${rowIndex % 2 === 1 ? 'bg-gray-50/50' : ''}`}
                >
                  {columns.map((col) => (
                    <td key={col.key} className="px-4 py-3 text-text">
                      {col.render
                        ? col.render(row[col.key], row)
                        : (row[col.key] as ReactNode) ?? '-'}
                    </td>
                  ))}
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>

      {/* Pagination */}
      <div className="flex flex-col items-center justify-between gap-3 border-t border-border bg-gray-50 px-4 py-3 sm:flex-row">
        <div className="flex items-center gap-2 text-sm text-text-light">
          <span>
            {totalRows > 0
              ? `${startRow}-${endRow} de ${totalRows}`
              : '0 registros'}
          </span>
          <select
            value={pageSize}
            onChange={(e) => onPageSizeChange(Number(e.target.value))}
            className="rounded border border-border bg-white px-2 py-1 text-sm"
          >
            {[10, 25, 50].map((size) => (
              <option key={size} value={size}>
                {size} por pagina
              </option>
            ))}
          </select>
        </div>

        <div className="flex items-center gap-1">
          <button
            onClick={() => onPageChange(pageIndex - 1)}
            disabled={pageIndex === 0}
            className="rounded-lg px-3 py-1.5 text-sm text-text-light hover:bg-white hover:text-text disabled:opacity-40 disabled:cursor-not-allowed transition-colors"
          >
            Anterior
          </button>

          {Array.from({ length: Math.min(totalPages, 5) }, (_, i) => {
            let page: number
            if (totalPages <= 5) {
              page = i
            } else if (pageIndex < 3) {
              page = i
            } else if (pageIndex > totalPages - 4) {
              page = totalPages - 5 + i
            } else {
              page = pageIndex - 2 + i
            }
            return (
              <button
                key={page}
                onClick={() => onPageChange(page)}
                className={`min-w-[36px] rounded-lg px-3 py-1.5 text-sm transition-colors ${
                  page === pageIndex
                    ? 'bg-primary-600 text-white font-medium'
                    : 'text-text-light hover:bg-white hover:text-text'
                }`}
              >
                {page + 1}
              </button>
            )
          })}

          <button
            onClick={() => onPageChange(pageIndex + 1)}
            disabled={pageIndex >= totalPages - 1}
            className="rounded-lg px-3 py-1.5 text-sm text-text-light hover:bg-white hover:text-text disabled:opacity-40 disabled:cursor-not-allowed transition-colors"
          >
            Siguiente
          </button>
        </div>
      </div>
    </div>
  )
}
