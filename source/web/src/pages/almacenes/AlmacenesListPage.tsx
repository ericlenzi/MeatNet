import { useState, useEffect, useCallback } from 'react'
import { useNavigate } from 'react-router'
import { getAlmacenes, deleteAlmacen, getTiposAlmacenes } from '@/services/almacenes.service'
import type { AlmacenItem, TipoAlmacenOption } from '@/services/almacenes.service'
import { useApp } from '@/contexts/AppContext'
import { useToast } from '@/components/ui/Toast'
import DataTable from '@/components/ui/DataTable'
import type { Column, SortState } from '@/components/ui/DataTable'
import PageHeader from '@/components/ui/PageHeader'
import Button from '@/components/ui/Button'
import Badge from '@/components/ui/Badge'
import ConfirmDialog from '@/components/ui/ConfirmDialog'

export default function AlmacenesListPage() {
  const navigate = useNavigate()
  const { toast } = useToast()
  const { currentEstablecimiento } = useApp()
  const [data, setData] = useState<AlmacenItem[]>([])
  const [tipos, setTipos] = useState<TipoAlmacenOption[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [sort, setSort] = useState<SortState | null>(null)
  const [deleteTarget, setDeleteTarget] = useState<AlmacenItem | null>(null)
  const [isDeleting, setIsDeleting] = useState(false)

  const tipoNombre = (codigo: string) => tipos.find((t) => t.codigo === codigo)?.nombre ?? codigo

  const fetchData = useCallback(async () => {
    setIsLoading(true)
    try {
      const [items, ts] = await Promise.all([
        getAlmacenes({ EstablecimientoId: currentEstablecimiento?.id }),
        getTiposAlmacenes(),
      ])
      setData(items)
      setTipos(ts)
    } catch {
      toast('error', 'Error al cargar almacenes')
    } finally {
      setIsLoading(false)
    }
  }, [currentEstablecimiento?.id, toast])

  useEffect(() => {
    void fetchData()
  }, [fetchData])

  const handleDelete = async () => {
    if (!deleteTarget) return
    setIsDeleting(true)
    try {
      await deleteAlmacen(deleteTarget.id)
      toast('success', 'Almacen eliminado')
      setDeleteTarget(null)
      void fetchData()
    } catch (err) {
      toast('error', err instanceof Error ? err.message : 'Error al eliminar')
    } finally {
      setIsDeleting(false)
    }
  }

  const columns: Column<AlmacenItem>[] = [
    { key: 'codigoAlmacen', header: 'Codigo', width: '140px', sortable: true },
    { key: 'nombre', header: 'Nombre', sortable: true },
    { key: 'tipoAlmacenId', header: 'Tipo', width: '180px', render: (v) => tipoNombre(String(v)) },
    { key: 'capacidad', header: 'Capacidad', width: '120px', render: (v) => <span className="font-mono">{String(v)}</span> },
    {
      key: 'activo',
      header: 'Estado',
      width: '100px',
      sortable: true,
      render: (v) => <Badge variant={v ? 'success' : 'danger'}>{v ? 'Activo' : 'Inactivo'}</Badge>,
    },
    {
      key: '_actions',
      header: '',
      width: '100px',
      render: (_, row) => (
        <div className="flex items-center gap-1">
          <button
            onClick={(e) => { e.stopPropagation(); navigate(`/almacenes/${row.id}/edit`) }}
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
      <PageHeader title="Almacenes">
        <Button onClick={() => navigate('/almacenes/create')}>Nuevo Almacen</Button>
      </PageHeader>

      <DataTable
        columns={columns}
        data={data}
        totalRows={data.length}
        pageIndex={0}
        pageSize={data.length || 10}
        onPageChange={() => {}}
        onPageSizeChange={() => {}}
        isLoading={isLoading}
        sort={sort}
        onSortChange={setSort}
      />

      <ConfirmDialog
        isOpen={!!deleteTarget}
        onConfirm={handleDelete}
        onCancel={() => setDeleteTarget(null)}
        title="Eliminar almacen"
        message={`¿Esta seguro que desea eliminar el almacen ${deleteTarget?.codigoAlmacen} - ${deleteTarget?.nombre}?`}
        isLoading={isDeleting}
      />
    </>
  )
}
