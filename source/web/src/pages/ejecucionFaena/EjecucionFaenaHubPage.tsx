import { useState, useEffect, useCallback } from 'react'
import { useNavigate } from 'react-router'
import { getListasMatanzas } from '@/services/listasMatanzas.service'
import { getPuestosOptions } from '@/services/puestos.service'
import { useApp } from '@/contexts/AppContext'
import { useToast } from '@/components/ui/Toast'
import { EstadoListaMatanza } from '@/types'
import type { ListaMatanzaListItem, Puesto } from '@/types'
import PageHeader from '@/components/ui/PageHeader'
import Button from '@/components/ui/Button'

type Target = 'tipificador' | 'monitor'

const TITULO: Record<Target, string> = {
  tipificador: 'Ejecución de Faena',
  monitor: 'Monitor de Faena',
}

/** Cada destino cuelga de su propia ruta de menu para que el item activo del sidebar sea el correcto. */
const rutaDestino = (target: Target, listaMatanzaId: string) =>
  target === 'monitor'
    ? `/operaciones/monitor-faena/${listaMatanzaId}`
    : `/operaciones/ejecucion-faena/${listaMatanzaId}/tipificador`

/** El palco trabaja siempre en el mismo puesto: se recuerda el elegido en esta terminal. */
const PUESTO_STORAGE_KEY = 'puestoEjecucionFaena'

/**
 * Punto de entrada desde el menú para la Ejecución de Faena / Monitor.
 *
 * Se entra por el puesto: elegido un palco, solo se ven las listas de matanza que tienen ese
 * puesto asignado. Si hay una única lista En Ejecución, abre directo la pantalla que
 * corresponde; si hay varias, muestra un selector; si no hay ninguna, lo informa.
 */
export default function EjecucionFaenaHubPage({ target }: { target: Target }) {
  const navigate = useNavigate()
  const { toast } = useToast()
  const { currentEstablecimiento } = useApp()
  const [listas, setListas] = useState<ListaMatanzaListItem[]>([])
  const [puestos, setPuestos] = useState<Puesto[]>([])
  const [puestoId, setPuestoId] = useState<string>(
    () => localStorage.getItem(PUESTO_STORAGE_KEY) ?? '',
  )
  const [loading, setLoading] = useState(true)

  // Puestos del establecimiento activo (todas sus especies: el palco elige su especie al faenar).
  useEffect(() => {
    if (!currentEstablecimiento?.id) return
    let cancel = false
    void (async () => {
      try {
        const data = await getPuestosOptions(currentEstablecimiento.id)
        if (cancel) return
        setPuestos(data)
        // Un puesto que ya no existe (o no es de este establecimiento) deja de estar elegido.
        setPuestoId((prev) => (prev && data.some((p) => p.id === prev) ? prev : ''))
      } catch {
        if (!cancel) setPuestos([])
      }
    })()
    return () => {
      cancel = true
    }
  }, [currentEstablecimiento?.id])

  const elegirPuesto = useCallback((valor: string) => {
    setPuestoId(valor)
    if (valor) localStorage.setItem(PUESTO_STORAGE_KEY, valor)
    else localStorage.removeItem(PUESTO_STORAGE_KEY)
  }, [])

  useEffect(() => {
    let cancel = false
    setLoading(true)
    void (async () => {
      try {
        const res = await getListasMatanzas({
          EstadoListaMatanzaId: EstadoListaMatanza.EnEjecucion,
          PuestoId: puestoId || undefined,
          PageSize: 200,
        })
        const data = res.data || []
        if (cancel) return
        const unica = data.length === 1 ? data[0] : undefined
        if (unica) {
          navigate(rutaDestino(target, unica.id), { replace: true })
          return
        }
        setListas(data)
      } catch (err) {
        if (!cancel) toast('error', err instanceof Error ? err.message : 'Error al cargar las faenas en curso')
      } finally {
        if (!cancel) setLoading(false)
      }
    })()
    return () => {
      cancel = true
    }
  }, [navigate, toast, target, puestoId])

  return (
    <>
      <PageHeader title={TITULO[target]} />

      {puestos.length > 0 && (
        <div className="mb-4 rounded-lg border border-border bg-surface p-4 shadow-sm">
          <label className="mb-1 block text-sm font-medium text-text">Puesto</label>
          <select
            className="w-full max-w-sm rounded-lg border border-border px-3 py-2 text-sm"
            value={puestoId}
            onChange={(e) => elegirPuesto(e.target.value)}
          >
            <option value="">Todos los puestos</option>
            {puestos.map((p) => (
              <option key={p.id} value={p.id}>
                {p.codigoPuesto} - {p.nombre} ({p.especieNombre ?? p.especieId})
              </option>
            ))}
          </select>
          <p className="mt-1 text-xs text-text-light">
            Elegido un puesto, solo se ven las listas de matanza asignadas a ese palco.
          </p>
        </div>
      )}

      <div className="rounded-lg border border-border bg-surface p-6 shadow-sm">
        <h3 className="mb-3 text-sm font-semibold text-text">Faenas en ejecucion</h3>
        {loading ? (
          <p className="text-sm text-text-light">Cargando...</p>
        ) : listas.length === 0 ? (
          <p className="text-sm text-text-light">
            {puestoId
              ? 'No hay ninguna lista de matanza En Ejecucion asignada a este puesto.'
              : 'No hay ninguna lista de matanza En Ejecucion.'}
          </p>
        ) : (
          <div className="divide-y divide-border">
            {listas.map((l) => (
              <div key={l.id} className="flex items-center justify-between py-3">
                <div>
                  <p className="font-medium text-text">
                    Lista N° {l.numeroLista} · {l.especieNombre}
                  </p>
                  <p className="text-xs text-text-light">
                    {l.establecimientoNombre}
                    {l.puestoCodigo ? ` · Puesto ${l.puestoCodigo}` : ''}
                  </p>
                </div>
                <div className="flex gap-2">
                  {target === 'tipificador' ? (
                    <>
                      <Button size="sm" onClick={() => navigate(rutaDestino('tipificador', l.id))}>
                        Tipificar
                      </Button>
                      <Button variant="secondary" size="sm" onClick={() => navigate(rutaDestino('monitor', l.id))}>
                        Monitor
                      </Button>
                    </>
                  ) : (
                    <>
                      <Button size="sm" onClick={() => navigate(rutaDestino('monitor', l.id))}>
                        Ver Monitor
                      </Button>
                      <Button variant="secondary" size="sm" onClick={() => navigate(rutaDestino('tipificador', l.id))}>
                        Tipificar
                      </Button>
                    </>
                  )}
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </>
  )
}
