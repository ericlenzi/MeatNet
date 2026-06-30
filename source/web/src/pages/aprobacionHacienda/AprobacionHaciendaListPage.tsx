import { useState, useEffect, useCallback } from 'react'
import { useNavigate } from 'react-router'
import {
  getIngresosHaciendas,
  aprobarIngresoHacienda,
  rechazarIngresoHacienda,
} from '@/services/ingresosHacienda.service'
import { useApp } from '@/contexts/AppContext'
import { useToast } from '@/components/ui/Toast'
import { usePagination } from '@/hooks/usePagination'
import { EstadoIngreso } from '@/types'
import type { IngresoHaciendaListItem } from '@/types'
import DataTable from '@/components/ui/DataTable'
import type { Column, SortState } from '@/components/ui/DataTable'
import PageHeader from '@/components/ui/PageHeader'
import Button from '@/components/ui/Button'
import ConfirmDialog from '@/components/ui/ConfirmDialog'

function formatFecha(value: string): string {
  if (!value) return ''
  return new Date(value).toLocaleString('es-AR', { dateStyle: 'short', timeStyle: 'short' })
}

type ActionTarget = { row: IngresoHaciendaListItem; type: 'aprobar' | 'rechazar' }

export default function AprobacionHaciendaListPage() {
  const navigate = useNavigate()
  const { toast } = useToast()
  const { currentEstablecimiento } = useApp()
  const pagination = usePagination()
  const { setTotalRows } = pagination

  const [data, setData] = useState<IngresoHaciendaListItem[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [sort, setSort] = useState<SortState | null>(null)
  const [actionTarget, setActionTarget] = useState<ActionTarget | null>(null)
  const [isProcessing, setIsProcessing] = useState(false)

  const fetchData = useCallback(async () => {
    setIsLoading(true)
    try {
      const res = await getIngresosHaciendas({
        PageIndex: pagination.pageIndex,
        PageSize: pagination.pageSize,
        EstablecimientoId: currentEstablecimiento?.id,
        EstadoIngresoId: EstadoIngreso.PendienteAprobacion,
      })
      setData(res.data || [])
      setTotalRows(res.totalRows || 0)
    } catch {
      toast('error', 'Error al cargar ingresos pendientes')
    } finally {
      setIsLoading(false)
    }
  }, [pagination.pageIndex, pagination.pageSize, currentEstablecimiento?.id, setTotalRows, toast])

  useEffect(() => {
    void fetchData()
  }, [fetchData])

  const handleConfirm = async () => {
    if (!actionTarget) return
    setIsProcessing(true)
    try {
      if (actionTarget.type === 'aprobar') {
        const result = await aprobarIngresoHacienda(actionTarget.row.id)
        toast('success', `Ingreso aprobado. Tropas generadas: ${result.tropas.length}`)
        if (result.advertencia) toast('error', result.advertencia)
      } else {
        await rechazarIngresoHacienda(actionTarget.row.id)
        toast('success', 'Ingreso rechazado (vuelve a Borrador)')
      }
      setActionTarget(null)
      void fetchData()
    } catch (err) {
      toast('error', err instanceof Error ? err.message : 'Error al procesar')
    } finally {
      setIsProcessing(false)
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
      key: '_actions',
      header: '',
      width: '200px',
      render: (_, row) => (
        <div className="flex items-center gap-2">
          <Button size="sm" variant="ghost" onClick={() => navigate(`/operaciones/ingreso-hacienda/${row.id}/edit`)}>Ver</Button>
          <Button size="sm" onClick={() => setActionTarget({ row, type: 'aprobar' })}>Aprobar</Button>
          <Button size="sm" variant="secondary" onClick={() => setActionTarget({ row, type: 'rechazar' })}>Rechazar</Button>
        </div>
      ),
    },
  ]

  return (
    <>
      <PageHeader title="Aprobación de Hacienda" />

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
        isOpen={!!actionTarget}
        onConfirm={handleConfirm}
        onCancel={() => setActionTarget(null)}
        title={actionTarget?.type === 'aprobar' ? 'Aprobar ingreso' : 'Rechazar ingreso'}
        message={
          actionTarget?.type === 'aprobar'
            ? `¿Aprobar el ingreso N° ${actionTarget?.row.numeroIngreso}? Se generaran las tropas y la hacienda contara como stock.`
            : `¿Rechazar el ingreso N° ${actionTarget?.row.numeroIngreso}? Volvera a estado Borrador.`
        }
        isLoading={isProcessing}
      />
    </>
  )
}
