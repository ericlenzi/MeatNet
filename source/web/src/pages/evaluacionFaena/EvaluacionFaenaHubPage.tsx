import { useState, useEffect, useCallback } from 'react'
import { useNavigate } from 'react-router'
import { getListasMatanzas } from '@/services/listasMatanzas.service'
import { useToast } from '@/components/ui/Toast'
import { EstadoListaMatanza } from '@/types'
import type { ListaMatanzaListItem } from '@/types'
import PageHeader from '@/components/ui/PageHeader'
import Button from '@/components/ui/Button'
import Badge from '@/components/ui/Badge'
import Input from '@/components/ui/Input'
import Select from '@/components/ui/Select'

type Target = 'evaluacion' | 'analisis'

const TITULO: Record<Target, string> = {
  evaluacion: 'Evaluación de Faena',
  analisis: 'Análisis de Faena',
}

// Mismos colores y etiquetas de estado que el listado de Planificacion: es la misma lista
// mirada desde otro paso, y dos leyendas distintas para el mismo estado confunden.
const estadoVariant: Record<string, 'success' | 'danger' | 'neutral' | 'info'> = {
  [EstadoListaMatanza.Confirmada]: 'info',
  [EstadoListaMatanza.EnEjecucion]: 'info',
  [EstadoListaMatanza.Finalizada]: 'success',
  [EstadoListaMatanza.Anulada]: 'danger',
}

// Sin Borrador: una lista en Borrador no se planifico todavia, no tiene romaneos y no hay nada
// que evaluar ni analizar. Tampoco entra en el listado cuando no se filtra por estado.
const estadoOptions = [
  { value: EstadoListaMatanza.Confirmada, label: 'Confirmada' },
  { value: EstadoListaMatanza.EnEjecucion, label: 'En Ejecucion' },
  { value: EstadoListaMatanza.Finalizada, label: 'Finalizada' },
  { value: EstadoListaMatanza.Anulada, label: 'Anulada' },
]

/** Filas que muestra el listado; el resto se alcanza con los filtros. */
const MAX_FILAS = 20

/** dd/MM/yyyy, el mismo formato que el resto de las pantallas. */
function formatFecha(value: string): string {
  if (!value) return ''
  return new Date(value).toLocaleDateString('es-AR', { day: '2-digit', month: '2-digit', year: 'numeric' })
}

/** Cada destino cuelga de su ruta de menu para que el item activo del sidebar sea el correcto. */
const rutaDestino = (target: Target, listaMatanzaId: string) =>
  target === 'analisis'
    ? `/operaciones/analisis-faena/${listaMatanzaId}`
    : `/operaciones/evaluacion-faena/${listaMatanzaId}`

/**
 * Punto de entrada desde el menu para la Evaluacion y el Analisis de Faena.
 *
 * Los dos filtros arrancan sin filtrar: todas las fechas y todos los estados menos Borrador.
 * La jornada Finalizada es la unica que se puede liberar, pero la lista no esconde al resto:
 * la En Ejecucion se mira para anticipar problemas antes de cerrarla.
 */
