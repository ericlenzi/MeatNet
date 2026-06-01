import { useState, useEffect, useCallback } from 'react'
import { useNavigate } from 'react-router'
import { getEstablecimientos, deleteEstablecimiento } from '@/services/establecimientos.service'
import { usePagination } from '@/hooks/usePagination'
import { useDebounce } from '@/hooks/useDebounce'
import { useToast } from '@/components/ui/Toast'
import type { Establecimiento } from '@/types'
import DataTable from '@/components/ui/DataTable'
import type { Column } from '@/components/ui/DataTable'
import SearchInput from '@/components/ui/SearchInput'
import PageHeader from '@/components/ui/PageHeader'
import Button from '@/components/ui/Button'
import Badge from '@/components/ui/Badge'
import ConfirmDialog from '@/components/ui/ConfirmDialog'

export default function EstablecimientosListPage() {
  const navigate = useNavigate()
  const { toast } = useToast()
  const pagination = usePagination()
  const debouncedFilter = useDebounce(pagination.filter)
  const [data, setData] = useState<Establecimiento[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [deleteTarget, setDeleteTarget] = useState<Establecimiento | null>(null)
  const [isDeleting, setIsDeleting] = useState(false)

  const fetchData = useCallback(async () => {
    setIsLoading(true)
    try {
      const response = await getEstablecimientos({
        Filter: debouncedFilter || undefined,
        PageIndex: pagination.pageIndex,
        PageSize: pagination.pageSize,
      })
      setData(response.data || [])
      pagination.setTotalRows(response.totalRows)
    } catch {
      toast('error', 'Error al cargar establecimientos')
    } finally {
      setIsLoading(false)
    }
  }, [debouncedFilter, pagination.pageIndex, pagination.pageSize]) // eslint-disable-line react-hooks/exhaustive-deps

  useEffect(() => {
    void fetchData()
  }, [fetchData])

  const handleDelete = async () => {
    if (!deleteTarget) return
    setIsDeleting(true)
    try {
      await deleteEstablecimiento(deleteTarget.id)
      toast('success', 'Establecimiento eliminado')
      setDeleteTarget(null)
      void fetchData()
    } catch (err) {
      toast('error', err instanceof Error ? err.message : 'Error al eliminar')
    } finally {
      setIsDeleting(false)
    }
  }

  const columns: Column<Establecimiento>[] = [
    { key: 'codigoEstablecimiento', header: 'Codigo', width: '120px' },
    { key: 'nombre', header: 'Nombre' },
    { key: 'sucursalNombre', header: 'Sucursal', width: '150px' },
    { key: 'especieNombre', header: 'Especie', width: '120px' },
    { key: 'numeroSenasa', header: 'SENASA', width: '120px' },
    {
      key: 'activo',
      header: 'Estado',
      width: '100px',
      render: (value) => (
        <Badge variant={value ? 'success' : 'danger'}>
          {value ? 'Activo' : 'Inactivo'}
        </Badge>
      ),
    },
    {
      key: '_actions',
      header: '',
      width: '100px',
      render: (_, row) => (
        <div className="flex items-center gap-1">
          <button
            onClick={(e) => { e.stopPropagation(); navigate(`/establecimientos/${row.id}/edit`) }}
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

  return (
    <>
      <PageHeader title="Establecimientos">
        <Button onClick={() => navigate('/establecimientos/create')}>Nuevo Establecimiento</Button>
      </PageHeader>

      <div className="mb-4">
        <SearchInput
          value={pagination.filter}
          onChange={pagination.setFilter}
          placeholder="Buscar por nombre, codigo..."
        />
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
      />

      <ConfirmDialog
        isOpen={!!deleteTarget}
        onConfirm={handleDelete}
        onCancel={() => setDeleteTarget(null)}
        title="Eliminar establecimiento"
        message={`¿Esta seguro que desea eliminar el establecimiento "${deleteTarget?.nombre}"?`}
        isLoading={isDeleting}
      />
    </>
  )
}
