import { useState, useEffect, useCallback } from 'react'
import { useNavigate } from 'react-router'
import {
  getRendimientosSubproductos,
  deleteRendimientoSubproducto,
} from '@/services/rendimientosSubproductos.service'
import { getEspecies } from '@/services/especies.service'
import { usePagination } from '@/hooks/usePagination'
import { useDebounce } from '@/hooks/useDebounce'
import { useToast } from '@/components/ui/Toast'
import type { RendimientoSubproducto, Especie } from '@/types'
import DataTable from '@/components/ui/DataTable'
import type { Column, SortState } from '@/components/ui/DataTable'
import SearchInput from '@/components/ui/SearchInput'
import StatusFilter from '@/components/ui/StatusFilter'
import PageHeader from '@/components/ui/PageHeader'
import Button from '@/components/ui/Button'
import Badge from '@/components/ui/Badge'
import ConfirmDialog from '@/components/ui/ConfirmDialog'
import Select from '@/components/ui/Select'

export default function RendimientosSubproductosListPage() {
  const navigate = useNavigate()
  const { toast } = useToast()
  const pagination = usePagination()
  const debouncedFilter = useDebounce(pagination.filter)
  const [statusFilter, setStatusFilter] = useState('active')
  const [especieFilter, setEspecieFilter] = useState('')
  const [data, setData] = useState<RendimientoSubproducto[]>([])
  const [especies, setEspecies] = useState<Especie[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [sort, setSort] = useState<SortState | null>(null)
  const [deleteTarget, setDeleteTarget] = useState<RendimientoSubproducto | null>(null)
  const [isDeleting, setIsDeleting] = useState(false)

  useEffect(() => {
    getEspecies({ PageSize: 1000, Estado: true })
      .then((res) => setEspecies(res.data || []))
      .catch(() => {})
  }, [])

  const fetchData = useCallback(async () => {
    setIsLoading(true)
    try {
      const response = await getRendimientosSubproductos({
        Filter: debouncedFilter || undefined,
        Estado: statusFilter === 'active' ? true : statusFilter === 'inactive' ? false : undefined,
        EspecieId: especieFilter || undefined,
        PageIndex: pagination.pageIndex,
        PageSize: pagination.pageSize,
      })
      setData(response.data || [])
      pagination.setTotalRows(response.totalRows)
    } catch {
      toast('error', 'Error al cargar los rendimientos')
    } finally {
      setIsLoading(false)
    }
  }, [debouncedFilter, statusFilter, especieFilter, pagination.pageIndex, pagination.pageSize]) // eslint-disable-line react-hooks/exhaustive-deps

  useEffect(() => {
    void fetchData()
  }, [fetchData])

  const handleFilterChange = (setter: (v: string) => void) => (value: string) => {
    setter(value)
    pagination.setPage(0)
  }

  const handleDelete = async () => {
    if (!deleteTarget) return
    setIsDeleting(true)
    try {
      await deleteRendimientoSubproducto(deleteTarget.id)
      toast('success', 'Rendimiento eliminado')
      setDeleteTarget(null)
      void fetchData()
    } catch (err) {
      toast('error', err instanceof Error ? err.message : 'Error al eliminar')
    } finally {
      setIsDeleting(false)
    }
  }

  const columns: Column<RendimientoSubproducto>[] = [
    { key: 'especieNombre', header: 'Especie', width: '140px' },
    { key: 'materialCodigo', header: 'Codigo', width: '110px' },
    { key: 'materialNombre', header: 'Subproducto' },
    {
      key: 'porcentaje',
      header: '% del peso de la res',
      width: '170px',
      render: (value) => <span className="font-mono">{Number(value).toFixed(2)}%</span>,
    },
    {
      key: 'activo',
      header: 'Estado',
      width: '100px',
      render: (value) => (
        <Badge variant={value ? 'success' : 'danger'}>{value ? 'Activo' : 'Inactivo'}</Badge>
      ),
    },
    {
      key: '_actions',
      header: '',
      width: '100px',
      render: (_, row) => (
        <div className="flex items-center gap-1">
          <button
            onClick={(e) => { e.stopPropagation(); navigate(`/rendimientos-subproductos/${row.id}/edit`) }}
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
      <PageHeader title="Rendimientos de Subproductos">
        <Button onClick={() => navigate('/rendimientos-subproductos/create')}>Nuevo Rendimiento</Button>
      </PageHeader>

      <p className="mb-4 text-sm text-text-light">
        Cuánto subproducto deja un animal, en porcentaje del <strong>peso de la res faenada</strong>.
        Con esto el Análisis de Faena estima la producción de cuero, sebo y menudencias de cada
        jornada. Es una estimación: no genera existencia, porque esos kilos no se pesaron.
      </p>

      <div className="mb-4 flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <SearchInput
          value={pagination.filter}
          onChange={pagination.setFilter}
          placeholder="Buscar por subproducto..."
        />
        <div className="flex items-center gap-2">
          <Select
            value={especieFilter}
            onChange={(e) => handleFilterChange(setEspecieFilter)(e.target.value)}
            options={[
              { value: '', label: 'Todas las especies' },
              ...especies.map((e) => ({ value: e.codigo, label: e.nombre })),
            ]}
          />
          <StatusFilter value={statusFilter} onChange={handleFilterChange(setStatusFilter)} />
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
        title="Eliminar rendimiento"
        message={`\u00bfEsta seguro que desea eliminar el rendimiento de "${deleteTarget?.materialNombre}"?`}
        isLoading={isDeleting}
      />
    </>
  )
}
