import { useState, useEffect, useCallback } from 'react'
import { useParams, useNavigate } from 'react-router'
import { getAnalisisFaena } from '@/services/analisisFaena.service'
import { useToast } from '@/components/ui/Toast'
import type { AnalisisFaenaResponse } from '@/types/analisisFaena'
import PageHeader from '@/components/ui/PageHeader'
import Button from '@/components/ui/Button'
import Badge from '@/components/ui/Badge'

const kg = (v: number) => v.toLocaleString('es-AR', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
const pct = (v: number) => `${v.toLocaleString('es-AR', { minimumFractionDigits: 2, maximumFractionDigits: 2 })}%`
/** Las descripciones cargadas a mano a veces traen saltos de linea. */
const limpio = (s: string | null) => (s ?? '').replace(/\s+/g, ' ').trim()

function Seccion({ titulo, children }: { titulo: string; children: React.ReactNode }) {
  return (
    <div className="mb-4 rounded-lg border border-border bg-surface shadow-sm">
      <h3 className="border-b border-border px-4 py-3 text-sm font-semibold text-text">{titulo}</h3>
      <div className="overflow-x-auto">{children}</div>
    </div>
  )
}

const th = 'px-3 py-2 text-left font-medium'
const thr = 'px-3 py-2 text-right font-medium'
const td = 'px-3 py-2'
const tdr = 'px-3 py-2 text-right font-mono'

export default function AnalisisFaenaPage() {
  const { listaMatanzaId } = useParams<{ listaMatanzaId: string }>()
  const navigate = useNavigate()
  const { toast } = useToast()
  const [data, setData] = useState<AnalisisFaenaResponse | null>(null)
  const [isLoading, setIsLoading] = useState(true)

  const fetchData = useCallback(async () => {
    if (!listaMatanzaId) return
    setIsLoading(true)
    try {
      setData(await getAnalisisFaena(listaMatanzaId))
    } catch (err) {
      toast('error', err instanceof Error ? err.message : 'Error al cargar el análisis')
    } finally {
      setIsLoading(false)
    }
  }, [listaMatanzaId, toast])

  useEffect(() => {
    void fetchData()
  }, [fetchData])

  if (isLoading) return <div className="p-6 text-text-light">Cargando...</div>
  if (!data) return <div className="p-6 text-text-light">Jornada no encontrada.</div>

  return (
    <>
      <div className="no-print">
        <PageHeader title={`Análisis de Faena · Lista N° ${data.numeroLista}`} />
      </div>

      <div className="mb-4 rounded-lg border border-border bg-surface p-4 shadow-sm">
        <div className="flex flex-wrap items-center gap-x-6 gap-y-2 text-sm">
          <span>
            <span className="text-text-light">Establecimiento: </span>
            <span className="font-medium">{data.establecimientoNombre}</span>
          </span>
          <span>
            <span className="text-text-light">Fecha: </span>
            <span className="font-medium">{new Date(data.fecha).toLocaleDateString('es-AR')}</span>
          </span>
          <Badge variant={data.estadoListaMatanzaId === 'FINALIZADA' ? 'success' : 'info'}>
            {data.estadoListaMatanzaId}
          </Badge>
        </div>
      </div>

      {/* Resumen: el rinde va acompanado de su definicion (R-A5). */}
      <div className="mb-4 grid grid-cols-2 gap-3 md:grid-cols-5">
        {[
          { label: 'Animales', valor: data.animalesFaenados.toLocaleString('es-AR') },
          { label: 'Piezas', valor: data.piezas.toLocaleString('es-AR') },
          { label: 'Kg vivos', valor: data.kgVivos != null ? kg(data.kgVivos) : 's/d' },
          { label: 'Kg faena', valor: kg(data.kgFaena) },
        ].map((c) => (
          <div key={c.label} className="rounded-lg border border-border bg-surface px-4 py-3 shadow-sm">
            <p className="text-xs text-text-light">{c.label}</p>
            <p className="font-mono text-lg font-semibold">{c.valor}</p>
          </div>
        ))}
        <div className="rounded-lg border border-primary-200 bg-primary-50 px-4 py-3 shadow-sm">
          <p className="text-xs text-primary-700">Rinde caliente</p>
          <p className="font-mono text-lg font-semibold text-primary-800">
            {data.rindeCaliente != null ? pct(data.rindeCaliente) : 's/d'}
          </p>
        </div>
      </div>

      <div className="mb-4 rounded-md bg-amber-50 px-4 py-3 text-xs text-amber-900">
        <p className="mb-1 font-semibold">Cómo leer el rinde</p>
        <p>
          Kg de romaneo sobre kg vivos <strong>de ingreso</strong>, prorrateados por el peso promedio
          de cada tropa. No descuenta el desbaste previo al sacrificio (no hay balanza en playa), es
          peso <strong>caliente</strong> (sin merma de oreo) y no descuenta decomisos. Sirve para
          comparar jornadas entre sí, no contra un rinde frío de referencia.
        </p>
        {data.animalesSinPesoVivo > 0 && (
          <p className="mt-1 font-medium">
            {data.animalesSinPesoVivo} animal(es) sin peso de ingreso cargado quedaron fuera del cálculo.
          </p>
        )}
      </div>

      <Seccion titulo="Por cliente">
        <table className="w-full text-sm">
          <thead className="border-b border-border bg-gray-50 text-text-light">
            <tr>
              <th className={th}>Cliente</th>
              <th className={thr}>Animales</th>
              <th className={thr}>Piezas</th>
              <th className={thr}>Kg vivos</th>
              <th className={thr}>Kg faena</th>
              <th className={thr}>Rinde</th>
              <th className={thr}>Partic.</th>
            </tr>
          </thead>
          <tbody>
            {data.porCliente.map((c) => (
              <tr key={c.clienteId} className="border-b border-border/60">
                <td className={td}>{c.clienteNombre}</td>
                <td className={tdr}>{c.animalesFaenados}</td>
                <td className={tdr}>{c.piezas}</td>
                <td className={tdr}>{c.kgVivos != null ? kg(c.kgVivos) : 's/d'}</td>
                <td className={tdr}>{kg(c.kgFaena)}</td>
                <td className={`${tdr} font-semibold`}>
                  {c.rindeCaliente != null ? pct(c.rindeCaliente) : 's/d'}
                </td>
                <td className={`${tdr} text-text-light`}>{pct(c.participacionKg)}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </Seccion>

      <Seccion titulo="Plan vs. real">
        <table className="w-full text-sm">
          <thead className="border-b border-border bg-gray-50 text-text-light">
            <tr>
              <th className={thr}>Sec.</th>
              <th className={th}>Tropa</th>
              <th className={th}>Cliente</th>
              <th className={th}>Corral</th>
              <th className={th}>Categoría</th>
              <th className={thr}>Planif.</th>
              <th className={thr}>Faenado</th>
              <th className={thr}>Dif.</th>
              <th className={thr}>Cumpl.</th>
            </tr>
          </thead>
          <tbody>
            {data.planVsReal.map((r, i) => (
              <tr key={`${r.numeroTropa}-${r.secuencia}-${i}`} className="border-b border-border/60">
                <td className={tdr}>{r.secuencia}</td>
                <td className={`${td} font-mono`}>{r.numeroTropa}</td>
                <td className={td}>{r.clienteNombre}</td>
                <td className={`${td} text-text-light`}>{r.corralNombre}</td>
                <td className={td}>{r.tipoEspecieNombre}</td>
                <td className={tdr}>{r.planificado}</td>
                <td className={tdr}>{r.faenado}</td>
                <td className={`${tdr} ${r.diferencia < 0 ? 'text-amber-600' : ''}`}>{r.diferencia}</td>
                <td className={`${tdr} font-semibold`}>{pct(r.cumplimiento)}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </Seccion>

      <Seccion titulo="Tipificación consolidada">
        <table className="w-full text-sm">
          <thead className="border-b border-border bg-gray-50 text-text-light">
            <tr>
              <th className={th}>Tipificación</th>
              <th className={th}>Material</th>
              <th className={thr}>Piezas</th>
              <th className={thr}>Kg</th>
              <th className={thr}>Peso prom.</th>
              <th className={thr}>Partic.</th>
            </tr>
          </thead>
          <tbody>
            {data.tipificaciones.map((t) => (
              <tr key={t.tipificacionId} className="border-b border-border/60">
                <td className={td}>{limpio(t.descripcion)}</td>
                <td className={`${td} text-text-light`}>{limpio(t.materialNombre) || '—'}</td>
                <td className={tdr}>{t.piezas}</td>
                <td className={tdr}>{kg(t.kgFaena)}</td>
                <td className={tdr}>{kg(t.pesoPromedio)}</td>
                <td className={`${tdr} font-semibold`}>{pct(t.participacionKg)}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </Seccion>

      <Seccion titulo="Pesos y dispersión">
        <table className="w-full text-sm">
          <thead className="border-b border-border bg-gray-50 text-text-light">
            <tr>
              <th className={th}>Categoría</th>
              <th className={thr}>Piezas</th>
              <th className={thr}>Promedio</th>
              <th className={thr}>Mínimo</th>
              <th className={thr}>Máximo</th>
              <th className={thr}>Fuera de rango</th>
            </tr>
          </thead>
          <tbody>
            {data.dispersion.map((d) => (
              <tr key={d.tipoEspecieId} className="border-b border-border/60">
                <td className={td}>{d.tipoEspecieNombre}</td>
                <td className={tdr}>{d.piezas}</td>
                <td className={tdr}>{kg(d.pesoPromedio)}</td>
                <td className={tdr}>{kg(d.pesoMinimo)}</td>
                <td className={tdr}>{kg(d.pesoMaximo)}</td>
                <td className={`${tdr} ${d.piezasFueraRango > 0 ? 'font-semibold text-amber-600' : 'text-text-light'}`}>
                  {d.piezasFueraRango}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </Seccion>

      <Seccion titulo="Destino a cámaras">
        {data.camaras.length === 0 ? (
          <p className="px-4 py-6 text-sm text-text-light">
            La jornada todavía no fue liberada, así que no generó existencia en cámara.
          </p>
        ) : (
          <table className="w-full text-sm">
            <thead className="border-b border-border bg-gray-50 text-text-light">
              <tr>
                <th className={th}>Cámara</th>
                <th className={th}>Material</th>
                <th className={thr}>Cantidad</th>
                <th className={thr}>Kg</th>
              </tr>
            </thead>
            <tbody>
              {data.camaras.map((c, i) => (
                <tr key={`${c.almacenNombre}-${c.materialNombre}-${i}`} className="border-b border-border/60">
                  <td className={td}>{c.almacenNombre}</td>
                  <td className={td}>{c.materialNombre}</td>
                  <td className={tdr}>{c.cantidad}</td>
                  <td className={tdr}>{kg(c.peso)}</td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </Seccion>

      <div className="no-print mt-4 flex gap-2">
        <Button variant="secondary" size="sm" onClick={() => navigate('/operaciones/analisis-faena')}>
          Volver
        </Button>
        <Button variant="secondary" size="sm" onClick={() => window.print()}>
          Imprimir
        </Button>
        <Button
          variant="secondary"
          size="sm"
          onClick={() => navigate(`/operaciones/evaluacion-faena/${data.listaMatanzaId}`)}
        >
          Ver romaneos
        </Button>
      </div>
    </>
  )
}
