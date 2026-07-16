import { useState, useEffect, useCallback } from 'react'
import { useNavigate, useParams } from 'react-router'
import {
  getListaMatanza,
  getListasMatanzas,
  getDisponibilidadFaena,
  agregarRenglonListaMatanza,
  editarRenglonListaMatanza,
  quitarRenglonListaMatanza,
  faenaEmergenciaListaMatanza,
  confirmarListaMatanza,
  desconfirmarListaMatanza,
  iniciarListaMatanza,
  finalizarListaMatanza,
  cancelarListaMatanza,
} from '@/services/listasMatanzas.service'
import { useToast } from '@/components/ui/Toast'
import { EstadoListaMatanza } from '@/types'
import type { ListaMatanza, ListaMatanzaRenglon, DisponibilidadFaenaItem } from '@/types'
import PageHeader from '@/components/ui/PageHeader'
import Button from '@/components/ui/Button'
import Badge from '@/components/ui/Badge'
import ConfirmDialog from '@/components/ui/ConfirmDialog'
import Modal from '@/components/ui/Modal'

const estadoVariant: Record<string, 'success' | 'danger' | 'neutral' | 'info'> = {
  [EstadoListaMatanza.Borrador]: 'neutral',
  [EstadoListaMatanza.Confirmada]: 'info',
  [EstadoListaMatanza.EnEjecucion]: 'info',
  [EstadoListaMatanza.Finalizada]: 'success',
  [EstadoListaMatanza.Anulada]: 'danger',
}

const movimientoLabel: Record<string, string> = {
  ALTA_TROPA: 'Alta de tropa',
  BAJA_TROPA: 'Baja de tropa',
  INCREMENTO: 'Incremento',
  DECREMENTO: 'Decremento',
  CAMBIO_SECUENCIA: 'Cambio de secuencia',
  DIVISION: 'Division',
  FUSION: 'Fusion',
  FAENA_EMERGENCIA: 'Faena de emergencia',
  CONFIRMACION: 'Confirmacion',
  DESCONFIRMACION: 'Vuelta a Borrador',
  CANCELACION: 'Anulacion',
  INICIO: 'Inicio de ejecucion',
  FINALIZACION: 'Cierre de lista',
  LIBERACION: 'Liberacion de sobrante',
}

interface RowEdit {
  cantidad: number
  secuencia: number
}

function formatFecha(value: string | null): string {
  if (!value) return ''
  return new Date(value).toLocaleDateString('es-AR', { day: '2-digit', month: '2-digit', year: 'numeric' })
}

function formatFechaHora(value: string): string {
  if (!value) return ''
  return new Date(value).toLocaleString('es-AR', {
    day: '2-digit', month: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit', hour12: false,
  })
}

