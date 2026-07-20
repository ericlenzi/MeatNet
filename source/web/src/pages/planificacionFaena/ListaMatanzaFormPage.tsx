import { useState, useEffect, useCallback } from 'react'
import { useNavigate, useParams } from 'react-router'
import {
  getListaMatanza,
  getListasMatanzas,
  getDisponibilidadFaena,
  createListaMatanza,
  updateListaMatanza,
  confirmarListaMatanza,
} from '@/services/listasMatanzas.service'
import { getEspecies } from '@/services/especies.service'
import { getAlmacenes, FamiliaAlmacen } from '@/services/almacenes.service'
import { getPuestos } from '@/services/puestos.service'
import { useApp } from '@/contexts/AppContext'
import { useToast } from '@/components/ui/Toast'
import { EstadoListaMatanza } from '@/types'
import type { DisponibilidadFaenaItem, RenglonInput, ListaMatanzaListItem } from '@/types'
import PageHeader from '@/components/ui/PageHeader'
import Button from '@/components/ui/Button'
import Input from '@/components/ui/Input'
import Select from '@/components/ui/Select'

interface RenglonRow {
  key: string
  tropaId: string
  numeroTropa: number
  almacenId: string
  almacenNombre: string
  almacenDestinoId: string
  tipoEspecieId: string
  tipoEspecieNombre: string
  secuencia: number
  cantidad: number
}

function today(): string {
  return new Date().toISOString().slice(0, 10)
}

function newKey(): string {
  return typeof crypto !== 'undefined' && crypto.randomUUID ? crypto.randomUUID() : Math.random().toString(36).slice(2)
}

