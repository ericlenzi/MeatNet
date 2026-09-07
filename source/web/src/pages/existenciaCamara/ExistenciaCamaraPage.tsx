import { useState, useEffect, useCallback } from 'react'
import { getExistenciaCamara, getMovimientosCamara } from '@/services/existenciaCamara.service'
import { getClientes } from '@/services/clientes.service'
import { useApp } from '@/contexts/AppContext'
import { useToast } from '@/components/ui/Toast'
import type { Agrupacion, ExistenciaCamaraItem, MovimientoCamaraItem } from '@/types/existenciaCamara'
import { AgrupacionExistencia } from '@/types/existenciaCamara'
import type { Cliente } from '@/types/cliente'
import DataTable from '@/components/ui/DataTable'
import type { Column, SortState } from '@/components/ui/DataTable'
import PageHeader from '@/components/ui/PageHeader'
import Modal from '@/components/ui/Modal'
import Spinner from '@/components/ui/Spinner'

const num = (v: unknown) => Number(v).toLocaleString('es-AR')
const kg = (v: unknown) =>
  Number(v).toLocaleString('es-AR', { minimumFractionDigits: 2, maximumFractionDigits: 2 })

const SIN_PROVEEDOR = 'Sin proveedor'

const VISTAS: { valor: Agrupacion; label: string }[] = [
  { valor: AgrupacionExistencia.Material, label: 'Material' },
  { valor: AgrupacionExistencia.Proveedor, label: 'Proveedor' },
  { valor: AgrupacionExistencia.Camara, label: 'Cámara' },
]

