import { useState, useEffect, useCallback, useMemo } from 'react'
import { useParams, useNavigate } from 'react-router'
import {
  getRomaneosEvaluacion,
  previsualizarLiberacion,
  actualizarPieza,
  liberarJornada,
} from '@/services/evaluacionFaena.service'
import { getTipificaciones } from '@/services/tipificaciones.service'
import { useToast } from '@/components/ui/Toast'
import type {
  RomaneosEvaluacionResponse,
  PrevisualizacionLiberacion,
  PiezaEvaluacionItem,
  RomaneoEvaluacionItem,
} from '@/types/evaluacionFaena'
import type { Tipificacion } from '@/types/tipificacion'
import PageHeader from '@/components/ui/PageHeader'
import Button from '@/components/ui/Button'
import Badge from '@/components/ui/Badge'
import Modal from '@/components/ui/Modal'
import Input from '@/components/ui/Input'
import Select from '@/components/ui/Select'
import ConfirmDialog from '@/components/ui/ConfirmDialog'
import Spinner from '@/components/ui/Spinner'

const kg = (v: number) => v.toLocaleString('es-AR', { minimumFractionDigits: 2, maximumFractionDigits: 2 })

/** dd/MM/yyyy, el mismo formato que el resto de las pantallas. */
function formatFecha(value: string): string {
  if (!value) return ''
  return new Date(value).toLocaleDateString('es-AR', { day: '2-digit', month: '2-digit', year: 'numeric' })
}


/** Una fila por pieza: es el grano en el que se revisa y se corrige. */
interface FilaPieza {
  romaneo: RomaneoEvaluacionItem
  pieza: PiezaEvaluacionItem
}

