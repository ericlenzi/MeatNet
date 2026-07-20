import { useState, useEffect, useCallback } from 'react'
import { useNavigate } from 'react-router'
import { getListasMatanzas, deleteListaMatanza } from '@/services/listasMatanzas.service'
import { useApp } from '@/contexts/AppContext'
import { useToast } from '@/components/ui/Toast'
import { usePagination } from '@/hooks/usePagination'
import { EstadoListaMatanza } from '@/types'
import type { ListaMatanzaListItem } from '@/types'
import DataTable from '@/components/ui/DataTable'
import type { Column, SortState } from '@/components/ui/DataTable'
import PageHeader from '@/components/ui/PageHeader'
import Button from '@/components/ui/Button'
import Badge from '@/components/ui/Badge'
import Select from '@/components/ui/Select'
import ConfirmDialog from '@/components/ui/ConfirmDialog'

const estadoVariant: Record<string, 'success' | 'danger' | 'neutral' | 'info'> = {
  [EstadoListaMatanza.Borrador]: 'neutral',
  [EstadoListaMatanza.Confirmada]: 'info',
  [EstadoListaMatanza.EnEjecucion]: 'info',
  [EstadoListaMatanza.Finalizada]: 'success',
  [EstadoListaMatanza.Anulada]: 'danger',
}

const estadoOptions = [
  { value: '', label: 'Todos los estados' },
  { value: EstadoListaMatanza.Borrador, label: 'Borrador' },
  { value: EstadoListaMatanza.Confirmada, label: 'Confirmada' },
  { value: EstadoListaMatanza.EnEjecucion, label: 'En Ejecucion' },
  { value: EstadoListaMatanza.Finalizada, label: 'Finalizada' },
  { value: EstadoListaMatanza.Anulada, label: 'Anulada' },
]

function formatFecha(value: string): string {
  if (!value) return ''
  return new Date(value).toLocaleDateString('es-AR', { day: '2-digit', month: '2-digit', year: 'numeric' })
}

export default function PlanificacionFaenaListPage() {
  const navigate = useNavigate()
  const { toast } = useToast()
  const { currentEstablecimiento } = useApp()
  const pagination = usePagination()
  const { setTotalRows, setPage } = pagination

  const [estado, setEstado] = useState('')
  const [data, setData] = useState<ListaMatanzaListItem[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [sort, setSort] = useState<SortState | null>(null)
  const [deleteTarget, setDeleteTarget] = useState<ListaMatanzaListItem | null>(null)
  const [isDeleting, setIsDeleting] = useState(false)

  const fetchData = useCallback(async () => {
    setIsLoading(true)
    try {
      const res = await getListasMatanzas({
        PageIndex: pagination.pageIndex,
        PageSize: pagination.pageSize,
        EstablecimientoId: currentEstablecimiento?.id,
        EstadoListaMatanzaId: estado || undefined,
      })
      setData(res.data || [])
      setTotalRows(res.totalRows || 0)
    } catch {
      toast('error', 'Error al cargar listas de matanza')
    } finally {
      setIsLoading(false)
    }
  }, [pagination.pageIndex, pagination.pageSize, currentEstablecimiento?.id, estado, setTotalRows, toast])

  useEffect(() => {
    void fetchData()
  }, [fetchData])

  const handleDelete = async () => {
    if (!deleteTarget) return
    setIsDeleting(true)
    try {
      await deleteListaMatanza(deleteTarget.id)
      toast('success', 'Lista de matanza eliminada')
      setDeleteTarget(null)
      void fetchData()
    } catch (err) {
      toast('error', err instanceof Error ? err.message : 'Error al eliminar')
    } finally {
      setIsDeleting(false)
    }
  }

  const columns: Column<ListaMatanzaListItem>[] = [
    { key: 'numeroLista', header: 'N°', width: '80px', sortable: true },
    { key: 'fecha', header: 'Fecha', width: '120px', render: (_, row) => formatFecha(row.fecha) },
    { key: 'especieNombre', header: 'Especie', width: '130px' },
    { key: 'puestoCodigo', header: 'Puesto', width: '110px', render: (v) => (v ? String(v) : '—') },
    { key: 'totalRenglones', header: 'Renglones', width: '110px', render: (v) => <span className="font-mono">{String(v)}</span> },
    { key: 'totalCabezas', header: 'Cabezas', width: '100px', render: (v) => <span className="font-mono">{String(v)}</span> },
    { key: 'version', header: 'Ver.', width: '70px', render: (v) => <span className="font-mono">{String(v)}</span> },
    {
      key: 'estadoListaMatanzaNombre',
      header: 'Estado',
      width: '150px',
      render: (_, row) => (
        <Badge variant={estadoVariant[row.estadoListaMatanzaId] ?? 'neutral'}>{row.estadoListaMatanzaNombre}</Badge>
      ),
    },
    {
      key: '_actions',
      header: '',
      width: '110px',
      render: (_, row) => {
        const esBorrador = row.estadoListaMatanzaId === EstadoListaMatanza.Borrador
        return (
          <div className="flex items-center gap-1">
            <button
              onClick={(e) => { e.stopPropagation(); navigate(`/operaciones/planificacion-faena/${row.id}`) }}
              className="rounded p-1.5 text-text-light hover:bg-primary-50 hover:text-primary-600 transition-colors"
              title="Ver detalle"
            >
              <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
              </svg>
            </button>
            {esBorrador && (
              <>
                <button
                  onClick={(e) => { e.stopPropagation(); navigate(`/operaciones/planificacion-faena/${row.id}/edit`) }}
                  className="rounded p-1.5 text-text-light hover:bg-primary-50 hover:text-primary-600 transition-colors"
                  title="Editar"
                >
                  <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
                  </svg>
                </button>
                <button
                  onClick={(e) => { e.stopPropagation(); setDeleteTarget(row) }}
                  className="rounded p-1.5 text-text-light hover:bg-red-50 hover:text-danger transition-colors"
                  title="Eliminar"
                >
                  <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                  </svg>
                </button>
              </>
            )}
          </div>
        )
      },
    },
  ]

  return (
    <>
      <PageHeader title="Planificacion de Faena">
        <Button onClick={() => navigate('/operaciones/planificacion-faena/create')}>Nueva Lista de Matanza</Button>
      </PageHeader>

      <div className="mb-4 flex flex-wrap items-center gap-3">
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
        title="Eliminar lista de matanza"
        message={`¿Esta seguro que desea eliminar la lista N° ${deleteTarget?.numeroLista}?`}
        isLoading={isDeleting}
      />
    </>
  )
}
