import { useState, useEffect } from 'react'
import { useNavigate } from 'react-router'
import { getListasMatanzas } from '@/services/listasMatanzas.service'
import { useToast } from '@/components/ui/Toast'
import { EstadoListaMatanza } from '@/types'
import type { ListaMatanzaListItem } from '@/types'
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

/**
 * Punto de entrada desde el menú para la Ejecución de Faena / Monitor.
 * Si hay una única LM En Ejecución, abre directo la pantalla que corresponde;
 * si hay varias, muestra un selector; si no hay ninguna, lo informa.
 */
export default function EjecucionFaenaHubPage({ target }: { target: Target }) {
  const navigate = useNavigate()
  const { toast } = useToast()
  const [listas, setListas] = useState<ListaMatanzaListItem[]>([])
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    let cancel = false
    void (async () => {
      try {
        const res = await getListasMatanzas({
          EstadoListaMatanzaId: EstadoListaMatanza.EnEjecucion,
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
  }, [navigate, toast, target])

  if (loading) return <div className="p-6 text-text-light">Cargando...</div>

  return (
    <>
      <PageHeader title={TITULO[target]} />
      <div className="rounded-lg border border-border bg-surface p-6 shadow-sm">
        <h3 className="mb-3 text-sm font-semibold text-text">Faenas en ejecucion</h3>
        {listas.length === 0 ? (
          <p className="text-sm text-text-light">No hay ninguna lista de matanza En Ejecucion.</p>
        ) : (
          <div className="divide-y divide-border">
            {listas.map((l) => (
              <div key={l.id} className="flex items-center justify-between py-3">
                <div>
                  <p className="font-medium text-text">
                    Lista N° {l.numeroLista} · {l.especieNombre}
                  </p>
                  <p className="text-xs text-text-light">{l.establecimientoNombre}</p>
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