export default function EvaluacionFaenaPage() {
  const { listaMatanzaId } = useParams<{ listaMatanzaId: string }>()
  const navigate = useNavigate()
  const { toast } = useToast()

  const [jornada, setJornada] = useState<RomaneosEvaluacionResponse | null>(null)
  const [previa, setPrevia] = useState<PrevisualizacionLiberacion | null>(null)
  const [tipificaciones, setTipificaciones] = useState<Tipificacion[]>([])
  const [isLoading, setIsLoading] = useState(true)

  const [editando, setEditando] = useState<FilaPieza | null>(null)
  const [form, setForm] = useState({ peso: '', tipificacionId: '', almacenDestinoId: '' })
  const [isSaving, setIsSaving] = useState(false)

  const [confirmarLiberar, setConfirmarLiberar] = useState(false)
  const [isLiberando, setIsLiberando] = useState(false)

  const fetchData = useCallback(async () => {
    if (!listaMatanzaId) return
    setIsLoading(true)
    try {
      const [j, p] = await Promise.all([
        getRomaneosEvaluacion(listaMatanzaId),
        previsualizarLiberacion(listaMatanzaId),
      ])
      setJornada(j)
      setPrevia(p)
      const t = await getTipificaciones({ EspecieId: j.especieId, Estado: true, PageSize: 500 })
      setTipificaciones(t.data || [])
    } catch (err) {
      toast('error', err instanceof Error ? err.message : 'Error al cargar la jornada')
    } finally {
      setIsLoading(false)
    }
  }, [listaMatanzaId, toast])

  useEffect(() => {
    void fetchData()
  }, [fetchData])

  // Las piezas con problema se marcan en la grilla, para ir directo a corregirlas.
  const problemasPorPieza = useMemo(() => {
    const map = new Map<string, string>()
    previa?.problemas.forEach((p) => map.set(p.piezaId, p.motivo))
    return map
  }, [previa])

  const filas = useMemo<FilaPieza[]>(
    () => (jornada?.data ?? []).flatMap((r) => r.piezas.map((pieza) => ({ romaneo: r, pieza }))),
    [jornada],
  )

  const abrirEdicion = (fila: FilaPieza) => {
    setEditando(fila)
    setForm({
      peso: String(fila.pieza.peso),
      tipificacionId: fila.pieza.tipificacionId ?? '',
      almacenDestinoId: fila.pieza.almacenDestinoId ?? '',
    })
  }

  const guardar = async (forzar: boolean) => {
    if (!editando) return
    setIsSaving(true)
    try {
      await actualizarPieza(editando.pieza.id, {
        Peso: Number(form.peso),
        TipificacionId: form.tipificacionId,
        AlmacenDestinoId: form.almacenDestinoId,
        ForzarFueraRango: forzar,
      })
      toast('success', 'Pieza actualizada')
      setEditando(null)
      await fetchData()
    } catch (err) {
      const msg = err instanceof Error ? err.message : 'Error al actualizar la pieza'
      // El backend pide confirmacion explicita para un peso fuera de rango.
      if (!forzar && msg.includes('fuera del rango')) {
        if (window.confirm(`${msg}\n\n¿Guardar de todos modos?`)) {
          setIsSaving(false)
          return guardar(true)
        }
      } else {
        toast('error', msg)
      }
    } finally {
      setIsSaving(false)
    }
  }

  const liberar = async () => {
    if (!listaMatanzaId) return
    setIsLiberando(true)
    try {
      const res = await liberarJornada(listaMatanzaId)
      toast(
        'success',
        `Jornada liberada: ${res.piezasLiberadas - res.piezasDecomisadas} piezas a cámara en ${res.movimientosGenerados} movimientos (${kg(res.kilosIngresados)} kg).` +
          (res.piezasDecomisadas > 0
            ? ` ${res.piezasDecomisadas} pieza(s) condenada(s) (${kg(res.kilosDecomisados)} kg) quedaron fijadas sin generar existencia.`
            : ''),
      )
      setConfirmarLiberar(false)
      await fetchData()
    } catch (err) {
      toast('error', err instanceof Error ? err.message : 'Error al liberar la jornada')
    } finally {
      setIsLiberando(false)
    }
  }

  if (isLoading) return <div className="p-6 text-text-light">Cargando...</div>
  if (!jornada) return <div className="p-6 text-text-light">Jornada no encontrada.</div>

  const opcionesTipificacion = tipificaciones.map((t) => ({
    value: t.codigo,
    label: `${t.descripcion} (${t.pesoDesde}-${t.pesoHasta} kg)${t.materialId ? '' : ' — sin material'}`,
  }))
  const opcionesCamara = jornada.camaras.map((c) => ({ value: c.id, label: c.nombre }))

  return (
    <>
      <div className="no-print">
        <PageHeader title={`Evaluación de Faena · Lista N° ${jornada.numeroLista}`} />
      </div>

      {/* Cabecera: sirve en pantalla y encabeza la planilla impresa. */}
      <div className="mb-4 rounded-lg border border-border bg-surface p-4 shadow-sm">
        <div className="flex flex-wrap items-center gap-x-6 gap-y-2 text-sm">
          <span>
            <span className="text-text-light">Establecimiento: </span>
            <span className="font-medium">{jornada.establecimientoNombre}</span>
          </span>
          <span>
            <span className="text-text-light">Puesto: </span>
            <span className="font-medium">
              {jornada.puestoNombre ? `${jornada.puestoCodigo} - ${jornada.puestoNombre}` : 'Sin asignar'}
            </span>
          </span>
          <span>
            <span className="text-text-light">Fecha: </span>
            <span className="font-medium">{formatFecha(jornada.fecha)}</span>
          </span>
          <span>
            <span className="text-text-light">Romaneos: </span>
            <span className="font-mono font-medium">{jornada.totalRomaneos}</span>
          </span>
          <span>
            <span className="text-text-light">Piezas: </span>
            <span className="font-mono font-medium">{jornada.totalPiezas}</span>
          </span>
          <span>
            <span className="text-text-light">Total: </span>
            <span className="font-mono font-medium">{kg(jornada.totalKg)} kg</span>
          </span>
          <Badge variant={jornada.estadoListaMatanzaId === 'FINALIZADA' ? 'success' : 'info'}>
            {jornada.estadoListaMatanzaId}
          </Badge>
          {jornada.piezasLiberadas > 0 && (
            <Badge variant="success">{jornada.piezasLiberadas} piezas liberadas</Badge>
          )}
          {jornada.totalKgDecomisados > 0 && (
            <Badge variant="danger">
              {jornada.totalDecomisosTotales > 0
                ? `${jornada.totalDecomisosTotales} res(es) condenada(s) · `
                : ''}
              {jornada.totalPiezasDecomisadas > 0
                ? `${jornada.totalPiezasDecomisadas} media(s) res(es) condenada(s) · `
                : ''}
              {kg(jornada.totalKgDecomisados)} kg decomisados
            </Badge>
          )}
        </div>
      </div>

      {/* Estado de la liberacion: que quedaria en camara y que la bloquea. */}
      {previa && (
        <div className="no-print mb-4 rounded-lg border border-border bg-surface p-4 shadow-sm">
          <div className="mb-3 flex flex-wrap items-center justify-between gap-3">
            <h3 className="text-sm font-semibold text-text">Liberación a cámara</h3>
            <div className="flex gap-2">
              <Button variant="secondary" size="sm" onClick={() => window.print()}>
                Imprimir
              </Button>
              <Button
                size="sm"
                disabled={!previa.puedeLiberar}
                onClick={() => setConfirmarLiberar(true)}
              >
                Liberar jornada
              </Button>
            </div>
          </div>

          {previa.motivoBloqueo ? (
            <p className="mb-3 rounded-md bg-amber-50 px-3 py-2 text-sm text-amber-800">
              {previa.motivoBloqueo}
            </p>
          ) : (
            <p className="mb-3 rounded-md bg-green-50 px-3 py-2 text-sm text-green-800">
              Listo para liberar: {previa.piezasAProcesar - previa.piezasDecomisadas} pieza(s) a
              cámara, {kg(previa.kilosAIngresar)} kg.
              {previa.piezasDecomisadas > 0 &&
                ` Otras ${previa.piezasDecomisadas} pieza(s) están condenadas (${kg(previa.kilosDecomisados)} kg) y se fijan sin entrar a cámara.`}
            </p>
          )}

          {previa.resumen.length > 0 && (
            <div className="mb-3">
              <p className="mb-1 text-xs font-medium text-text-light">Quedaría en cámara:</p>
              <ul className="space-y-1 text-sm">
                {previa.resumen.map((r) => (
                  <li key={`${r.almacenId}-${r.materialId}`} className="font-mono">
                    {r.cantidad} × {r.materialNombre}
                    <span className="text-text-light"> — {kg(r.peso)} kg en {r.almacenNombre}</span>
                  </li>
                ))}
              </ul>
            </div>
          )}

          {previa.problemas.length > 0 && (
            <div>
              <p className="mb-1 text-xs font-medium text-danger">
                {previa.problemas.length} pieza(s) impiden liberar:
              </p>
              <ul className="space-y-1 text-sm text-text-light">
                {previa.problemas.map((p) => (
                  <li key={p.piezaId}>
                    Romaneo {p.numeroRomaneo} · Garrón {p.numeroGarron}
                    {p.letra ? ` (${p.letra})` : ''}: {p.motivo}
                  </li>
                ))}
              </ul>
            </div>
          )}
        </div>
      )}

      {/* Planilla de romaneo: una fila por pieza. */}
      <div className="overflow-x-auto rounded-lg border border-border bg-surface shadow-sm">
        <table className="w-full text-sm">
          <thead className="border-b border-border bg-gray-50 text-left text-text-light">
            <tr>
              <th className="px-3 py-2 font-medium">Romaneo</th>
              <th className="px-3 py-2 font-medium">Garrón</th>
              <th className="px-3 py-2 font-medium">Tropa</th>
              <th className="px-3 py-2 font-medium">Categoría</th>
              <th className="px-3 py-2 text-right font-medium">Peso (kg)</th>
              <th className="px-3 py-2 font-medium">Tipificación</th>
              <th className="px-3 py-2 font-medium">Material</th>
              <th className="px-3 py-2 font-medium">Cámara</th>
              <th className="px-3 py-2 font-medium">Estado</th>
              <th className="no-print px-3 py-2" />
            </tr>
          </thead>
          <tbody>
            {filas.length === 0 && (
              <tr>
                <td colSpan={10} className="px-3 py-6 text-center text-text-light">
                  La jornada no tiene romaneos.
                </td>
              </tr>
            )}
            {filas.map(({ romaneo, pieza }) => {
              const problema = problemasPorPieza.get(pieza.id)
              return (
                <tr
                  key={pieza.id}
                  className={`border-b border-border/60 ${romaneo.anulado ? 'opacity-50' : ''} ${problema ? 'bg-red-50' : ''}`}
                >
                  <td className="px-3 py-2 font-mono">{romaneo.numeroRomaneo}</td>
                  <td className="px-3 py-2 font-mono">
                    {romaneo.numeroGarron}
                    {pieza.letra ? ` ${pieza.letra}` : ''}
                  </td>
                  <td className="px-3 py-2 font-mono text-text-light">{romaneo.numeroTropa}</td>
                  <td className="px-3 py-2">{romaneo.tipoEspecieNombre}</td>
                  <td className={`px-3 py-2 text-right font-mono ${pieza.pesoFueraRango ? 'text-amber-600' : ''}`}>
                    {kg(pieza.peso)}
                  </td>
                  <td className="px-3 py-2">
                    {romaneo.decomisoTotal || pieza.decomisada ? (
                      <span className="text-danger">
                        {romaneo.decomisoTotal ? 'Res condenada' : 'Media res condenada'}
                        {(romaneo.decomisoTotal
                          ? romaneo.motivoDecomisoNombre
                          : pieza.motivoDecomisoNombre)
                          ? `: ${romaneo.decomisoTotal ? romaneo.motivoDecomisoNombre : pieza.motivoDecomisoNombre}`
                          : ''}
                      </span>
                    ) : (
                      <>
                        {pieza.tipificacionDescripcion ?? pieza.tipificacionId}
                        {pieza.motivoDecomisoNombre && (
                          <span className="ml-1 text-xs text-danger">
                            (−{kg(pieza.pesoDecomisado)} kg · {pieza.motivoDecomisoNombre})
                          </span>
                        )}
                      </>
                    )}
                  </td>
                  <td className="px-3 py-2">
                    {romaneo.decomisoTotal || pieza.decomisada ? (
                      <span className="text-text-light">no entra a cámara</span>
                    ) : (
                      pieza.materialNombre ?? <span className="text-danger">sin material</span>
                    )}
                  </td>
                  <td className="px-3 py-2 text-text-light">{pieza.almacenDestinoNombre}</td>
                  <td className="px-3 py-2">
                    {romaneo.anulado ? (
                      <Badge variant="danger">Anulado</Badge>
                    ) : pieza.liberado ? (
                      <Badge variant="success">Liberada</Badge>
                    ) : problema ? (
                      <span className="text-xs text-danger">{problema}</span>
                    ) : (
                      <Badge variant="neutral">Pendiente</Badge>
                    )}
                  </td>
                  <td className="no-print px-3 py-2">
                    {!romaneo.anulado && !pieza.liberado && (
                      <button
                        type="button"
                        onClick={() => abrirEdicion({ romaneo, pieza })}
                        className="text-sm font-medium text-primary-600 hover:text-primary-800 hover:underline"
                      >
                        Editar
                      </button>
                    )}
                  </td>
                </tr>
              )
            })}
          </tbody>
        </table>
      </div>

      <div className="no-print mt-4 flex gap-2">
        <Button variant="secondary" size="sm" onClick={() => navigate('/operaciones/evaluacion-faena')}>
          Volver
        </Button>
        <Button
          variant="secondary"
          size="sm"
          onClick={() => navigate(`/operaciones/analisis-faena/${jornada.listaMatanzaId}`)}
        >
          Ver análisis
        </Button>
      </div>

      <Modal
        isOpen={editando !== null}
        onClose={() => setEditando(null)}
        title={
          editando
            ? `Romaneo ${editando.romaneo.numeroRomaneo} · Garrón ${editando.romaneo.numeroGarron}${editando.pieza.letra ? ` (${editando.pieza.letra})` : ''}`
            : ''
        }
      >
        {isSaving ? (
          <div className="flex justify-center py-8">
            <Spinner />
          </div>
        ) : (
          <div className="space-y-4">
            <Input
              label="Peso (kg)"
              type="number"
              step="0.01"
              value={form.peso}
              onChange={(e) => setForm({ ...form, peso: e.target.value })}
            />
            {editando?.romaneo.decomisoTotal || editando?.pieza.decomisada ? (
              <p className="rounded-md bg-red-50 px-3 py-2 text-sm text-danger">
                {editando.romaneo.decomisoTotal ? 'Res condenada' : 'Media res condenada'}
                {(editando.romaneo.decomisoTotal
                  ? editando.romaneo.motivoDecomisoNombre
                  : editando.pieza.motivoDecomisoNombre)
                  ? `: ${editando.romaneo.decomisoTotal ? editando.romaneo.motivoDecomisoNombre : editando.pieza.motivoDecomisoNombre}`
                  : ''}
                . No se tipifica ni va a cámara, así que lo único corregible es el peso.
              </p>
            ) : (
              <>
                <Select
                  label="Tipificación"
                  options={opcionesTipificacion}
                  value={form.tipificacionId}
                  onChange={(e) => setForm({ ...form, tipificacionId: e.target.value })}
                />
                <Select
                  label="Cámara destino"
                  options={opcionesCamara}
                  value={form.almacenDestinoId}
                  onChange={(e) => setForm({ ...form, almacenDestinoId: e.target.value })}
                />
              </>
            )}
            <div className="flex justify-end gap-3 pt-2">
              <Button variant="secondary" onClick={() => setEditando(null)}>
                Cancelar
              </Button>
              <Button onClick={() => void guardar(false)}>Guardar</Button>
            </div>
          </div>
        )}
      </Modal>

      <ConfirmDialog
        isOpen={confirmarLiberar}
        onConfirm={() => void liberar()}
        onCancel={() => setConfirmarLiberar(false)}
        title="Liberar la jornada"
        message={
          previa
            ? `Se van a generar ${previa.piezasAProcesar - previa.piezasDecomisadas} pieza(s) de existencia en cámara (${kg(previa.kilosAIngresar)} kg).` +
              (previa.piezasDecomisadas > 0
                ? ` Las ${previa.piezasDecomisadas} pieza(s) condenada(s) quedan fijadas sin entrar a cámara.`
                : '') +
              ' Los romaneos quedan definitivos y no se van a poder editar. Esta acción no se puede deshacer.'
            : ''
        }
        confirmLabel="Liberar"
        confirmVariant="primary"
        isLoading={isLiberando}
      />
    </>
  )
}