export default function ListaMatanzaFormPage() {
  const { id } = useParams()
  const navigate = useNavigate()
  const { toast } = useToast()
  const { currentEstablecimiento } = useApp()
  const isEdit = !!id

  const [especieId, setEspecieId] = useState('')
  const [puestoId, setPuestoId] = useState('')
  const [puestoOptions, setPuestoOptions] = useState<{ value: string; label: string }[]>([])
  const [fecha, setFecha] = useState(today())
  const [renglones, setRenglones] = useState<RenglonRow[]>([])
  const [disponibilidad, setDisponibilidad] = useState<DisponibilidadFaenaItem[]>([])
  const [especieOptions, setEspecieOptions] = useState<{ value: string; label: string }[]>([])
  const [camaraOptions, setCamaraOptions] = useState<{ value: string; label: string }[]>([])
  // R-A4: destino por defecto = ultimo destino seleccionado en esta sesion de edicion
  const [ultimoDestino, setUltimoDestino] = useState('')

  const [estado, setEstado] = useState<string>(EstadoListaMatanza.Borrador)
  const [duplicada, setDuplicada] = useState<ListaMatanzaListItem | null>(null)
  const [fetching, setFetching] = useState(true)
  const [saving, setSaving] = useState(false)
  const [confirming, setConfirming] = useState(false)

  const editable = !isEdit || estado === EstadoListaMatanza.Borrador

  // Carga inicial: especies + (si edita) la lista
  useEffect(() => {
    const load = async () => {
      setFetching(true)
      try {
        const esp = await getEspecies({ Estado: true, PageSize: 1000 })
        setEspecieOptions((esp.data || []).map((e) => ({ value: e.codigo, label: e.nombre })))

        // Camaras de faena del establecimiento (destino de los renglones)
        if (currentEstablecimiento?.id) {
          const camaras = await getAlmacenes({
            EstablecimientoId: currentEstablecimiento.id,
            Estado: true,
            Familia: FamiliaAlmacen.Camara,
          })
          setCamaraOptions(camaras.map((c) => ({ value: c.id, label: c.nombre })))

          // Puestos (palcos de faena) del establecimiento
          const puestos = await getPuestos(currentEstablecimiento.id)
          setPuestoOptions(puestos.map((p) => ({ value: p.id, label: p.nombre || p.codigoPuesto })))
        }

        if (isEdit && id) {
          const lm = await getListaMatanza(id)
          if (lm.estadoListaMatanzaId !== EstadoListaMatanza.Borrador) {
            // No editable: ir al detalle
            navigate(`/operaciones/planificacion-faena/${id}`, { replace: true })
            return
          }
          setEstado(lm.estadoListaMatanzaId)
          setEspecieId(lm.especieId)
          setPuestoId(lm.puestoId ?? '')
          setFecha(lm.fecha.slice(0, 10))
          setRenglones(
            lm.renglones.map((r) => ({
              key: newKey(),
              tropaId: r.tropaId,
              numeroTropa: r.numeroTropa,
              almacenId: r.almacenId,
              almacenNombre: r.almacenNombre,
              almacenDestinoId: r.almacenDestinoId ?? '',
              tipoEspecieId: r.tipoEspecieId,
              tipoEspecieNombre: r.tipoEspecieNombre,
              secuencia: r.secuencia,
              cantidad: r.cantidad,
            })),
          )
        }
      } catch {
        toast('error', 'Error al cargar la lista de matanza')
      } finally {
        setFetching(false)
      }
    }
    void load()
  }, [id, isEdit, navigate, toast, currentEstablecimiento?.id])

  // Cargar disponibilidad al cambiar la especie
  const loadDisponibilidad = useCallback(async () => {
    if (!currentEstablecimiento?.id || !especieId) {
      setDisponibilidad([])
      return
    }
    try {
      const items = await getDisponibilidadFaena({
        EstablecimientoId: currentEstablecimiento.id,
        EspecieId: especieId,
        ExcludeListaId: id,
      })
      setDisponibilidad(items)
    } catch {
      setDisponibilidad([])
    }
  }, [currentEstablecimiento?.id, especieId, id])

  useEffect(() => {
    void loadDisponibilidad()
  }, [loadDisponibilidad])

  // R-01: chequeo temprano de unicidad (Establecimiento, Fecha, Especie).
  // Solo cuenta una LM no anulada; si existe, se bloquea antes de cargar renglones.
  useEffect(() => {
    if (isEdit || !currentEstablecimiento?.id || !especieId || !fecha) {
      setDuplicada(null)
      return
    }
    let cancelled = false
    const check = async () => {
      try {
        const res = await getListasMatanzas({
          EstablecimientoId: currentEstablecimiento.id,
          EspecieId: especieId,
          Fecha: fecha,
          PageSize: 50,
        })
        if (cancelled) return
        const activa = (res.data || []).find((x) => x.estadoListaMatanzaId !== EstadoListaMatanza.Anulada)
        setDuplicada(activa ?? null)
      } catch {
        if (!cancelled) setDuplicada(null)
      }
    }
    void check()
    return () => {
      cancelled = true
    }
  }, [isEdit, currentEstablecimiento?.id, especieId, fecha])

  // Suma ya planificada por (tropa, corral, categoria)
  const usadoPorClave = (tropaId: string, almacenId: string, tipoEspecieId: string, exceptKey?: string): number =>
    renglones
      .filter((r) => r.tropaId === tropaId && r.almacenId === almacenId && r.tipoEspecieId === tipoEspecieId && r.key !== exceptKey)
      .reduce((acc, r) => acc + (r.cantidad || 0), 0)

  const disponibleDe = (tropaId: string, almacenId: string, tipoEspecieId: string): number => {
    const d = disponibilidad.find((x) => x.tropaId === tropaId && x.almacenId === almacenId && x.tipoEspecieId === tipoEspecieId)
    return d ? d.disponible : Number.POSITIVE_INFINITY
  }

  const agregarRenglon = (item: DisponibilidadFaenaItem) => {
    const yaUsado = usadoPorClave(item.tropaId, item.almacenId, item.tipoEspecieId)
    const restante = item.disponible - yaUsado
    if (restante <= 0) {
      toast('info', 'No queda disponible de esa tropa/corral/categoria')
      return
    }
    setRenglones((prev) => [
      ...prev,
      {
        key: newKey(),
        tropaId: item.tropaId,
        numeroTropa: item.numeroTropa,
        almacenId: item.almacenId,
        almacenNombre: item.almacenNombre,
        almacenDestinoId: ultimoDestino,   // R-A4: prellenar con el ultimo destino elegido
        tipoEspecieId: item.tipoEspecieId,
        tipoEspecieNombre: item.tipoEspecieNombre,
        secuencia: prev.length === 0 ? 10 : Math.max(...prev.map((r) => r.secuencia)) + 10,
        cantidad: restante,
      },
    ])
  }

  const updateRenglon = (key: string, patch: Partial<RenglonRow>) => {
    setRenglones((prev) => prev.map((r) => (r.key === key ? { ...r, ...patch } : r)))
  }

  const cambiarDestino = (key: string, destinoId: string) => {
    updateRenglon(key, { almacenDestinoId: destinoId })
    if (destinoId) setUltimoDestino(destinoId)   // R-A4
  }

  const quitarRenglon = (key: string) => {
    setRenglones((prev) => prev.filter((r) => r.key !== key))
  }

  const dividirRenglon = (key: string) => {
    setRenglones((prev) => {
      const row = prev.find((r) => r.key === key)
      if (!row || row.cantidad < 2) return prev
      const mitad = Math.floor(row.cantidad / 2)
      const maxSeq = Math.max(...prev.map((r) => r.secuencia))
      return prev.map((r) => (r.key === key ? { ...r, cantidad: row.cantidad - mitad } : r)).concat({
        ...row,
        key: newKey(),
        secuencia: maxSeq + 10,
        cantidad: mitad,
      })
    })
  }

  const fusionarDuplicados = () => {
    setRenglones((prev) => {
      const map = new Map<string, RenglonRow>()
      for (const r of prev) {
        const k = `${r.tropaId}|${r.almacenId}|${r.tipoEspecieId}`
        const existing = map.get(k)
        if (existing) {
          existing.cantidad += r.cantidad
          existing.secuencia = Math.min(existing.secuencia, r.secuencia)
        } else {
          map.set(k, { ...r })
        }
      }
      return Array.from(map.values()).sort((a, b) => a.secuencia - b.secuencia)
    })
  }

  const validar = (): string | null => {
    if (!especieId) return 'Debe seleccionar la especie.'
    if (!fecha) return 'Debe indicar la fecha.'
    if (duplicada)
      return `Ya existe la lista N° ${duplicada.numeroLista} (${duplicada.estadoListaMatanzaNombre}) para esa fecha y especie.`
    if (renglones.length === 0) return 'Debe agregar al menos un renglon.'
    if (renglones.some((r) => r.cantidad <= 0)) return 'Todas las cantidades deben ser mayores a cero.'
    const grupos = new Map<string, { tropaId: string; almacenId: string; tipoEspecieId: string; numeroTropa: number; almacenNombre: string; tipoEspecieNombre: string }>()
    for (const r of renglones) {
      grupos.set(`${r.tropaId}|${r.almacenId}|${r.tipoEspecieId}`, {
        tropaId: r.tropaId, almacenId: r.almacenId, tipoEspecieId: r.tipoEspecieId,
        numeroTropa: r.numeroTropa, almacenNombre: r.almacenNombre, tipoEspecieNombre: r.tipoEspecieNombre,
      })
    }
    for (const g of grupos.values()) {
      const total = usadoPorClave(g.tropaId, g.almacenId, g.tipoEspecieId)
      const disp = disponibleDe(g.tropaId, g.almacenId, g.tipoEspecieId)
      if (total > disp) {
        return `La tropa N° ${g.numeroTropa} (${g.tipoEspecieNombre}) en ${g.almacenNombre} supera el disponible (${disp}).`
      }
    }
    return null
  }

  const buildRenglones = (): RenglonInput[] =>
    renglones.map((r) => ({
      TropaId: r.tropaId,
      AlmacenId: r.almacenId,
      AlmacenDestinoId: r.almacenDestinoId || null,
      TipoEspecieId: r.tipoEspecieId,
      Secuencia: r.secuencia,
      Cantidad: r.cantidad,
    }))

  const guardar = async (): Promise<string | null> => {
    const error = validar()
    if (error) {
      toast('error', error)
      return null
    }
    setSaving(true)
    try {
      if (isEdit && id) {
        await updateListaMatanza(id, { EspecieId: especieId, PuestoId: puestoId || null, Fecha: fecha, Renglones: buildRenglones() })
        toast('success', 'Lista guardada')
        return id
      } else {
        const res = await createListaMatanza({
          EstablecimientoId: currentEstablecimiento?.id ?? '',
          EspecieId: especieId,
          PuestoId: puestoId || null,
          Fecha: fecha,
          Renglones: buildRenglones(),
        })
        toast('success', `Lista N° ${res.numeroLista} creada`)
        navigate(`/operaciones/planificacion-faena/${res.id}/edit`, { replace: true })
        return res.id
      }
    } catch (err) {
      toast('error', err instanceof Error ? err.message : 'Error al guardar')
      return null
    } finally {
      setSaving(false)
    }
  }

  const confirmar = async () => {
    // R-A3: al confirmar, todos los renglones deben tener destino (camara)
    if (renglones.some((r) => !r.almacenDestinoId)) {
      toast('error', 'Todos los renglones deben tener un destino de faena (camara) para confirmar.')
      return
    }
    const savedId = await guardar()
    if (!savedId) return
    setConfirming(true)
    try {
      await confirmarListaMatanza(savedId)
      toast('success', 'Lista confirmada')
      navigate(`/operaciones/planificacion-faena/${savedId}`)
    } catch (err) {
      toast('error', err instanceof Error ? err.message : 'Error al confirmar')
    } finally {
      setConfirming(false)
    }
  }

  if (fetching) {
    return <div className="p-6 text-text-light">Cargando...</div>
  }

  const disponiblesConRestante = disponibilidad
    .map((d) => ({ ...d, restante: d.disponible - usadoPorClave(d.tropaId, d.almacenId, d.tipoEspecieId) }))

  return (
    <>
      <PageHeader title={isEdit ? 'Editar Lista de Matanza' : 'Nueva Lista de Matanza'} />

      <div className="space-y-4">
        {/* Cabecera */}
        <div className="rounded-lg border border-border bg-surface p-6 shadow-sm">
          <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-4">
            <Input label="Establecimiento" value={currentEstablecimiento?.nombre ?? ''} disabled />
            <Select
              label="Especie"
              options={especieOptions}
              placeholder="Seleccione especie"
              value={especieId}
              onChange={(e) => { setEspecieId(e.target.value); setRenglones([]) }}
              disabled={isEdit}
            />
            <Select
              label="Puesto de faena"
              options={puestoOptions}
              placeholder="(Sin asignar)"
              value={puestoId}
              onChange={(e) => setPuestoId(e.target.value)}
              disabled={!editable}
            />
            <Input label="Fecha de faena" type="date" value={fecha} onChange={(e) => setFecha(e.target.value)} disabled={isEdit} />
          </div>
        </div>

        {/* R-01: ya existe una LM activa para (Establecimiento, Fecha, Especie) */}
        {duplicada && (
          <div className="rounded-lg border border-amber-200 bg-amber-50 p-4 text-sm text-amber-800">
            Ya existe la lista de matanza <span className="font-semibold">N° {duplicada.numeroLista}</span> en
            estado <span className="font-semibold">{duplicada.estadoListaMatanzaNombre}</span> para esa fecha y
            especie. Solo puede haber una lista activa por establecimiento, fecha y especie (se permite otra
            únicamente si la existente está Anulada).{' '}
            <button
              type="button"
              className="font-medium underline hover:text-amber-900"
              onClick={() => navigate(`/operaciones/planificacion-faena/${duplicada.id}`)}
            >
              Ver la lista existente
            </button>
          </div>
        )}

        {/* Tropas disponibles */}
        {editable && especieId && !duplicada && (
          <div className="rounded-lg border border-border bg-surface p-6 shadow-sm">
            <h3 className="mb-3 text-sm font-semibold text-text">Tropas disponibles (En Pie)</h3>
            {disponiblesConRestante.length === 0 ? (
              <p className="text-sm text-text-light">No hay stock disponible para esta especie en el establecimiento.</p>
            ) : (
              <div className="overflow-x-auto">
                <table className="w-full text-sm">
                  <thead>
                    <tr className="border-b border-border text-left text-text-light">
                      <th className="py-2 pr-3">Tropa</th>
                      <th className="py-2 pr-3">Corral</th>
                      <th className="py-2 pr-3">Cliente</th>
                      <th className="py-2 pr-3">Tipo Especie</th>
                      <th className="py-2 pr-3 text-right">En Pie</th>
                      <th className="py-2 pr-3 text-right">Reservado</th>
                      <th className="py-2 pr-3 text-right">Restante</th>
                      <th className="py-2 pr-3 text-right">Peso prom.</th>
                      <th className="py-2" />
                    </tr>
                  </thead>
                  <tbody>
                    {disponiblesConRestante.map((d) => (
                      <tr key={`${d.tropaId}-${d.almacenId}-${d.tipoEspecieId}`} className="border-b border-border/60">
                        <td className="py-2 pr-3 font-mono">{d.numeroTropa}</td>
                        <td className="py-2 pr-3">{d.almacenNombre}</td>
                        <td className="py-2 pr-3">{d.clienteNombre}</td>
                        <td className="py-2 pr-3">{d.tipoEspecieNombre}</td>
                        <td className="py-2 pr-3 text-right font-mono">{d.enPie}</td>
                        <td className="py-2 pr-3 text-right font-mono">{d.reservado}</td>
                        <td className="py-2 pr-3 text-right font-mono">{d.restante}</td>
                        <td className="py-2 pr-3 text-right font-mono">{d.pesoPromedio.toFixed(1)}</td>
                        <td className="py-2 text-right">
                          <Button variant="secondary" onClick={() => agregarRenglon(d)} disabled={d.restante <= 0}>
                            Agregar
                          </Button>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            )}
          </div>
        )}

        {/* Renglones de la lista */}
        <div className="rounded-lg border border-border bg-surface p-6 shadow-sm">
          <div className="mb-3 flex items-center justify-between">
            <h3 className="text-sm font-semibold text-text">Renglones de la lista (secuencia de faena)</h3>
            {editable && renglones.length > 1 && (
              <Button variant="secondary" onClick={fusionarDuplicados}>Fusionar duplicados</Button>
            )}
          </div>
          {renglones.length === 0 ? (
            <p className="text-sm text-text-light">Agregue tropas desde el listado de disponibles.</p>
          ) : (
            <div className="overflow-x-auto">
              <table className="w-full text-sm">
                <thead>
                  <tr className="border-b border-border text-left text-text-light">
                    <th className="py-2 pr-3 w-24">Secuencia</th>
                    <th className="py-2 pr-3">Tropa</th>
                    <th className="py-2 pr-3">Corral</th>
                    <th className="py-2 pr-3">Tipo Especie</th>
                    <th className="py-2 pr-3">Destino (camara)</th>
                    <th className="py-2 pr-3 w-28">Cantidad</th>
                    <th className="py-2" />
                  </tr>
                </thead>
                <tbody>
                  {[...renglones].sort((a, b) => a.secuencia - b.secuencia).map((r) => (
                    <tr key={r.key} className="border-b border-border/60">
                      <td className="py-1.5 pr-3">
                        <input
                          type="number"
                          className="w-20 rounded-lg border border-border px-2 py-1 text-sm disabled:bg-gray-50"
                          value={r.secuencia}
                          min={1}
                          disabled={!editable}
                          onChange={(e) => updateRenglon(r.key, { secuencia: Number(e.target.value) })}
                        />
                      </td>
                      <td className="py-1.5 pr-3 font-mono">{r.numeroTropa}</td>
                      <td className="py-1.5 pr-3">{r.almacenNombre}</td>
                      <td className="py-1.5 pr-3">{r.tipoEspecieNombre}</td>
                      <td className="py-1.5 pr-3">
                        <select
                          className="w-44 rounded-lg border border-border px-2 py-1 text-sm disabled:bg-gray-50"
                          value={r.almacenDestinoId}
                          disabled={!editable}
                          onChange={(e) => cambiarDestino(r.key, e.target.value)}
                        >
                          <option value="">Seleccionar...</option>
                          {camaraOptions.map((c) => (
                            <option key={c.value} value={c.value}>{c.label}</option>
                          ))}
                        </select>
                      </td>
                      <td className="py-1.5 pr-3">
                        <input
                          type="number"
                          className="w-24 rounded-lg border border-border px-2 py-1 text-sm disabled:bg-gray-50"
                          value={r.cantidad}
                          min={1}
                          disabled={!editable}
                          onChange={(e) => updateRenglon(r.key, { cantidad: Number(e.target.value) })}
                        />
                      </td>
                      <td className="py-1.5 text-right">
                        {editable && (
                          <div className="flex items-center justify-end gap-1">
                            <button
                              onClick={() => dividirRenglon(r.key)}
                              className="rounded px-2 py-1 text-xs text-primary-600 hover:bg-primary-50 disabled:text-muted"
                              title="Dividir renglon"
                              disabled={r.cantidad < 2}
                            >
                              Dividir
                            </button>
                            <button
                              onClick={() => quitarRenglon(r.key)}
                              className="rounded px-2 py-1 text-xs text-danger hover:bg-red-50"
                              title="Quitar renglon"
                            >
                              Quitar
                            </button>
                          </div>
                        )}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>

        {/* Acciones */}
        <div className="flex justify-end gap-3">
          <Button variant="secondary" onClick={() => navigate('/operaciones/planificacion-faena')}>Volver</Button>
          <Button onClick={() => void guardar()} loading={saving} disabled={!!duplicada}>Guardar Borrador</Button>
          <Button variant="primary" onClick={() => void confirmar()} loading={confirming} disabled={renglones.length === 0 || !!duplicada}>
            Confirmar
          </Button>
        </div>
      </div>
    </>
  )
}
