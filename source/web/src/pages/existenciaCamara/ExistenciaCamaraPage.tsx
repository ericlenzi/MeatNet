import { useState, useEffect, useCallback } from 'react'
import { getExistenciaCamara, getMovimientosCamara } from '@/services/existenciaCamara.service'
import { useApp } from '@/contexts/AppContext'
import { useToast } from '@/components/ui/Toast'
import type { ExistenciaCamaraItem, MovimientoCamaraItem } from '@/types/existenciaCamara'
import DataTable from '@/components/ui/DataTable'
import type { Column, SortState } from '@/components/ui/DataTable'
import PageHeader from '@/components/ui/PageHeader'
import Modal from '@/components/ui/Modal'
import Spinner from '@/components/ui/Spinner'

const num = (v: unknown) => Number(v).toLocaleString('es-AR')
const kg = (v: unknown) => Number(v).toLocaleString('es-AR', { minimumFractionDigits: 2, maximumFractionDigits: 2 })

/** Origen de un movimiento: la media res y el garron de los que salio. */
function origen(m: MovimientoCamaraItem) {
  if (m.numeroRomaneo == null) return '-'
  const letra = m.letra ? ` (${m.letra})` : ''
  return `Romaneo ${m.numeroRomaneo} · Garrón ${m.numeroGarron}${letra}`
}

export default function ExistenciaCamaraPage() {
  const { toast } = useToast()
  const { currentEstablecimiento } = useApp()
  const [data, setData] = useState<ExistenciaCamaraItem[]>([])
  const [totales, setTotales] = useState({ cantidad: 0, peso: 0 })
  const [isLoading, setIsLoading] = useState(true)
  const [sort, setSort] = useState<SortState | null>(null)

  // Drill-down: los movimientos que componen el saldo de una linea.
  const [detalle, setDetalle] = useState<ExistenciaCamaraItem | null>(null)
  const [movimientos, setMovimientos] = useState<MovimientoCamaraItem[]>([])
  const [isLoadingDetalle, setIsLoadingDetalle] = useState(false)

  const fetchData = useCallback(async () => {
    setIsLoading(true)
    try {
      const res = await getExistenciaCamara({ EstablecimientoId: currentEstablecimiento?.id })
      setData(res.data)
      setTotales({ cantidad: res.totalCantidad, peso: res.totalPeso })
    } catch {
      toast('error', 'Error al cargar la existencia de cámara')
    } finally {
      setIsLoading(false)
    }
  }, [currentEstablecimiento?.id, toast])

  useEffect(() => {
    void fetchData()
  }, [fetchData])

  const verDetalle = async (item: ExistenciaCamaraItem) => {
    setDetalle(item)
    setIsLoadingDetalle(true)
    try {
      setMovimientos(
        await getMovimientosCamara({ AlmacenId: item.almacenId, MaterialId: item.materialId }),
      )
    } catch {
      toast('error', 'Error al cargar los movimientos')
      setMovimientos([])
    } finally {
      setIsLoadingDetalle(false)
    }
  }

  const columns: Column<ExistenciaCamaraItem>[] = [
    { key: 'almacenNombre', header: 'Cámara', sortable: true },
    { key: 'materialCodigo', header: 'Código', width: '90px', render: (v) => <span className="font-mono">{String(v)}</span> },
    { key: 'materialNombre', header: 'Material', sortable: true },
    { key: 'tipoMaterialNombre', header: 'Tipo', width: '120px' },
    { key: 'cantidad', header: 'Cantidad', width: '100px', render: (v) => <span className="font-mono font-semibold">{num(v)}</span> },
    { key: 'peso', header: 'Peso (KG)', width: '120px', render: (v) => <span className="font-mono">{kg(v)}</span> },
    {
      key: 'materialId',
      header: '',
      width: '90px',
      render: (_v, row) => (
        <button
          type="button"
          onClick={() => void verDetalle(row)}
          className="text-sm font-medium text-primary-600 hover:text-primary-800 hover:underline"
        >
          Trazar
        </button>
      ),
    },
  ]

  return (
    <>
      <PageHeader title="Existencia de Cámara" />

      <div className="mb-4 flex flex-wrap gap-3">
        <div className="rounded-lg border border-border bg-surface px-4 py-2 text-sm">
          <span className="text-text-light">Piezas en cámara: </span>
          <span className="font-mono font-semibold">{num(totales.cantidad)}</span>
        </div>
        <div className="rounded-lg border border-border bg-surface px-4 py-2 text-sm">
          <span className="text-text-light">Total peso (KG): </span>
          <span className="font-mono font-semibold">{kg(totales.peso)}</span>
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

      <Modal
        isOpen={detalle !== null}
        onClose={() => setDetalle(null)}
        title={detalle ? `${detalle.materialNombre} · ${detalle.almacenNombre}` : ''}
        size="lg"
      >
        {isLoadingDetalle ? (
          <div className="flex justify-center py-8">
            <Spinner />
          </div>
        ) : movimientos.length === 0 ? (
          <p className="py-6 text-center text-sm text-text-light">Sin movimientos.</p>
        ) : (
          <div className="max-h-[60vh] overflow-auto">
            <table className="w-full text-sm">
              <thead className="sticky top-0 bg-surface">
                <tr className="border-b border-border text-left text-text-light">
                  <th className="px-2 py-2 font-medium">Movimiento</th>
                  <th className="px-2 py-2 text-right font-medium">Cant.</th>
                  <th className="px-2 py-2 text-right font-medium">Peso</th>
                  <th className="px-2 py-2 font-medium">Origen</th>
                  <th className="px-2 py-2 font-medium">Tropa</th>
                </tr>
              </thead>
              <tbody>
                {movimientos.map((m) => (
                  <tr key={m.id} className="border-b border-border/60">
                    <td className="px-2 py-2">{m.tipoMovimientoNombre}</td>
                    <td className={`px-2 py-2 text-right font-mono ${m.cantidad < 0 ? 'text-amber-600' : ''}`}>
                      {num(m.cantidad)}
                    </td>
                    <td className={`px-2 py-2 text-right font-mono ${m.peso < 0 ? 'text-amber-600' : ''}`}>
                      {kg(m.peso)}
                    </td>
                    <td className="px-2 py-2 text-text-light">{origen(m)}</td>
                    <td className="px-2 py-2 font-mono text-text-light">{m.numeroTropa ?? '-'}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </Modal>
    </>
  )
}
