import { useState } from 'react'
import type { FormEvent } from 'react'
import { getTrazabilidadTropa } from '@/services/trazabilidadTropas.service'
import { useApp } from '@/contexts/AppContext'
import { useToast } from '@/components/ui/Toast'
import type { TrazabilidadTropa } from '@/types'
import PageHeader from '@/components/ui/PageHeader'
import Input from '@/components/ui/Input'
import Button from '@/components/ui/Button'
import Badge from '@/components/ui/Badge'

const movimientoLabel: Record<string, string> = {
  // Log propio de la tropa
  RECEPCION: 'Recepción',
  UBICACION: 'Ubicación en corral',
  ANULACION: 'Anulación',
  // Historial de planificación (ListaMatanzaMovimiento)
  ALTA_TROPA: 'Alta en plan de faena',
  BAJA_TROPA: 'Baja del plan de faena',
  INCREMENTO: 'Incremento de cantidad',
  DECREMENTO: 'Decremento de cantidad',
  CAMBIO_SECUENCIA: 'Cambio de secuencia',
  DIVISION: 'División de renglón',
  FUSION: 'Fusión de renglón',
  FAENA_EMERGENCIA: 'Faena de emergencia',
  CONFIRMACION: 'Confirmación del plan',
  DESCONFIRMACION: 'Vuelta a borrador',
  CANCELACION: 'Anulación del plan',
  INICIO: 'Inicio de ejecución',
  FINALIZACION: 'Cierre del plan',
  LIBERACION: 'Liberación de sobrante',
}

function faseDotClass(fase: string): string {
  if (fase === 'Planificación') return 'border-amber-500'
  if (fase === 'Anulación') return 'border-danger'
  return 'border-primary-500'
}

function estadoVariant(estado: string): 'success' | 'danger' | 'neutral' {
  if (estado === 'RECEPCIONADA') return 'success'
  if (estado === 'ANULADA') return 'danger'
  return 'neutral'
}

function formatFechaHora(value: string): string {
  if (!value) return ''
  return new Date(value).toLocaleString('es-AR', {
    day: '2-digit', month: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit', hour12: false,
  })
}

export default function TrazabilidadTropasPage() {
  const { toast } = useToast()
  const { currentEstablecimiento } = useApp()
  const [numero, setNumero] = useState('')
  const [isLoading, setIsLoading] = useState(false)
  const [searched, setSearched] = useState(false)
  const [results, setResults] = useState<TrazabilidadTropa[]>([])

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault()
    const parsed = Number(numero)
    if (!numero.trim() || !Number.isFinite(parsed) || parsed <= 0) {
      toast('error', 'Ingrese un número de tropa válido')
      return
    }

    setIsLoading(true)
    try {
      const data = await getTrazabilidadTropa({
        NumeroTropa: parsed,
        EstablecimientoId: currentEstablecimiento?.id,
      })
      setResults(data)
      setSearched(true)
    } catch {
      toast('error', 'Error al consultar la trazabilidad de la tropa')
    } finally {
      setIsLoading(false)
    }
  }

  return (
    <>
      <PageHeader title="Trazabilidad de Tropas" />

      <form onSubmit={handleSubmit} className="mb-6 flex items-end gap-3">
        <div className="w-48">
          <Input
            label="Número de Tropa"
            type="number"
            min={1}
            value={numero}
            onChange={(e) => setNumero(e.target.value)}
            placeholder="Ej: 1024"
          />
        </div>
        <Button type="submit" loading={isLoading}>
          Buscar
        </Button>
      </form>

      {searched && !isLoading && results.length === 0 && (
        <div className="rounded-lg border border-border bg-surface p-6 text-center text-sm text-text-light">
          No se encontró ninguna tropa con el número ingresado
          {currentEstablecimiento ? ` en ${currentEstablecimiento.nombre}` : ''}.
        </div>
      )}

      <div className="space-y-5">
        {results.map((tropa) => (
          <div key={tropa.tropaId} className="rounded-lg border border-border bg-surface p-5 shadow-sm">
            {/* Cabecera de la tropa */}
            <div className="mb-4 flex flex-wrap items-center justify-between gap-3 border-b border-border pb-3">
              <div>
                <div className="flex items-center gap-2">
                  <h3 className="text-lg font-semibold text-text">Tropa N° {tropa.numeroTropa}</h3>
                  <Badge variant={estadoVariant(tropa.estadoTropaId)}>{tropa.estadoTropaNombre}</Badge>
                </div>
                <p className="mt-1 text-sm text-text-light">
                  {tropa.especieNombre} · {tropa.clienteNombre} · {tropa.establecimientoNombre} · Ingreso #{tropa.numeroIngreso}
                </p>
              </div>
            </div>

            {/* Linea de tiempo de eventos */}
            {tropa.movimientos.length === 0 ? (
              <p className="text-sm text-text-light">Sin eventos registrados.</p>
            ) : (
              <ol className="relative ml-2 border-l-2 border-border">
                {tropa.movimientos.map((m, idx) => (
                  <li key={idx} className="relative mb-5 pl-6 last:mb-0">
                    <span className={`absolute -left-[7px] top-1 h-3 w-3 rounded-full border-2 bg-surface ${faseDotClass(m.fase)}`} />
                    <div className="flex flex-wrap items-baseline gap-x-2">
                      <span className="font-medium text-text">
                        {movimientoLabel[m.tipoMovimiento] ?? m.tipoMovimiento}
                      </span>
                      {m.referencia && (
                        <span className="rounded bg-amber-50 px-1.5 py-0.5 text-xs font-medium text-amber-700">{m.referencia}</span>
                      )}
                      <span className="font-mono text-xs text-text-light">{formatFechaHora(m.fecha)}</span>
                    </div>
                    {m.detalle && <p className="mt-0.5 text-sm text-text-light">{m.detalle}</p>}
                    {m.usuarioNombre && (
                      <p className="mt-0.5 text-xs text-muted">Usuario: {m.usuarioNombre}</p>
                    )}
                  </li>
                ))}
              </ol>
            )}
          </div>
        ))}
      </div>
    </>
  )
}
