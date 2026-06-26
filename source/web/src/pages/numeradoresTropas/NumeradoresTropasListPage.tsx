import { useState, useEffect, useCallback } from 'react'
import { useNavigate } from 'react-router'
import { getNumeradoresTropas, deleteNumeradorTropa } from '@/services/numeradoresTropas.service'
import { useToast } from '@/components/ui/Toast'
import type { NumeradorTropa } from '@/types'
import DataTable from '@/components/ui/DataTable'
import type { Column, SortState } from '@/components/ui/DataTable'
import PageHeader from '@/components/ui/PageHeader'
import Button from '@/components/ui/Button'
import ConfirmDialog from '@/components/ui/ConfirmDialog'

export default function NumeradoresTropasListPage() {
  const navigate = useNavigate()
  const { toast } = useToast()
  const [data, setData] = useState<NumeradorTropa[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [sort, setSort] = useState<SortState | null>(null)
  const [deleteTarget, setDeleteTarget] = useState<NumeradorTropa | null>(null)
  const [isDeleting, setIsDeleting] = useState(false)

  const fetchData = useCallback(async () => {
    setIsLoading(true)
    try {
      const items = await getNumeradoresTropas()
      setData(items)
    } catch {
      toast('error', 'Error al cargar numeradores de tropas')
    } finally {
      setIsLoading(false)
    }
  }, [toast])

  useEffect(() => {
    void fetchData()
  }, [fetchData])

  const handleDelete = async () => {
    if (!deleteTarget) return
    setIsDeleting(true)
    try {
      await deleteNumeradorTropa(deleteTarget.id)
      toast('success', 'Numerador eliminado')
      setDeleteTarget(null)
      void fetchData()
    } catch (err) {
      toast('error', err instanceof Error ? err.message : 'Error al eliminar')
    } finally {
      setIsDeleting(false)
    }
  }

  const columns: Column<NumeradorTropa>[] = [
    { key: 'nombreCliente', header: 'Cliente', sortable: true },
    { key: 'nombreEstablecimiento', header: 'Establecimiento', sortable: true },
    { key: 'especieNombre', header: 'Especie', width: '150px', sortable: true },
    {
      key: 'ultimoNumeroTropa',
      header: 'Ultimo Numero Tropa',
      width: '180px',
      sortable: true,
      render: (value) => <span className="font-mono">{String(value)}</span>,
    },
    {
      key: '_actions',
      header: '',
      width: '100px',
      render: (_, row) => (
        <div className="flex items-center gap-1">
          <button
            onClick={(e) => { e.stopPropagation(); navigate(`/numeradores-tropas/${row.id}/edit`) }}
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
      <PageHeader title="Numeradores de Tropas">
        <Button onClick={() => navigate('/numeradores-tropas/create')}>Nuevo Numerador</Button>
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
        title="Eliminar numerador"
        message={`¿Esta seguro que desea eliminar el numerador de ${deleteTarget?.nombreCliente} - ${deleteTarget?.nombreEstablecimiento} (${deleteTarget?.especieNombre})?`}
        isLoading={isDeleting}
      />
    </>
  )
}