export default function EvaluacionFaenaHubPage({ target = 'evaluacion' }: { target?: Target }) {
  const navigate = useNavigate()
  const { toast } = useToast()
  const [listas, setListas] = useState<ListaMatanzaListItem[]>([])
  const [total, setTotal] = useState(0)
  const [loading, setLoading] = useState(true)
  const [fecha, setFecha] = useState('')
  // El check y la fecha son las dos caras del mismo filtro: tildar el check limpia la fecha, y
  // elegir una fecha lo destilda. Sin fecha, el listado trae todas.
  const [todasLasFechas, setTodasLasFechas] = useState(true)
  const [estado, setEstado] = useState('')

  const elegirFecha = (valor: string) => {
    setFecha(valor)
    if (valor) setTodasLasFechas(false)
  }

  const marcarTodasLasFechas = (marcado: boolean) => {
    setTodasLasFechas(marcado)
    if (marcado) setFecha('')
  }

  const cargar = useCallback(async () => {
    setLoading(true)
    try {
      const res = await getListasMatanzas({
        EstadoListaMatanzaId: estado || undefined,
        Fecha: fecha || undefined,
        PageSize: 200,
      })

      const jornadas = (res.data || [])
        // El Borrador no se muestra ni cuando no se filtra por estado: no tiene romaneos.
        .filter((l) => l.estadoListaMatanzaId !== EstadoListaMatanza.Borrador)
        // La jornada mas reciente primero, y a igual fecha la lista de numero mas alto.
        .sort((a, b) => {
          const dif = new Date(b.fecha).getTime() - new Date(a.fecha).getTime()
          return dif !== 0 ? dif : Number(b.numeroLista) - Number(a.numeroLista)
        })

      // La pantalla es un punto de entrada, no un listado para recorrer: muestra las mas
      // recientes y la leyenda dice cuantas quedaron afuera. Para buscar una vieja estan los
      // filtros de fecha y estado.
      setTotal(jornadas.length)
      setListas(jornadas.slice(0, MAX_FILAS))
    } catch (err) {
      toast('error', err instanceof Error ? err.message : 'Error al cargar las jornadas')
    } finally {
      setLoading(false)
    }
  }, [estado, fecha, toast])

  useEffect(() => {
    void cargar()
  }, [cargar])

  const filtrando = !!fecha || !!estado

  return (
    <>
      <PageHeader title={TITULO[target]} />

      <div className="mb-4 rounded-lg border border-border bg-surface p-4 shadow-sm">
        <div className="flex flex-wrap items-end gap-3">
          <div className="w-44">
            <Input
              label="Fecha de faena"
              type="date"
              value={fecha}
              onChange={(e) => elegirFecha(e.target.value)}
            />
          </div>
          <label className="flex items-center gap-2 pb-2.5 text-sm text-text">
            <input
              type="checkbox"
              checked={todasLasFechas}
              onChange={(e) => marcarTodasLasFechas(e.target.checked)}
              className="h-4 w-4 rounded border-border text-primary-600 focus:ring-primary-500"
            />
            Todas las fechas
          </label>
          <div className="w-56">
            <Select
              label="Estado"
              value={estado}
              onChange={(e) => setEstado(e.target.value)}
              options={estadoOptions}
              placeholder="Seleccionar..."
            />
          </div>
        </div>
      </div>

      <div className="rounded-lg border border-border bg-surface p-6 shadow-sm">
        <h3 className="mb-3 text-sm font-semibold text-text">Jornadas de faena</h3>
        {loading ? (
          <p className="text-sm text-text-light">Cargando...</p>
        ) : listas.length === 0 ? (
          <p className="text-sm text-text-light">
            {filtrando
              ? 'No hay jornadas con los filtros elegidos.'
              : 'No hay listas de matanza cargadas.'}
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
                    {` · ${formatFecha(l.fecha)}`}
                  </p>
                </div>
                <div className="flex items-center gap-3">
                  <Badge variant={estadoVariant[l.estadoListaMatanzaId] ?? 'neutral'}>
                    {l.estadoListaMatanzaNombre}
                  </Badge>
                  <Button size="sm" onClick={() => navigate(rutaDestino(target, l.id))}>
                    {target === 'analisis' ? 'Analizar' : 'Evaluar'}
                  </Button>
                </div>
              </div>
            ))}
          </div>
        )}

        {!loading && total > 0 && (
          <p className="mt-3 text-right text-xs text-text-light">
            {total > MAX_FILAS
              ? `Mostrando las ${MAX_FILAS} jornadas más recientes de ${total}. Use los filtros para ver las demás.`
              : `${total} ${total === 1 ? 'jornada' : 'jornadas'}`}
          </p>
        )}
      </div>
    </>
  )
}
