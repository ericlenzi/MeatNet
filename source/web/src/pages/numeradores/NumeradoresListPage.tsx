import { useState, useEffect, useCallback } from 'react'
import { useNavigate } from 'react-router'
import { getNumeradores, deleteNumerador } from '@/services/numeradores.service'
import { useApp } from '@/contexts/AppContext'
import { useToast } from '@/components/ui/Toast'
import type { Numerador } from '@/types'
import DataTable from '@/components/ui/DataTable'
import type { Column, SortState } from '@/components/ui/DataTable'
import PageHeader from '@/components/ui/PageHeader'
import Button from '@/components/ui/Button'
import Badge from '@/components/ui/Badge'
import ConfirmDialog from '@/components/ui/ConfirmDialog'

export default function NumeradoresListPage() {
  const navigate = useNavigate()
  const { toast } = useToast()
  const { currentEstablecimiento } = useApp()
  const [data, setData] = useState<Numerador[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [sort, setSort] = useState<SortState | null>(null)
  const [deleteTarget, setDeleteTarget] = useState<Numerador | null>(null)
  const [isDeleting, setIsDeleting] = useState(false)

  const fetchData = useCallback(async () => {
    setIsLoading(true)
    try {
      const items = await getNumeradores({ EstablecimientoId: currentEstablecimiento?.id })
      setData(items)
    } catch {
      toast('error', 'Error al cargar numeradores')
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
      await deleteNumerador(deleteTarget.id)
      toast('success', 'Numerador eliminado')
      setDeleteTarget(null)
      void fetchData()
    } catch (err) {
      toast('error', err instanceof Error ? err.message : 'Error al eliminar')
    } finally {
      setIsDeleting(false)
    }
  }

  const columns: Column<Numerador>[] = [
    { key: 'codigo', header: 'Codigo', width: '120px', sortable: true },
    { key: 'descripcion', header: 'Descripcion' },
    { key: 'especieNombre', header: 'Especie', width: '120px' },
    { key: 'tipoNumerador', header: 'Tipo', width: '130px' },
    {
      key: 'ultimoNumero',
      header: 'Ultimo N°',
      width: '110px',
      render: (v) => <span className="font-mono">{String(v)}</span>,
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
            onClick={(e) => { e.stopPropagation(); navigate(`/numeradores/${row.id}/edit`) }}
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
      <PageHeader title="Numeradores">
        <Button onClick={() => navigate('/numeradores/create')}>Nuevo Numerador</Button>
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
        message={`¿Esta seguro que desea eliminar el numerador "${deleteTarget?.codigo}"?`}
        isLoading={isDeleting}
      />
    </>
  )
}
