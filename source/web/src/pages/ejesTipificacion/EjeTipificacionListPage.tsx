import { useState, useEffect, useCallback } from 'react'
import { useNavigate } from 'react-router'
import { getEjes, deleteEje, ETIQUETAS } from '@/services/ejesTipificacion.service'
import type { EjeTipificacionId } from '@/services/ejesTipificacion.service'
import { getEspecies } from '@/services/especies.service'
import { usePagination } from '@/hooks/usePagination'
import { useDebounce } from '@/hooks/useDebounce'
import { useToast } from '@/components/ui/Toast'
import type { EjeTipificacion, Especie } from '@/types'
import DataTable from '@/components/ui/DataTable'
import type { Column, SortState } from '@/components/ui/DataTable'
import SearchInput from '@/components/ui/SearchInput'
import StatusFilter from '@/components/ui/StatusFilter'
import PageHeader from '@/components/ui/PageHeader'
import Button from '@/components/ui/Button'
import Badge from '@/components/ui/Badge'
import ConfirmDialog from '@/components/ui/ConfirmDialog'
import Select from '@/components/ui/Select'

// Una sola pantalla para los dos ejes de la tipificacion oficial: comparten forma y reglas, y lo
// unico que cambia es el catalogo del que salen. La ruta decide cual via la prop.
export default function EjeTipificacionListPage({ eje }: { eje: EjeTipificacionId }) {
  const navigate = useNavigate()
  const { toast } = useToast()
  const pagination = usePagination()
  const debouncedFilter = useDebounce(pagination.filter)
  const [statusFilter, setStatusFilter] = useState('active')
  const [especieFilter, setEspecieFilter] = useState('')
  const [data, setData] = useState<EjeTipificacion[]>([])
  const [especies, setEspecies] = useState<Especie[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [sort, setSort] = useState<SortState | null>(null)
  const [deleteTarget, setDeleteTarget] = useState<EjeTipificacion | null>(null)
  const [isDeleting, setIsDeleting] = useState(false)

  const etiqueta = ETIQUETAS[eje]

  useEffect(() => {
    getEspecies({ PageSize: 1000, Estado: true })
      .then((res) => setEspecies(res.data || []))
      .catch(() => {})
  }, [])

  const fetchData = useCallback(async () => {
    setIsLoading(true)
    try {
      const response = await getEjes(eje, {
        Filter: debouncedFilter || undefined,
        Estado: statusFilter === 'active' ? true : statusFilter === 'inactive' ? false : undefined,
        EspecieId: especieFilter || undefined,
        PageIndex: pagination.pageIndex,
        PageSize: pagination.pageSize,
      })
      setData(response.data || [])
      pagination.setTotalRows(response.totalRows)
    } catch {
      toast('error', 'Error al cargar ' + etiqueta.plural.toLowerCase())
    } finally {
      setIsLoading(false)
    }
  }, [eje, debouncedFilter, statusFilter, especieFilter, pagination.pageIndex, pagination.pageSize]) // eslint-disable-line react-hooks/exhaustive-deps

  useEffect(() => {
    // Al cambiar de eje se vuelve a la primera pagina: son catalogos distintos.
    pagination.setPage(0)
  }, [eje]) // eslint-disable-line react-hooks/exhaustive-deps

  useEffect(() => {
    void fetchData()
  }, [fetchData])

  const handleDelete = async () => {
    if (!deleteTarget) return
    setIsDeleting(true)
    try {
      await deleteEje(eje, deleteTarget.codigo)
      toast('success', etiqueta.singular + ' eliminada')
      setDeleteTarget(null)
      void fetchData()
    } catch (err) {
      toast('error', err instanceof Error ? err.message : 'Error al eliminar')
    } finally {
      setIsDeleting(false)
    }
  }

  const columns: Column<EjeTipificacion>[] = [
    { key: 'orden', header: 'Orden', width: '80px' },
    { key: 'codigo', header: 'Codigo', width: '120px' },
    { key: 'nombre', header: 'Nombre' },
    {
      key: 'especieId',
      header: 'Especie',
      width: '140px',
      render: (value) => especies.find((e) => e.codigo === value)?.nombre ?? String(value ?? ''),
    },
    {
      key: 'activo',
      header: 'Estado',
      width: '90px',
      render: (value) => (
        <Badge variant={value ? 'success' : 'danger'}>{value ? 'Activo' : 'Inactivo'}</Badge>
      ),
    },
    {
      key: '_actions',
      header: '',
      width: '90px',
      render: (_, row) => (
        <div className="flex items-center gap-1">
          <button
            onClick={(e) => { e.stopPropagation(); navigate('/' + eje + '/' + row.codigo + '/edit') }}
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
        </div>
      ),
    },
  ]

  const mensajeBaja =
    'Confirma que desea eliminar "' + (deleteTarget?.nombre ?? '') + '"? Si ya se uso en romaneos, desactivela en lugar de eliminarla.'

  return (
    <>
      <PageHeader title={etiqueta.plural}>
        <Button onClick={() => navigate('/' + eje + '/create')}>Nuevo</Button>
      </PageHeader>

      <div className="mb-4 flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <SearchInput
          value={pagination.filter}
          onChange={pagination.setFilter}
          placeholder="Buscar por codigo, nombre..."
        />
        <div className="flex items-center gap-2">
          <Select
            value={especieFilter}
            onChange={(e) => { setEspecieFilter(e.target.value); pagination.setPage(0) }}
            options={[
              { value: '', label: 'Todas las especies' },
              ...especies.map((e) => ({ value: e.codigo, label: e.nombre })),
            ]}
          />
          <StatusFilter value={statusFilter} onChange={(v) => { setStatusFilter(v); pagination.setPage(0) }} />
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
        title={'Eliminar ' + etiqueta.singular.toLowerCase()}
        message={mensajeBaja}
        isLoading={isDeleting}
      />
    </>
  )
}
