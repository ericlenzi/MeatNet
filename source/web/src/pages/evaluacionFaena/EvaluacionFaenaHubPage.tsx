import { useState, useEffect } from 'react'
import { useNavigate } from 'react-router'
import { getListasMatanzas } from '@/services/listasMatanzas.service'
import { useToast } from '@/components/ui/Toast'
import { EstadoListaMatanza } from '@/types'
import type { ListaMatanzaListItem } from '@/types'
import PageHeader from '@/components/ui/PageHeader'
import Button from '@/components/ui/Button'
import Badge from '@/components/ui/Badge'

/**
 * Punto de entrada desde el menu para la Evaluacion de Faena. Ofrece las jornadas
 * Finalizadas (las unicas que se pueden liberar) y tambien las En Ejecucion, para
 * poder revisarlas y anticipar problemas antes de cerrarlas.
 */
export default function EvaluacionFaenaHubPage() {
  const navigate = useNavigate()
  const { toast } = useToast()
  const [listas, setListas] = useState<ListaMatanzaListItem[]>([])
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    let cancel = false
    void (async () => {
      try {
        const [finalizadas, enEjecucion] = await Promise.all([
          getListasMatanzas({ EstadoListaMatanzaId: EstadoListaMatanza.Finalizada, PageSize: 200 }),
          getListasMatanzas({ EstadoListaMatanzaId: EstadoListaMatanza.EnEjecucion, PageSize: 200 }),
        ])
        if (cancel) return
        // Las finalizadas primero: son las que estan listas para liberar.
        setListas([...(finalizadas.data || []), ...(enEjecucion.data || [])])
      } catch (err) {
        if (!cancel) toast('error', err instanceof Error ? err.message : 'Error al cargar las jornadas')
      } finally {
        if (!cancel) setLoading(false)
      }
    })()
    return () => {
      cancel = true
    }
  }, [toast])

  if (loading) return <div className="p-6 text-text-light">Cargando...</div>

  return (
    <>
      <PageHeader title="Evaluación de Faena" />
      <div className="rounded-lg border border-border bg-surface p-6 shadow-sm">
        <h3 className="mb-3 text-sm font-semibold text-text">Jornadas para evaluar</h3>
        {listas.length === 0 ? (
          <p className="text-sm text-text-light">
            No hay jornadas finalizadas ni en ejecución para evaluar.
          </p>
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
                <div className="flex items-center gap-3">
                  <Badge variant={l.estadoListaMatanzaId === EstadoListaMatanza.Finalizada ? 'success' : 'info'}>
                    {l.estadoListaMatanzaId === EstadoListaMatanza.Finalizada ? 'Finalizada' : 'En ejecución'}
                  </Badge>
                  <Button size="sm" onClick={() => navigate(`/operaciones/evaluacion-faena/${l.id}`)}>
                    Evaluar
                  </Button>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </>
  )
}
