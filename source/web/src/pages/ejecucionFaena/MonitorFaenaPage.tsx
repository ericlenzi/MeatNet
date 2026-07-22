import { useState, useEffect, useCallback } from 'react'
import { useNavigate, useParams } from 'react-router'
import { getMonitorFaena } from '@/services/romaneos.service'
import { useToast } from '@/components/ui/Toast'
import type { MonitorFaena, RenglonMonitorItem } from '@/types'
import PageHeader from '@/components/ui/PageHeader'
import Button from '@/components/ui/Button'

const POLL_MS = 5000

function Stat({ label, value, hint }: { label: string; value: string | number; hint?: string }) {
  return (
    <div className="rounded-lg border border-border bg-surface p-4 shadow-sm">
      <p className="text-xs text-text-light">{label}</p>
      <p className="mt-1 text-2xl font-semibold text-text">{value}</p>
      {hint && <p className="text-xs text-text-light">{hint}</p>}
    </div>
  )
}

/** Rango de numeros de romaneo registrados en el renglon ("120 - 135", "120" o "—"). */
function rangoRomaneos(r: RenglonMonitorItem) {
  if (r.romaneoDesde == null || r.romaneoHasta == null) return '—'
  return r.romaneoDesde === r.romaneoHasta ? `${r.romaneoDesde}` : `${r.romaneoDesde} - ${r.romaneoHasta}`
}

function MonitorBoard({ listaMatanzaId }: { listaMatanzaId: string }) {
  const navigate = useNavigate()
  const { toast } = useToast()
  const [m, setM] = useState<MonitorFaena | null>(null)
  const [loading, setLoading] = useState(true)

  const fetchData = useCallback(async () => {
    try {
      setM(await getMonitorFaena(listaMatanzaId))
    } catch (err) {
      toast('error', err instanceof Error ? err.message : 'Error al cargar el monitor')
    } finally {
      setLoading(false)
    }
  }, [listaMatanzaId, toast])

  useEffect(() => {
    void fetchData()
    const t = setInterval(() => void fetchData(), POLL_MS)
    return () => clearInterval(t)
  }, [fetchData])

  if (loading) return <div className="p-6 text-text-light">Cargando...</div>
  if (!m) return <div className="p-6 text-text-light">Monitor no disponible.</div>

  const avance = m.totalPlanificado > 0 ? Math.round((m.totalFaenado / m.totalPlanificado) * 100) : 0

  return (
    <>
      <PageHeader title={`Monitor de Faena — Lista N° ${m.numeroLista} (${m.especieNombre})`}>
        <Button
          variant="secondary"
          onClick={() => navigate(`/operaciones/ejecucion-faena/${m.listaMatanzaId}/tipificador`)}
        >
          Ir al Tipificador
        </Button>
      </PageHeader>

      <div className="mb-4 grid grid-cols-2 gap-3 sm:grid-cols-3 lg:grid-cols-6">
        <Stat label="Planificado" value={m.totalPlanificado} />
        <Stat label="Faenado" value={m.totalFaenado} hint={`${avance}%`} />
        <Stat label="Pendiente" value={m.totalPendiente} />
        <Stat label="Animales" value={m.animalesRomaneados} />
        <Stat label="Kg totales" value={m.kgTotales.toFixed(0)} />
        <Stat label="Ritmo" value={m.ritmoPorHora} hint="animales/hora" />
      </div>

      {/* Barra de avance */}
      <div className="mb-4 rounded-lg border border-border bg-surface p-4 shadow-sm">
        <div className="mb-1 flex items-center justify-between text-sm">
          <span className="text-text-light">Avance de la jornada</span>
          <span className="font-medium">{avance}%</span>
        </div>
        <div className="h-3 w-full overflow-hidden rounded-full bg-background">
          <div className="h-full rounded-full bg-primary-500 transition-all" style={{ width: `${avance}%` }} />
        </div>
      </div>

      {/* Por renglon */}
      <div className="rounded-lg border border-border bg-surface p-6 shadow-sm">
        <h3 className="mb-3 text-sm font-semibold text-text">Avance por renglon</h3>
        <div className="overflow-x-auto">
          <table className="w-full text-sm">
            <thead>
              <tr className="border-b border-border text-left text-text-light">
                <th className="py-2 pr-3 w-16">Sec.</th>
                <th className="py-2 pr-3">Tropa</th>
                <th className="py-2 pr-3">Corral</th>
                <th className="py-2 pr-3">Camara destino</th>
                <th className="py-2 pr-3">Categoria</th>
                <th className="py-2 pr-3 text-right">Plan.</th>
                <th className="py-2 pr-3 text-right">Faenado</th>
                <th className="py-2 pr-3 text-right">Pend.</th>
                <th className="py-2 pr-3">N° Romaneo (desde–hasta)</th>
                <th className="py-2 pr-3 w-40">Avance</th>
              </tr>
            </thead>
            <tbody>
              {m.porRenglon.map((r) => {
                const pct = r.cantidad > 0 ? Math.round((r.cantidadFaenada / r.cantidad) * 100) : 0
                return (
                  <tr key={r.listaMatanzaDetalleId} className="border-b border-border/60">
                    <td className="py-2 pr-3 font-mono">{r.secuencia}</td>
                    <td className="py-2 pr-3 font-mono">{r.numeroTropa}</td>
                    <td className="py-2 pr-3">{r.almacenNombre}</td>
                    <td className="py-2 pr-3">{r.almacenDestinoNombre ?? <span className="text-text-light">—</span>}</td>
                    <td className="py-2 pr-3">{r.tipoEspecieNombre}</td>
                    <td className="py-2 pr-3 text-right font-mono">{r.cantidad}</td>
                    <td className="py-2 pr-3 text-right font-mono">{r.cantidadFaenada}</td>
                    <td className="py-2 pr-3 text-right font-mono">{r.pendiente}</td>
                    <td className="py-2 pr-3 font-mono text-xs">{rangoRomaneos(r)}</td>
                    <td className="py-2 pr-3">
                      <div className="flex items-center gap-2">
                        <div className="h-2 flex-1 overflow-hidden rounded-full bg-background">
                          <div className="h-full rounded-full bg-primary-500" style={{ width: `${pct}%` }} />
                        </div>
                        <span className="w-9 text-right font-mono text-xs">{pct}%</span>
                      </div>
                    </td>
                  </tr>
                )
              })}
            </tbody>
          </table>
        </div>
      </div>
    </>
  )
}

export default function MonitorFaenaPage() {
  const { listaMatanzaId } = useParams()
  if (!listaMatanzaId) return <div className="p-6 text-text-light">Falta la lista de matanza.</div>
  return <MonitorBoard listaMatanzaId={listaMatanzaId} />
}
