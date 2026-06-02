import { useState, useEffect, useCallback } from 'react'
import { useNavigate } from 'react-router'
import { getEmpresas, deleteEmpresa } from '@/services/empresas.service'
import { usePagination } from '@/hooks/usePagination'
import { useDebounce } from '@/hooks/useDebounce'
import { useToast } from '@/components/ui/Toast'
import type { Empresa } from '@/types'
import DataTable from '@/components/ui/DataTable'
import type { Column, SortState } from '@/components/ui/DataTable'
import SearchInput from '@/components/ui/SearchInput'
import StatusFilter from '@/components/ui/StatusFilter'
import PageHeader from '@/components/ui/PageHeader'
import Button from '@/components/ui/Button'
import Badge from '@/components/ui/Badge'
import ConfirmDialog from '@/components/ui/ConfirmDialog'

export default function EmpresasListPage() {
  const navigate = useNavigate()
  const { toast } = useToast()
  const pagination = usePagination()
  const debouncedFilter = useDebounce(pagination.filter)
  const [statusFilter, setStatusFilter] = useState('active')
  const [data, setData] = useState<Empresa[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [sort, setSort] = useState<SortState | null>(null)
  const [deleteTarget, setDeleteTarget] = useState<Empresa | null>(null)
  const [isDeleting, setIsDeleting] = useState(false)

  const fetchData = useCallback(async () => {
    setIsLoading(true)
    try {
      const response = await getEmpresas({
        Filter: debouncedFilter || undefined,
        Estado: statusFilter === 'active' ? true : statusFilter === 'inactive' ? false : undefined,
        PageIndex: pagination.pageIndex,
        PageSize: pagination.pageSize,
      })
      setData(response.data || [])
      pagination.setTotalRows(response.totalRows)
    } catch {
      toast('error', 'Error al cargar empresas')
    } finally {
      setIsLoading(false)
    }
  }, [debouncedFilter, statusFilter, pagination.pageIndex, pagination.pageSize]) // eslint-disable-line react-hooks/exhaustive-deps

  useEffect(() => {
    void fetchData()
  }, [fetchData])

  const handleStatusChange = (value: string) => {
    setStatusFilter(value)
    pagination.setPage(0)
  }

  const handleDelete = async () => {
    if (!deleteTarget) return
    setIsDeleting(true)
    try {
      await deleteEmpresa(deleteTarget.id)
      toast('success', 'Empresa eliminada')
      setDeleteTarget(null)
      void fetchData()
    } catch (err) {
      toast('error', err instanceof Error ? err.message : 'Error al eliminar')
    } finally {
      setIsDeleting(false)
    }
  }

  const columns: Column<Empresa>[] = [
    { key: 'codigoEmpresa', header: 'Codigo', width: '120px', sortable: true },
    { key: 'nombre', header: 'Nombre', sortable: true },
    { key: 'tipoEmpresaId', header: 'Tipo', width: '100px', sortable: true },
    { key: 'numeroCuit', header: 'CUIT', width: '150px', sortable: true },
    {
      key: 'activo',
      header: 'Estado',
      width: '100px',
      sortable: true,
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
            onClick={(e) => { e.stopPropagation(); navigate(`/empresas/${row.id}/edit`) }}
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
      <PageHeader title="Empresas">
        <Button onClick={() => navigate('/empresas/create')}>Nueva Empresa</Button>
      </PageHeader>

      <div className="mb-4 flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <SearchInput
          value={pagination.filter}
          onChange={pagination.setFilter}
          placeholder="Buscar por nombre, codigo..."
        />
        <StatusFilter value={statusFilter} onChange={handleStatusChange} />
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
        title="Eliminar empresa"
        message={`¿Esta seguro que desea eliminar la empresa "${deleteTarget?.nombre}"?`}
        isLoading={isDeleting}
      />
    </>
  )
}
