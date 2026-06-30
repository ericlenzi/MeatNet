import { useState, useEffect, useCallback } from 'react'
import { getExistenciaHacienda } from '@/services/existenciaHacienda.service'
import { useApp } from '@/contexts/AppContext'
import { useToast } from '@/components/ui/Toast'
import type { ExistenciaCorralItem } from '@/types'
import DataTable from '@/components/ui/DataTable'
import type { Column, SortState } from '@/components/ui/DataTable'
import PageHeader from '@/components/ui/PageHeader'

const num = (v: unknown) => Number(v).toLocaleString('es-AR')

export default function ExistenciaHaciendaPage() {
  const { toast } = useToast()
  const { currentEstablecimiento } = useApp()
  const [data, setData] = useState<ExistenciaCorralItem[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [sort, setSort] = useState<SortState | null>(null)

  const fetchData = useCallback(async () => {
    setIsLoading(true)
    try {
      const items = await getExistenciaHacienda({ EstablecimientoId: currentEstablecimiento?.id })
      setData(items)
    } catch {
      toast('error', 'Error al cargar la existencia de hacienda')
    } finally {
      setIsLoading(false)
    }
  }, [currentEstablecimiento?.id, toast])

  useEffect(() => {
    void fetchData()
  }, [fetchData])

  const totalUN = data.reduce((acc, x) => acc + x.cantidadUN, 0)
  const totalKG = data.reduce((acc, x) => acc + x.pesoKG, 0)

  const columns: Column<ExistenciaCorralItem>[] = [
    { key: 'almacenNombre', header: 'Corral', sortable: true },
    { key: 'capacidadCorral', header: 'Capacidad', width: '110px', render: (v) => <span className="font-mono">{num(v)}</span> },
    { key: 'tipoEspecieNombre', header: 'Tipo Especie', sortable: true },
    { key: 'clienteNombre', header: 'Cliente', sortable: true },
    { key: 'numeroTropa', header: 'Tropa', width: '90px', render: (v) => <span className="font-mono">{String(v)}</span> },
    { key: 'cantidadUN', header: 'Cabezas (UN)', width: '130px', render: (v) => <span className="font-mono">{num(v)}</span> },
    { key: 'pesoKG', header: 'Peso (KG)', width: '130px', render: (v) => <span className="font-mono">{num(Math.round(Number(v)))}</span> },
  ]

  return (
    <>
      <PageHeader title="Existencia de Hacienda" />

      <div className="mb-4 flex flex-wrap gap-3">
        <div className="rounded-lg border border-border bg-surface px-4 py-2 text-sm">
          <span className="text-text-light">Total cabezas (UN): </span>
          <span className="font-mono font-semibold">{num(totalUN)}</span>
        </div>
        <div className="rounded-lg border border-border bg-surface px-4 py-2 text-sm">
          <span className="text-text-light">Total peso (KG): </span>
          <span className="font-mono font-semibold">{num(Math.round(totalKG))}</span>
        </div>
      </div>

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
    </>
  )
}
