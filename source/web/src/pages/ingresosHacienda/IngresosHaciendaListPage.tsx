import { useState, useEffect, useCallback } from 'react'
import { useNavigate } from 'react-router'
import {
  getIngresosHaciendas,
  deleteIngresoHacienda,
} from '@/services/ingresosHacienda.service'
import { useApp } from '@/contexts/AppContext'
import { useToast } from '@/components/ui/Toast'
import { useDebounce } from '@/hooks/useDebounce'
import { usePagination } from '@/hooks/usePagination'
import { EstadoIngreso } from '@/types'
import type { IngresoHaciendaListItem } from '@/types'
import DataTable from '@/components/ui/DataTable'
import type { Column, SortState } from '@/components/ui/DataTable'
import PageHeader from '@/components/ui/PageHeader'
import Button from '@/components/ui/Button'
import Badge from '@/components/ui/Badge'
import Select from '@/components/ui/Select'
import SearchInput from '@/components/ui/SearchInput'
import ConfirmDialog from '@/components/ui/ConfirmDialog'

const estadoVariant: Record<string, 'success' | 'danger' | 'neutral' | 'info'> = {
  [EstadoIngreso.Borrador]: 'neutral',
  [EstadoIngreso.PendienteAprobacion]: 'info',
  [EstadoIngreso.Aprobado]: 'success',
  [EstadoIngreso.Anulado]: 'danger',
}

const estadoOptions = [
  { value: '', label: 'Todos los estados' },
  { value: EstadoIngreso.Borrador, label: 'Borrador' },
  { value: EstadoIngreso.PendienteAprobacion, label: 'Pendiente Aprobacion' },
  { value: EstadoIngreso.Aprobado, label: 'Aprobado' },
  { value: EstadoIngreso.Anulado, label: 'Anulado' },
]

function formatFecha(value: string): string {
  if (!value) return ''
  return new Date(value).toLocaleString('es-AR', { dateStyle: 'short', timeStyle: 'short' })
}

export default function IngresosHaciendaListPage() {
  const navigate = useNavigate()
  const { toast } = useToast()
  const { currentEstablecimiento } = useApp()
  const pagination = usePagination()
  const debouncedFilter = useDebounce(pagination.filter)
  const { setTotalRows, setPage } = pagination

  const [estado, setEstado] = useState('')
  const [data, setData] = useState<IngresoHaciendaListItem[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [sort, setSort] = useState<SortState | null>(null)
  const [deleteTarget, setDeleteTarget] = useState<IngresoHaciendaListItem | null>(null)
  const [isDeleting, setIsDeleting] = useState(false)

  const fetchData = useCallback(async () => {
    setIsLoading(true)
    try {
      const res = await getIngresosHaciendas({
        Filter: debouncedFilter || undefined,
        PageIndex: pagination.pageIndex,
        PageSize: pagination.pageSize,
        EstablecimientoId: currentEstablecimiento?.id,
        EstadoIngresoId: estado || undefined,
      })
      setData(res.data || [])
      setTotalRows(res.totalRows || 0)
    } catch {
      toast('error', 'Error al cargar ingresos de hacienda')
    } finally {
      setIsLoading(false)
    }
  }, [debouncedFilter, pagination.pageIndex, pagination.pageSize, currentEstablecimiento?.id, estado, setTotalRows, toast])

  useEffect(() => {
    void fetchData()
  }, [fetchData])

  const handleDelete = async () => {
    if (!deleteTarget) return
    setIsDeleting(true)
    try {
      await deleteIngresoHacienda(deleteTarget.id)
      toast('success', 'Ingreso eliminado')
      setDeleteTarget(null)
      void fetchData()
    } catch (err) {
      toast('error', err instanceof Error ? err.message : 'Error al eliminar')
    } finally {
      setIsDeleting(false)
    }
  }

  const columns: Column<IngresoHaciendaListItem>[] = [
    { key: 'numeroIngreso', header: 'N°', width: '80px', sortable: true },
    { key: 'fechaHoraIngreso', header: 'Fecha Ingreso', width: '160px', render: (v) => formatFecha(String(v)) },
    { key: 'numeroDte', header: 'DT-e', width: '130px' },
    { key: 'clienteNombre', header: 'Cliente', sortable: true },
    { key: 'totalCabezas', header: 'Cabezas', width: '100px', render: (v) => <span className="font-mono">{String(v)}</span> },
    { key: 'pesoNeto', header: 'Peso Neto (kg)', width: '130px', render: (v) => <span className="font-mono">{Number(v).toLocaleString('es-AR')}</span> },
    {
      key: 'estadoIngresoNombre',
      header: 'Estado',
      width: '150px',
      render: (_, row) => (
        <Badge variant={estadoVariant[row.estadoIngresoId] ?? 'neutral'}>{row.estadoIngresoNombre}</Badge>
      ),
    },
    {
      key: '_actions',
      header: '',
      width: '100px',
      render: (_, row) => (
        <div className="flex items-center gap-1">
          <button
            onClick={(e) => { e.stopPropagation(); navigate(`/operaciones/ingreso-hacienda/${row.id}/edit`) }}
            className="rounded p-1.5 text-text-light hover:bg-primary-50 hover:text-primary-600 transition-colors"
            title={row.estadoIngresoId === EstadoIngreso.Borrador ? 'Editar' : 'Ver'}
          >
            <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
            </svg>
          </button>
          {row.estadoIngresoId === EstadoIngreso.Borrador && (
            <button
              onClick={(e) => { e.stopPropagation(); setDeleteTarget(row) }}
              className="rounded p-1.5 text-text-light hover:bg-red-50 hover:text-danger transition-colors"
              title="Eliminar"
            >
              <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
              </svg>
            </button>
          )}
        </div>
      ),
    },
  ]

  return (
    <>
      <PageHeader title="Ingreso de Hacienda">
        <Button onClick={() => navigate('/operaciones/ingreso-hacienda/create')}>Nuevo Ingreso</Button>
      </PageHeader>

      <div className="mb-4 flex flex-wrap items-center gap-3">
        <SearchInput value={pagination.filter} onChange={pagination.setFilter} placeholder="Buscar por DT-e o cliente..." />
        <div className="w-56">
          <Select
            options={estadoOptions}
            value={estado}
            onChange={(e) => { setEstado(e.target.value); setPage(0) }}
          />
        </div>
      </div>

      <DataTable
        columns={columns}
        data={data}
        totalRows={pagination.totalRows}
        pageIndex={pagination.pageIndex}
        pageSize={pagination.pageSize}
        onPageChange={pagination.setPage}
        onPageSizeChange={pagination.setPageSize}
        isLoading={isLoading}
        sort={sort}
        onSortChange={setSort}
      />

      <ConfirmDialog
        isOpen={!!deleteTarget}
        onConfirm={handleDelete}
        onCancel={() => setDeleteTarget(null)}
        title="Eliminar ingreso"
        message={`¿Esta seguro que desea eliminar el ingreso N° ${deleteTarget?.numeroIngreso}?`}
        isLoading={isDeleting}
      />
    </>
  )
}