export default function ListaMatanzaDetailPage() {
  const { id } = useParams()
  const navigate = useNavigate()
  const { toast } = useToast()

  const [lm, setLm] = useState<ListaMatanza | null>(null)
  const [disponibilidad, setDisponibilidad] = useState<DisponibilidadFaenaItem[]>([])
  const [rowEdits, setRowEdits] = useState<Record<string, RowEdit>>({})
  const [loading, setLoading] = useState(true)
  const [acting, setActing] = useState(false)
  const [iniciarBloqueado, setIniciarBloqueado] = useState<string | null>(null)
  const [showCancel, setShowCancel] = useState(false)
  const [showCierre, setShowCierre] = useState(false)
  const [motivoCierre, setMotivoCierre] = useState('')
  const [quitarTarget, setQuitarTarget] = useState<ListaMatanzaRenglon | null>(null)

  const fetchData = useCallback(async () => {
    if (!id) return
    setLoading(true)
    try {
      const data = await getListaMatanza(id)
      setLm(data)
      setRowEdits({})

      // R-18: para poder Iniciar, ninguna otra LM del mismo establecimiento+especie
      // puede estar En Ejecucion (un solo ciclo de faena a la vez).
      if (data.estadoListaMatanzaId === EstadoListaMatanza.Confirmada) {
        try {
          const res = await getListasMatanzas({
            EstablecimientoId: data.establecimientoId,
            EspecieId: data.especieId,
            EstadoListaMatanzaId: EstadoListaMatanza.EnEjecucion,
            PageSize: 200,
          })
          const otra = (res.data || []).find((x) => x.id !== data.id)
          setIniciarBloqueado(
            otra
              ? `La lista N° ${otra.numeroLista} ya esta En Ejecucion para esta especie. Debe finalizarla antes de iniciar otra faena.`
              : null,
          )
        } catch {
          setIniciarBloqueado(null)
        }
      } else {
        setIniciarBloqueado(null)
      }

      // Con la lista Confirmada / En Ejecucion se puede seguir agregando tropas
      // (alta controlada / emergencia): cargar el disponible actual.
      if (
        data.estadoListaMatanzaId === EstadoListaMatanza.Confirmada ||
        data.estadoListaMatanzaId === EstadoListaMatanza.EnEjecucion
      ) {
        try {
          setDisponibilidad(
            await getDisponibilidadFaena({
              EstablecimientoId: data.establecimientoId,
              EspecieId: data.especieId,
            }),
          )
        } catch {
          setDisponibilidad([])
        }
      } else {
        setDisponibilidad([])
      }
    } catch {
      toast('error', 'Error al cargar la lista de matanza')
    } finally {
      setLoading(false)
    }
  }, [id, toast])

  useEffect(() => {
    void fetchData()
  }, [fetchData])

  const runAction = async (fn: () => Promise<unknown>, okMsg: string) => {
    setActing(true)
    try {
      await fn()
      toast('success', okMsg)
      await fetchData()
    } catch (err) {
      toast('error', err instanceof Error ? err.message : 'Error en la operacion')
    } finally {
      setActing(false)
    }
  }

  if (loading) return <div className="p-6 text-text-light">Cargando...</div>
  if (!lm) return <div className="p-6 text-text-light">Lista no encontrada.</div>

  const esBorrador = lm.estadoListaMatanzaId === EstadoListaMatanza.Borrador
  const esConfirmada = lm.estadoListaMatanzaId === EstadoListaMatanza.Confirmada
  const enEjecucion = lm.estadoListaMatanzaId === EstadoListaMatanza.EnEjecucion
  const sinFaena = lm.renglones.every((r) => r.cantidadFaenada === 0)
  const edicionControlada = esConfirmada || enEjecucion

  // Reglas de edicion por renglon (espejo del backend: R-11, R-12, R-14)
  const congelado = (r: ListaMatanzaRenglon) => r.cantidadFaenada >= r.cantidad
  const puedeEditarCantidad = (r: ListaMatanzaRenglon) =>
    esConfirmada ? !congelado(r) : enEjecucion ? r.cantidadFaenada === 0 : false
  const puedeEditarSecuencia = (r: ListaMatanzaRenglon) => edicionControlada && r.cantidadFaenada === 0
  const puedeQuitar = (r: ListaMatanzaRenglon) =>
    esConfirmada && r.cantidadFaenada === 0 && lm.renglones.length > 1

  // Sobrante del cierre: planificado no faenado que se libera (R-17)
  const totalSobrante = lm.renglones.reduce((acc, r) => acc + Math.max(0, r.cantidad - r.cantidadFaenada), 0)
  const renglonesConSobrante = lm.renglones.filter((r) => r.cantidad > r.cantidadFaenada).length

  const cerrarLista = async () => {
    setShowCierre(false)
    setActing(true)
    try {
      const res = await finalizarListaMatanza(lm.id, motivoCierre.trim() || undefined)
      toast(
        'success',
        res.totalLiberado > 0
          ? `Lista cerrada: se liberaron ${res.totalLiberado} animales no faenados`
          : 'Lista cerrada: todo lo planificado fue faenado',
      )
      setMotivoCierre('')
      await fetchData()
    } catch (err) {
      toast('error', err instanceof Error ? err.message : 'Error al cerrar la lista')
    } finally {
      setActing(false)
    }
  }

  const getEdit = (r: ListaMatanzaRenglon): RowEdit =>
    rowEdits[r.id] ?? { cantidad: r.cantidad, secuencia: r.secuencia }

  const setEdit = (renglonId: string, patch: Partial<RowEdit>, base: RowEdit) => {
    setRowEdits((prev) => ({ ...prev, [renglonId]: { ...base, ...prev[renglonId], ...patch } }))
  }

  const hayCambio = (r: ListaMatanzaRenglon): boolean => {
    const e = rowEdits[r.id]
    return !!e && (e.cantidad !== r.cantidad || e.secuencia !== r.secuencia)
  }

  const guardarRenglon = async (r: ListaMatanzaRenglon) => {
    const e = getEdit(r)
    if (e.cantidad <= 0) {
      toast('error', 'La cantidad debe ser mayor a cero.')
      return
    }
    await runAction(
      () => editarRenglonListaMatanza(lm.id, r.id, { Cantidad: e.cantidad, Secuencia: e.secuencia }),
      'Renglon actualizado (auditado)',
    )
  }

  const agregarTropa = async (d: DisponibilidadFaenaItem) => {
    if (d.disponible <= 0) return
    const req = { TropaId: d.tropaId, AlmacenId: d.almacenId, TipoEspecieId: d.tipoEspecieId, Cantidad: d.disponible }
    if (enEjecucion) {
      await runAction(() => faenaEmergenciaListaMatanza(lm.id, req), 'Faena de emergencia agregada')
    } else {
      await runAction(() => agregarRenglonListaMatanza(lm.id, req), 'Tropa agregada (auditado)')
    }
  }

  return (
    <>
      <PageHeader title={`Lista de Matanza N° ${lm.numeroLista}`}>
        <div className="flex items-center gap-2">
          {esBorrador && (
            <Button variant="secondary" onClick={() => navigate(`/operaciones/planificacion-faena/${lm.id}/edit`)}>
              Editar
            </Button>
          )}
          {esBorrador && (
            <Button onClick={() => void runAction(() => confirmarListaMatanza(lm.id), 'Lista confirmada')} loading={acting}>
              Confirmar
            </Button>
          )}
          {esConfirmada && sinFaena && (
            <Button
              variant="secondary"
              onClick={() => void runAction(() => desconfirmarListaMatanza(lm.id), 'Lista vuelta a Borrador')}
              loading={acting}
            >
              Volver a Borrador
            </Button>
          )}
          {esConfirmada && (
            <Button
              variant="success"
              onClick={() => void runAction(() => iniciarListaMatanza(lm.id), 'Faena iniciada')}
              loading={acting}
              disabled={!!iniciarBloqueado}
              title={iniciarBloqueado ?? undefined}
            >
              Iniciar Faena
            </Button>
          )}
          {enEjecucion && (
            <Button variant="success" onClick={() => setShowCierre(true)} loading={acting}>
              Cerrar Lista
            </Button>
          )}
          {(esBorrador || esConfirmada) && (
            <Button variant="danger" onClick={() => setShowCancel(true)}>Anular</Button>
          )}
        </div>
      </PageHeader>

      {esConfirmada && iniciarBloqueado && (
        <div className="mb-4 rounded-lg border border-amber-200 bg-amber-50 p-3 text-sm text-amber-800">
          {iniciarBloqueado}
        </div>
      )}

      {/* Cabecera */}
      <div className="mb-4 rounded-lg border border-border bg-surface p-6 shadow-sm">
        <div className="grid grid-cols-2 gap-4 text-sm sm:grid-cols-4">
          <div>
            <p className="text-text-light">Estado</p>
            <Badge variant={estadoVariant[lm.estadoListaMatanzaId] ?? 'neutral'}>{lm.estadoListaMatanzaNombre}</Badge>
          </div>
          <div>
            <p className="text-text-light">Fecha de faena</p>
            <p className="font-medium">{formatFecha(lm.fecha)}</p>
          </div>
          <div>
            <p className="text-text-light">Especie</p>
            <p className="font-medium">{lm.especieNombre}</p>
          </div>
          <div>
            <p className="text-text-light">Version</p>
            <p className="font-mono font-medium">{lm.version}</p>
          </div>
          <div>
            <p className="text-text-light">Establecimiento</p>
            <p className="font-medium">{lm.establecimientoNombre}</p>
          </div>
        </div>
      </div>

      {/* Renglones */}
      <div className="mb-4 rounded-lg border border-border bg-surface p-6 shadow-sm">
        <div className="mb-3 flex items-center justify-between">
          <h3 className="text-sm font-semibold text-text">Renglones (secuencia de faena)</h3>
          {edicionControlada && (
            <span className="text-xs text-text-light">Edicion controlada: cada cambio queda registrado en el historial</span>
          )}
        </div>
        <div className="overflow-x-auto">
          <table className="w-full text-sm">
            <thead>
              <tr className="border-b border-border text-left text-text-light">
                <th className="py-2 pr-3 w-28">Secuencia</th>
                <th className="py-2 pr-3">Tropa</th>
                <th className="py-2 pr-3">Corral</th>
                <th className="py-2 pr-3">Tipo Especie</th>
                <th className="py-2 pr-3 w-32">Cantidad</th>
                <th className="py-2 pr-3 text-right">Faenada</th>
                {edicionControlada && <th className="py-2" />}
              </tr>
            </thead>
            <tbody>
              {lm.renglones.map((r) => {
                const e = getEdit(r)
                return (
                  <tr key={r.id} className="border-b border-border/60">
                    <td className="py-1.5 pr-3">
                      {puedeEditarSecuencia(r) ? (
                        <input
                          type="number"
                          className="w-20 rounded-lg border border-border px-2 py-1 text-sm"
                          value={e.secuencia}
                          min={1}
                          onChange={(ev) => setEdit(r.id, { secuencia: Number(ev.target.value) }, getEdit(r))}
                        />
                      ) : (
                        <span className="font-mono">{r.secuencia}</span>
                      )}
                    </td>
                    <td className="py-1.5 pr-3 font-mono">{r.numeroTropa}</td>
                    <td className="py-1.5 pr-3">{r.almacenNombre}</td>
                    <td className="py-1.5 pr-3">{r.tipoEspecieNombre}</td>
                    <td className="py-1.5 pr-3">
                      {puedeEditarCantidad(r) ? (
                        <input
                          type="number"
                          className="w-24 rounded-lg border border-border px-2 py-1 text-sm"
                          value={e.cantidad}
                          min={Math.max(1, r.cantidadFaenada)}
                          onChange={(ev) => setEdit(r.id, { cantidad: Number(ev.target.value) }, getEdit(r))}
                        />
                      ) : (
                        <span className="font-mono">{r.cantidad}</span>
                      )}
                    </td>
                    <td className="py-1.5 pr-3 text-right font-mono">{r.cantidadFaenada}</td>
                    {edicionControlada && (
                      <td className="py-1.5 text-right">
                        <div className="flex items-center justify-end gap-1">
                          {hayCambio(r) && (
                            <Button size="sm" onClick={() => void guardarRenglon(r)} loading={acting}>
                              Guardar
                            </Button>
                          )}
                          {puedeQuitar(r) && (
                            <button
                              onClick={() => setQuitarTarget(r)}
                              className="rounded px-2 py-1 text-xs text-danger hover:bg-red-50"
                              title="Quitar renglon (auditado)"
                            >
                              Quitar
                            </button>
                          )}
                        </div>
                      </td>
                    )}
                  </tr>
                )
              })}
            </tbody>
          </table>
        </div>
      </div>

      {/* Agregar tropa / Faena de emergencia */}
      {edicionControlada && disponibilidad.length > 0 && (
        <div className="mb-4 rounded-lg border border-border bg-surface p-6 shadow-sm">
          <h3 className="mb-3 text-sm font-semibold text-text">
            {enEjecucion ? 'Faena de emergencia (se anexa al final de la secuencia)' : 'Agregar tropa (auditado)'}
          </h3>
          <div className="overflow-x-auto">
            <table className="w-full text-sm">
              <thead>
                <tr className="border-b border-border text-left text-text-light">
                  <th className="py-2 pr-3">Tropa</th>
                  <th className="py-2 pr-3">Corral</th>
                  <th className="py-2 pr-3">Cliente</th>
                  <th className="py-2 pr-3">Tipo Especie</th>
                  <th className="py-2 pr-3 text-right">Disponible</th>
                  <th className="py-2" />
                </tr>
              </thead>
              <tbody>
                {disponibilidad.map((d) => (
                  <tr key={`${d.tropaId}-${d.almacenId}-${d.tipoEspecieId}`} className="border-b border-border/60">
                    <td className="py-2 pr-3 font-mono">{d.numeroTropa}</td>
                    <td className="py-2 pr-3">{d.almacenNombre}</td>
                    <td className="py-2 pr-3">{d.clienteNombre}</td>
                    <td className="py-2 pr-3">{d.tipoEspecieNombre}</td>
                    <td className="py-2 pr-3 text-right font-mono">{d.disponible}</td>
                    <td className="py-2 text-right">
                      <Button variant="secondary" size="sm" onClick={() => void agregarTropa(d)} disabled={d.disponible <= 0 || acting}>
                        {enEjecucion ? 'Emergencia' : 'Agregar'}
                      </Button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
          <p className="mt-2 text-xs text-text-light">
            Se agrega con la cantidad disponible; luego pod&eacute;s ajustarla en el rengl&oacute;n (queda auditado).
          </p>
        </div>
      )}

      {/* Historial */}
      <div className="rounded-lg border border-border bg-surface p-6 shadow-sm">
        <h3 className="mb-3 text-sm font-semibold text-text">Historial de la programacion</h3>
        {lm.movimientos.length === 0 ? (
          <p className="text-sm text-text-light">Sin movimientos registrados (el borrador no genera historial).</p>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full text-sm">
              <thead>
                <tr className="border-b border-border text-left text-text-light">
                  <th className="py-2 pr-3 w-16">Ver.</th>
                  <th className="py-2 pr-3">Fecha</th>
                  <th className="py-2 pr-3">Movimiento</th>
                  <th className="py-2 pr-3 text-right">Cant. ant.</th>
                  <th className="py-2 pr-3 text-right">Cant. nueva</th>
                  <th className="py-2 pr-3">Detalle</th>
                </tr>
              </thead>
              <tbody>
                {lm.movimientos.map((m) => (
                  <tr key={m.id} className="border-b border-border/60">
                    <td className="py-2 pr-3 font-mono">{m.version}</td>
                    <td className="py-2 pr-3">{formatFechaHora(m.fecha)}</td>
                    <td className="py-2 pr-3">{movimientoLabel[m.tipoMovimiento] ?? m.tipoMovimiento}</td>
                    <td className="py-2 pr-3 text-right font-mono">{m.cantidadAnterior ?? ''}</td>
                    <td className="py-2 pr-3 text-right font-mono">{m.cantidadNueva ?? ''}</td>
                    <td className="py-2 pr-3 text-text-light">{m.motivo}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>

      <Modal isOpen={showCierre} onClose={() => setShowCierre(false)} title="Cerrar lista de matanza" size="sm">
        {totalSobrante > 0 ? (
          <div className="mb-4 rounded-lg border border-amber-200 bg-amber-50 p-3 text-sm text-amber-800">
            Se liberar&aacute;n <span className="font-semibold">{totalSobrante}</span> animales planificados no
            faenados ({renglonesConSobrante} {renglonesConSobrante === 1 ? 'renglon' : 'renglones'}), que volver&aacute;n
            a estar disponibles para futuras planificaciones. El detalle queda registrado en el historial.
          </div>
        ) : (
          <p className="mb-4 text-sm text-text-light">
            Todo lo planificado fue faenado. La lista se cierra sin sobrante.
          </p>
        )}
        {totalSobrante > 0 && (
          <div className="mb-4">
            <label htmlFor="motivo-cierre" className="mb-1 block text-sm font-medium text-text">
              Motivo (opcional)
            </label>
            <textarea
              id="motivo-cierre"
              rows={2}
              className="w-full rounded-lg border border-border px-3 py-2 text-sm text-text placeholder:text-muted focus:border-primary-500 focus:outline-none focus:ring-1 focus:ring-primary-500"
              placeholder="Por que no se faeno lo planificado..."
              value={motivoCierre}
              onChange={(e) => setMotivoCierre(e.target.value)}
            />
          </div>
        )}
        <div className="flex justify-end gap-3">
          <Button variant="secondary" onClick={() => setShowCierre(false)} disabled={acting}>
            Cancelar
          </Button>
          <Button variant="success" onClick={() => void cerrarLista()} loading={acting}>
            Cerrar Lista
          </Button>
        </div>
      </Modal>

      <ConfirmDialog
        isOpen={!!quitarTarget}
        onConfirm={async () => {
          const target = quitarTarget
          setQuitarTarget(null)
          if (target) {
            await runAction(() => quitarRenglonListaMatanza(lm.id, target.id), 'Renglon quitado (auditado)')
          }
        }}
        onCancel={() => setQuitarTarget(null)}
        title="Quitar renglon"
        message={`¿Quitar la tropa N° ${quitarTarget?.numeroTropa} (${quitarTarget?.almacenNombre}) de la lista? El cambio queda auditado.`}
        isLoading={acting}
      />

      <ConfirmDialog
        isOpen={showCancel}
        onConfirm={async () => {
          setShowCancel(false)
          await runAction(() => cancelarListaMatanza(lm.id), 'Lista anulada')
        }}
        onCancel={() => setShowCancel(false)}
        title="Anular lista de matanza"
        message={`¿Confirma anular la lista N° ${lm.numeroLista}? Esta accion no se puede deshacer.`}
        isLoading={acting}
      />
    </>
  )
}
