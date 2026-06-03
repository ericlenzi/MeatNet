import { useState, useEffect, useCallback } from 'react'
import { useNavigate } from 'react-router'
import { getUsuarios, deleteUsuario, restaurarPasswordUsuario } from '@/services/usuarios.service'
import { usePagination } from '@/hooks/usePagination'
import { useDebounce } from '@/hooks/useDebounce'
import { useToast } from '@/components/ui/Toast'
import { useAuth } from '@/contexts/AuthContext'
import type { Usuario } from '@/types'
import DataTable from '@/components/ui/DataTable'
import type { Column, SortState } from '@/components/ui/DataTable'
import SearchInput from '@/components/ui/SearchInput'
import StatusFilter from '@/components/ui/StatusFilter'
import PageHeader from '@/components/ui/PageHeader'
import Button from '@/components/ui/Button'
import Badge from '@/components/ui/Badge'
import ConfirmDialog from '@/components/ui/ConfirmDialog'

export default function UsuariosListPage() {
  const navigate = useNavigate()
  const { toast } = useToast()
  const { isAdmin } = useAuth()
  const pagination = usePagination()
  const debouncedFilter = useDebounce(pagination.filter)
  const [statusFilter, setStatusFilter] = useState('active')
  const [data, setData] = useState<Usuario[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [sort, setSort] = useState<SortState | null>(null)
  const [deleteTarget, setDeleteTarget] = useState<Usuario | null>(null)
  const [isDeleting, setIsDeleting] = useState(false)
  const [restaurarTarget, setRestaurarTarget] = useState<Usuario | null>(null)
  const [isRestaurando, setIsRestaurando] = useState(false)

  const fetchData = useCallback(async () => {
    setIsLoading(true)
    try {
      const response = await getUsuarios({
        Filter: debouncedFilter || undefined,
        Estado: statusFilter === 'active' ? 1 : statusFilter === 'inactive' ? 0 : undefined,
        PageIndex: pagination.pageIndex,
        PageSize: pagination.pageSize,
      })
      setData(response.data || [])
      pagination.setTotalRows(response.totalRows)
    } catch {
      toast('error', 'Error al cargar usuarios')
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
      await deleteUsuario(deleteTarget.id)
      toast('success', 'Usuario eliminado')
      setDeleteTarget(null)
      void fetchData()
    } catch (err) {
      toast('error', err instanceof Error ? err.message : 'Error al eliminar')
    } finally {
      setIsDeleting(false)
    }
  }

  const handleRestaurarPassword = async () => {
    if (!restaurarTarget) return
    setIsRestaurando(true)
    try {
      await restaurarPasswordUsuario(restaurarTarget.id)
      toast('success', `Contraseña de "${restaurarTarget.userName}" restaurada a la contraseña inicial`)
      setRestaurarTarget(null)
    } catch (err) {
      toast('error', err instanceof Error ? err.message : 'Error al restaurar contraseña')
    } finally {
      setIsRestaurando(false)
    }
  }

  const columns: Column<Usuario>[] = [
    { key: 'userName', header: 'Usuario', width: '130px', sortable: true },
    { key: 'nombre', header: 'Nombre', sortable: true },
    { key: 'apellido', header: 'Apellido', sortable: true },
    { key: 'email', header: 'Email', sortable: true },
    { key: 'rolId', header: 'Rol', width: '120px', sortable: true },
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
      width: '120px',
      render: (_, row) => (
        <div className="flex items-center gap-1">
          <button
            onClick={(e) => { e.stopPropagation(); navigate(`/usuarios/${row.id}/edit`) }}
            className="rounded p-1.5 text-text-light hover:bg-primary-50 hover:text-primary-600 transition-colors"
            title="Editar"
          >
            <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
            </svg>
          </button>
          {isAdmin && (
            <button
              onClick={(e) => { e.stopPropagation(); setRestaurarTarget(row) }}
              className="rounded p-1.5 text-text-light hover:bg-amber-50 hover:text-amber-600 transition-colors"
              title="Restaurar contraseña inicial"
            >
              <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 7a2 2 0 012 2m4 0a6 6 0 01-7.743 5.743L11 17H9v2H7v2H4a1 1 0 01-1-1v-2.586a1 1 0 01.293-.707l5.964-5.964A6 6 0 1121 9z" />
              </svg>
            </button>
          )}
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
      <PageHeader title="Usuarios">
        <Button onClick={() => navigate('/usuarios/create')}>Nuevo Usuario</Button>
      </PageHeader>

      <div className="mb-4 flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <SearchInput
          value={pagination.filter}
          onChange={pagination.setFilter}
          placeholder="Buscar por nombre, usuario, email..."
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
        title="Eliminar usuario"
        message={`¿Esta seguro que desea eliminar al usuario "${deleteTarget?.userName}"?`}
        isLoading={isDeleting}
      />

      <ConfirmDialog
        isOpen={!!restaurarTarget}
        onConfirm={handleRestaurarPassword}
        onCancel={() => setRestaurarTarget(null)}
        title="Restaurar contraseña"
        message={`¿Esta seguro que desea restaurar la contraseña de "${restaurarTarget?.userName}" a la contraseña inicial?`}
        isLoading={isRestaurando}
      />
    </>
  )
}