/** Origen de un movimiento: la media res y el garrón de los que salió. */
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

  const [vista, setVista] = useState<Agrupacion>(AgrupacionExistencia.Material)
  const [clienteId, setClienteId] = useState('')
  const [clientes, setClientes] = useState<Cliente[]>([])

  // Drill-down: los movimientos que componen el saldo de una línea.
  const [detalle, setDetalle] = useState<ExistenciaCamaraItem | null>(null)
  const [movimientos, setMovimientos] = useState<MovimientoCamaraItem[]>([])
  const [isLoadingDetalle, setIsLoadingDetalle] = useState(false)

  useEffect(() => {
    void (async () => {
      try {
        const res = await getClientes({ PageSize: 500 })
        setClientes(res.data || [])
      } catch {
        // El filtro es opcional: si no se pueden listar, la pantalla sigue sirviendo.
      }
    })()
  }, [])

  const fetchData = useCallback(async () => {
    setIsLoading(true)
    try {
      const res = await getExistenciaCamara({
        EstablecimientoId: currentEstablecimiento?.id,
        ClienteId: clienteId || undefined,
        AgruparPor: vista,
      })
      setData(res.data)
      setTotales({ cantidad: res.totalCantidad, peso: res.totalPeso })
    } catch {
      toast('error', 'Error al cargar la existencia de cámara')
    } finally {
      setIsLoading(false)
    }
  }, [currentEstablecimiento?.id, clienteId, vista, toast])

  useEffect(() => {
    void fetchData()
  }, [fetchData])

  const verDetalle = async (item: ExistenciaCamaraItem) => {
    setDetalle(item)
    setIsLoadingDetalle(true)
    try {
      // Se piden los movimientos por el mismo corte que arma la línea.
      setMovimientos(
        await getMovimientosCamara({
          AlmacenId: item.almacenId ?? undefined,
          MaterialId: item.materialId ?? undefined,
          ClienteId: item.clienteId ?? undefined,
        }),
      )
    } catch {
      toast('error', 'Error al cargar los movimientos')
      setMovimientos([])
    } finally {
      setIsLoadingDetalle(false)
    }
  }

  const colCantidad: Column<ExistenciaCamaraItem> = {
    key: 'cantidad',
    header: 'Cantidad',
    width: '100px',
    render: (v) => <span className="font-mono font-semibold">{num(v)}</span>,
  }
  const colPeso: Column<ExistenciaCamaraItem> = {
    key: 'peso',
    header: 'Peso (KG)',
    width: '120px',
    render: (v) => <span className="font-mono">{kg(v)}</span>,
  }
  const colTrazar: Column<ExistenciaCamaraItem> = {
    key: 'ultimoMovimiento',
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
  }
  const colProveedor: Column<ExistenciaCamaraItem> = {
    key: 'clienteNombre',
    header: 'Proveedor',
    sortable: true,
    render: (v) => <>{(v as string) ?? SIN_PROVEEDOR}</>,
  }

  // Las columnas siguen al corte: solo se muestra lo que participa de la agrupación.
  const columns: Column<ExistenciaCamaraItem>[] =
    vista === AgrupacionExistencia.Proveedor
      ? [
          colProveedor,
          { key: 'materialCodigo', header: 'Código', width: '90px', render: (v) => <span className="font-mono">{String(v ?? '')}</span> },
          { key: 'materialNombre', header: 'Material', sortable: true },
          { key: 'tipoMaterialNombre', header: 'Tipo', width: '120px' },
          colCantidad,
          colPeso,
          colTrazar,
        ]
      : vista === AgrupacionExistencia.Camara
        ? [
            { key: 'almacenNombre', header: 'Cámara', sortable: true },
            colProveedor,
            colCantidad,
            colPeso,
            colTrazar,
          ]
        : [
            { key: 'almacenNombre', header: 'Cámara', sortable: true },
            { key: 'materialCodigo', header: 'Código', width: '90px', render: (v) => <span className="font-mono">{String(v ?? '')}</span> },
            { key: 'materialNombre', header: 'Material', sortable: true },
            { key: 'tipoMaterialNombre', header: 'Tipo', width: '120px' },
            colCantidad,
            colPeso,
            colTrazar,
          ]

  const tituloDetalle = (d: ExistenciaCamaraItem) =>
    [d.materialNombre, d.almacenNombre, d.clienteNombre].filter(Boolean).join(' · ')

  return (
    <>
      <PageHeader title="Existencia de Cámara" />

      <div className="mb-4 flex flex-wrap items-center gap-3">
        <div className="flex items-center gap-2">
          <label htmlFor="proveedor" className="text-sm text-text-light">
            Proveedor:
          </label>
          <select
            id="proveedor"
            value={clienteId}
            onChange={(e) => setClienteId(e.target.value)}
            className="rounded-md border border-border bg-surface px-3 py-1.5 text-sm"
          >
            <option value="">Todos</option>
            {clientes.map((c) => (
              <option key={c.id} value={c.id}>
                {c.nombre}
              </option>
            ))}
          </select>
        </div>

        <div className="flex items-center gap-2">
          <span className="text-sm text-text-light">Ver por:</span>
          <div className="inline-flex overflow-hidden rounded-md border border-border">
            {VISTAS.map((v) => (
              <button
                key={v.valor}
                type="button"
                onClick={() => setVista(v.valor)}
                className={`px-3 py-1.5 text-sm ${
                  vista === v.valor
                    ? 'bg-primary-600 font-medium text-white'
                    : 'bg-surface text-text hover:bg-gray-50'
                }`}
              >
                {v.label}
              </button>
            ))}
          </div>
        </div>

        <div className="ml-auto flex gap-3">
          <div className="rounded-lg border border-border bg-surface px-4 py-2 text-sm">
            <span className="text-text-light">Piezas en cámara: </span>
            <span className="font-mono font-semibold">{num(totales.cantidad)}</span>
          </div>
          <div className="rounded-lg border border-border bg-surface px-4 py-2 text-sm">
            <span className="text-text-light">Total peso (KG): </span>
            <span className="font-mono font-semibold">{kg(totales.peso)}</span>
          </div>
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
        title={detalle ? tituloDetalle(detalle) : ''}
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
                  <th className="px-2 py-2 font-medium">Proveedor</th>
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
                    <td className="px-2 py-2">{m.clienteNombre ?? SIN_PROVEEDOR}</td>
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
