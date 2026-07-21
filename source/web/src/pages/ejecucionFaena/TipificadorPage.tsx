import { useState, useEffect, useCallback, useMemo, useRef } from 'react'
import { useNavigate, useParams } from 'react-router'
import {
  getRenglonesEjecucion,
  sugerirTipificacion,
  getRomaneosJornada,
  crearRomaneo,
  anularRomaneo,
} from '@/services/romaneos.service'
import { getUnidadesFaenasOptions } from '@/services/unidadesFaenas.service'
import { getDestinosComerciales } from '@/services/tipificaciones.service'
import { useToast } from '@/components/ui/Toast'
import { EstadoListaMatanza } from '@/types'
import type {
  RenglonesEjecucion,
  RenglonEjecucionItem,
  TipificacionCandidata,
  RomaneoJornadaItem,
  CatalogoFaenaOption,
} from '@/types'
import type { UnidadFaena } from '@/types'
import PageHeader from '@/components/ui/PageHeader'
import Button from '@/components/ui/Button'
import Badge from '@/components/ui/Badge'
import ConfirmDialog from '@/components/ui/ConfirmDialog'

const LETRAS = ['A', 'B', 'C', 'D']

interface StickyTip {
  codigo: string
  pesoDesde: number
  pesoHasta: number
}

interface PiezaState {
  peso: string
  tipificacionId: string
  almacenDestinoId: string
}

const nuevaPieza = (almacenDestinoId = ''): PiezaState => ({
  peso: '',
  tipificacionId: '',
  almacenDestinoId,
})

function piezasEsperadas(uf: UnidadFaena | undefined): number {
  if (!uf) return 1
  return Math.max(1, uf.piezasPorAnimal)
}

export default function TipificadorPage() {
  const { listaMatanzaId } = useParams()
  const navigate = useNavigate()
  const { toast } = useToast()

  const [data, setData] = useState<RenglonesEjecucion | null>(null)
  const [unidadesFaenas, setUnidadesFaenas] = useState<UnidadFaena[]>([])
  const [destinos, setDestinos] = useState<CatalogoFaenaOption[]>([])
  const [jornada, setJornada] = useState<RomaneoJornadaItem[]>([])

  const [renglonId, setRenglonId] = useState('')
  const [unidadFaenaId, setUnidadFaenaId] = useState('')
  const [destinoId, setDestinoId] = useState('')
  const [garron, setGarron] = useState<number>(1)
  const [piezas, setPiezas] = useState<PiezaState[]>([nuevaPieza()])

  const [candidatas, setCandidatas] = useState<TipificacionCandidata[]>([])
  const [sticky, setSticky] = useState<StickyTip | null>(null)

  const [loading, setLoading] = useState(true)
  const [saving, setSaving] = useState(false)
  const [anularTarget, setAnularTarget] = useState<RomaneoJornadaItem | null>(null)

  const renglonSel = useMemo(
    () => data?.renglones.find((r) => r.renglonId === renglonId) ?? null,
    [data, renglonId],
  )
  const ufSel = useMemo(
    () => unidadesFaenas.find((u) => u.codigo === unidadFaenaId),
    [unidadesFaenas, unidadFaenaId],
  )
  const nroPiezas = piezasEsperadas(ufSel)

  // Camara por defecto de cada pieza = la del animal programado (cámara del renglón/LM).
  const defaultCamara = renglonSel?.almacenDestinoId ?? ''
  const defaultCamaraRef = useRef(defaultCamara)
  defaultCamaraRef.current = defaultCamara

  // Carga inicial: renglones, unidades de faena de la especie, destinos, jornada
  const cargar = useCallback(async () => {
    if (!listaMatanzaId) return
    setLoading(true)
    try {
      const rengl = await getRenglonesEjecucion(listaMatanzaId)
      setData(rengl)
      setGarron(rengl.proximoGarron)
      setRenglonId((prev) => {
        const sigueValido = rengl.renglones.some((r) => r.renglonId === prev && r.pendiente > 0)
        return sigueValido ? prev : rengl.renglonSugeridoId ?? ''
      })

      const [ufs, dest, jorn] = await Promise.all([
        getUnidadesFaenasOptions(rengl.especieId),
        getDestinosComerciales(),
        getRomaneosJornada(listaMatanzaId),
      ])
      setUnidadesFaenas(ufs)
      setDestinos(dest)
      setJornada(jorn)

      // Default del destino comercial: el marcado Favorito (si no hay, "Todos").
      setDestinoId((prev) => {
        if (prev && dest.some((d) => d.codigo === prev)) return prev
        return dest.find((d) => d.favorito)?.codigo ?? ''
      })

      setUnidadFaenaId((prev) => {
        if (prev && ufs.some((u) => u.codigo === prev)) return prev
        // Default: la unidad marcada PorDefecto para la especie; si no hay, la primera.
        const preferido = ufs.find((u) => u.porDefecto)
        return (preferido ?? ufs[0])?.codigo ?? ''
      })
    } catch (err) {
      toast('error', err instanceof Error ? err.message : 'Error al cargar el tipificador')
    } finally {
      setLoading(false)
    }
  }, [listaMatanzaId, toast])

  useEffect(() => {
    void cargar()
  }, [cargar])

  // Camara destino de cada pieza: por defecto la del renglon (animal programado). Se re-aplica a
  // TODAS las piezas al cambiar de renglon; el operador puede overridear pieza por pieza (persiste
  // hasta el proximo cambio de renglon).
  useEffect(() => {
    setPiezas((prev) => prev.map((p) => ({ ...p, almacenDestinoId: defaultCamara })))
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [renglonId, data])

  // Ajustar la cantidad de piezas al cambiar la unidad de faena (las nuevas heredan la camara default)
  useEffect(() => {
    setPiezas((prev) =>
      Array.from({ length: nroPiezas }, (_, i) => prev[i] ?? nuevaPieza(defaultCamaraRef.current)),
    )
  }, [nroPiezas])

  // Refrescar candidatas al cambiar renglon (categoria) / UF / destino
  useEffect(() => {
    if (!renglonSel || !unidadFaenaId || !data) {
      setCandidatas([])
      return
    }
    let cancel = false
    void (async () => {
      try {
        const res = await sugerirTipificacion({
          EspecieId: data.especieId,
          TipoEspecieId: renglonSel.tipoEspecieId,
          UnidadFaenaId: unidadFaenaId,
          DestinoComercialId: destinoId || undefined,
        })
        if (!cancel) setCandidatas(res.candidatas)
      } catch {
        if (!cancel) setCandidatas([])
      }
    })()
    return () => {
      cancel = true
    }
  }, [renglonSel, unidadFaenaId, destinoId, data])

  // Sticky hibrido: dado un peso, propone el codigo de tipificacion
  const proponerTipificacion = useCallback(
    (peso: number): string => {
      if (sticky && peso >= sticky.pesoDesde && peso <= sticky.pesoHasta) return sticky.codigo
      const match = candidatas.find((c) => peso >= c.pesoDesde && peso <= c.pesoHasta)
      return match?.codigo ?? sticky?.codigo ?? ''
    },
    [sticky, candidatas],
  )

  const onPesoChange = (idx: number, value: string) => {
    setPiezas((prev) => {
      const current = prev[idx]
      if (!current) return prev
      const next = [...prev]
      const peso = Number(value)
      const tipificacionId = peso > 0 ? proponerTipificacion(peso) : current.tipificacionId
      next[idx] = { ...current, peso: value, tipificacionId }
      return next
    })
  }

  const onCamaraChange = (idx: number, almacenDestinoId: string) => {
    setPiezas((prev) => {
      const current = prev[idx]
      if (!current) return prev
      const next = [...prev]
      next[idx] = { ...current, almacenDestinoId }
      return next
    })
  }

  const onTipificacionChange = (idx: number, codigo: string) => {
    setPiezas((prev) => {
      const current = prev[idx]
      if (!current) return prev
      const next = [...prev]
      next[idx] = { ...current, tipificacionId: codigo }
      return next
    })
    const c = candidatas.find((x) => x.codigo === codigo)
    if (c) setSticky({ codigo: c.codigo, pesoDesde: c.pesoDesde, pesoHasta: c.pesoHasta })
  }

  const resetCaptura = (proximoGarron: number) => {
    setGarron(proximoGarron)
    setPiezas(Array.from({ length: nroPiezas }, () => nuevaPieza(defaultCamaraRef.current)))
  }

  const guardar = async () => {
    if (!data || !renglonSel) {
      toast('error', 'Seleccione un renglon.')
      return
    }
    if (garron <= 0) {
      toast('error', 'El numero de garron debe ser mayor a cero.')
      return
    }
    if (piezas.some((p) => Number(p.peso) <= 0)) {
      toast('error', 'Cada pieza requiere un peso mayor a cero.')
      return
    }
    if (piezas.some((p) => !p.tipificacionId)) {
      toast('error', 'Cada pieza requiere una tipificacion.')
      return
    }
    if (piezas.some((p) => !p.almacenDestinoId)) {
      toast('error', 'Cada pieza requiere una camara de destino.')
      return
    }
    setSaving(true)
    try {
      const res = await crearRomaneo({
        ListaMatanzaId: data.listaMatanzaId,
        ListaMatanzaDetalleId: renglonSel.renglonId,
        UnidadFaenaId: unidadFaenaId,
        NumeroGarron: garron,
        Piezas: piezas.map((p) => ({
          AlmacenDestinoId: p.almacenDestinoId,
          TipificacionId: p.tipificacionId,
          Peso: Number(p.peso),
        })),
      })
      toast('success', `Romaneo N° ${res.numeroRomaneo} (garron ${res.numeroGarron}) registrado`)
      await cargar()
      resetCaptura(res.numeroGarron + 1)
    } catch (err) {
      toast('error', err instanceof Error ? err.message : 'Error al registrar el romaneo')
    } finally {
      setSaving(false)
    }
  }

  if (loading) return <div className="p-6 text-text-light">Cargando...</div>
  if (!data) return <div className="p-6 text-text-light">Lista no encontrada.</div>

  const enEjecucion = data.estadoListaMatanzaId === EstadoListaMatanza.EnEjecucion

  return (
    <>
      <PageHeader title={`Tipificador — Lista N° ${data.numeroLista} (${data.especieNombre})`}>
        <div className="flex items-center gap-2">
          <Button
            variant="secondary"
            onClick={() => navigate(`/operaciones/ejecucion-faena/${data.listaMatanzaId}/monitor`)}
          >
            Monitor
          </Button>
          <Button
            variant="secondary"
            onClick={() => navigate(`/operaciones/planificacion-faena/${data.listaMatanzaId}`)}
          >
            Volver a la lista
          </Button>
        </div>
      </PageHeader>

      {!enEjecucion && (
        <div className="mb-4 rounded-lg border border-amber-200 bg-amber-50 p-3 text-sm text-amber-800">
          La lista no esta En Ejecucion: no se pueden registrar romaneos.
        </div>
      )}

      {/* Captura */}
      <div className="mb-4 rounded-lg border border-border bg-surface p-6 shadow-sm">
        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-4">
          <div>
            <label className="mb-1 block text-sm font-medium text-text">Renglon (tropa / categoria)</label>
            <select
              className="w-full rounded-lg border border-border px-3 py-2 text-sm"
              value={renglonId}
              onChange={(e) => setRenglonId(e.target.value)}
            >
              {data.renglones.map((r: RenglonEjecucionItem) => (
                <option key={r.renglonId} value={r.renglonId} disabled={r.pendiente <= 0}>
                  #{r.secuencia} · Tropa {r.numeroTropa} · {r.tipoEspecieNombre} · {r.almacenNombre}
                  {` (${r.cantidadFaenada}/${r.cantidad})`}
                </option>
              ))}
            </select>
            {renglonId === data.renglonSugeridoId && (
              <p className="mt-1 text-xs text-primary-600">Sugerido por secuencia</p>
            )}
          </div>

          <div>
            <label className="mb-1 block text-sm font-medium text-text">Unidad de faena</label>
            <select
              className="w-full rounded-lg border border-border px-3 py-2 text-sm"
              value={unidadFaenaId}
              onChange={(e) => setUnidadFaenaId(e.target.value)}
            >
              {unidadesFaenas.map((u) => (
                <option key={u.codigo} value={u.codigo}>{u.nombre}</option>
              ))}
            </select>
          </div>

          <div>
            <label className="mb-1 block text-sm font-medium text-text">Destino comercial</label>
            <select
              className="w-full rounded-lg border border-border px-3 py-2 text-sm"
              value={destinoId}
              onChange={(e) => setDestinoId(e.target.value)}
            >
              <option value="">Todos</option>
              {destinos.map((d) => (
                <option key={d.codigo} value={d.codigo}>{d.nombre}</option>
              ))}
            </select>
          </div>

          <div>
            <label className="mb-1 block text-sm font-medium text-text">Garron</label>
            <input
              type="number"
              min={1}
              className="w-full rounded-lg border border-border px-3 py-2 text-sm font-mono"
              value={garron}
              onChange={(e) => setGarron(Number(e.target.value))}
            />
          </div>
        </div>

        {/* Piezas */}
        <div className="mt-4">
          <h3 className="mb-2 text-sm font-semibold text-text">
            Piezas ({nroPiezas === 1 ? 'res completa' : `${nroPiezas} medias reses`})
          </h3>
          <div className="space-y-2">
            {piezas.map((p, idx) => (
              <div key={idx} className="grid grid-cols-1 items-end gap-3 sm:grid-cols-12">
                <div className="sm:col-span-1">
                  <label className="mb-1 block text-xs text-text-light">Letra</label>
                  <div className="rounded-lg border border-border bg-background px-3 py-2 text-center font-mono text-sm">
                    {nroPiezas > 1 ? LETRAS[idx] : '—'}
                  </div>
                </div>
                <div className="sm:col-span-2">
                  <label className="mb-1 block text-xs text-text-light">Peso (kg)</label>
                  <input
                    type="number"
                    step="0.01"
                    min={0}
                    className="w-full rounded-lg border border-border px-3 py-2 text-sm font-mono"
                    value={p.peso}
                    onChange={(e) => onPesoChange(idx, e.target.value)}
                  />
                </div>
                <div className="sm:col-span-4">
                  <label className="mb-1 block text-xs text-text-light">Camara destino</label>
                  <select
                    className="w-full rounded-lg border border-border px-3 py-2 text-sm"
                    value={p.almacenDestinoId}
                    onChange={(e) => onCamaraChange(idx, e.target.value)}
                  >
                    <option value="">Seleccionar...</option>
                    {data.camaras.map((c) => (
                      <option key={c.id} value={c.id}>{c.nombre}</option>
                    ))}
                  </select>
                </div>
                <div className="sm:col-span-5">
                  <label className="mb-1 block text-xs text-text-light">Tipificacion</label>
                  <select
                    className="w-full rounded-lg border border-border px-3 py-2 text-sm"
                    value={p.tipificacionId}
                    onChange={(e) => onTipificacionChange(idx, e.target.value)}
                  >
                    <option value="">Seleccionar...</option>
                    {candidatas.map((c) => (
                      <option key={c.codigo} value={c.codigo}>
                        {c.descripcion} ({c.pesoDesde}–{c.pesoHasta} kg
                        {c.destinoComercialNombre ? ` · ${c.destinoComercialNombre}` : ''})
                      </option>
                    ))}
                  </select>
                </div>
              </div>
            ))}
          </div>
        </div>

        <div className="mt-4 flex justify-end">
          <Button onClick={() => void guardar()} loading={saving} disabled={!enEjecucion || !renglonSel}>
            Registrar romaneo
          </Button>
        </div>
      </div>

      {/* Grilla de la jornada */}
      <div className="rounded-lg border border-border bg-surface p-6 shadow-sm">
        <h3 className="mb-3 text-sm font-semibold text-text">Romaneos de la jornada ({jornada.length})</h3>
        {jornada.length === 0 ? (
          <p className="text-sm text-text-light">Todavia no se registraron romaneos.</p>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full text-sm">
              <thead>
                <tr className="border-b border-border text-left text-text-light">
                  <th className="py-2 pr-3">N° Romaneo</th>
                  <th className="py-2 pr-3">Garron</th>
                  <th className="py-2 pr-3">Tropa</th>
                  <th className="py-2 pr-3">Categoria</th>
                  <th className="py-2 pr-3">Unidad</th>
                  <th className="py-2 pr-3">Piezas (peso · camara)</th>
                  <th className="py-2 pr-3 text-right">Kg</th>
                  <th className="py-2" />
                </tr>
              </thead>
              <tbody>
                {jornada.map((r) => (
                  <tr
                    key={r.id}
                    className={`border-b border-border/60 ${r.anulado ? 'text-text-light line-through' : ''}`}
                  >
                    <td className="py-2 pr-3 font-mono">{r.numeroRomaneo}</td>
                    <td className="py-2 pr-3 font-mono">{r.numeroGarron}</td>
                    <td className="py-2 pr-3 font-mono">{r.numeroTropa}</td>
                    <td className="py-2 pr-3">{r.tipoEspecieNombre}</td>
                    <td className="py-2 pr-3">{r.unidadFaenaNombre}</td>
                    <td className="py-2 pr-3">
                      {r.piezas
                        .map(
                          (p) =>
                            `${p.letra ? p.letra + ': ' : ''}${p.peso}kg → ${p.almacenDestinoNombre ?? '—'}`,
                        )
                        .join('  ·  ')}
                    </td>
                    <td className="py-2 pr-3 text-right font-mono">{r.pesoTotal.toFixed(2)}</td>
                    <td className="py-2 text-right">
                      {r.anulado ? (
                        <Badge variant="danger">Anulado</Badge>
                      ) : (
                        <button
                          onClick={() => setAnularTarget(r)}
                          className="rounded px-2 py-1 text-xs text-danger hover:bg-red-50"
                        >
                          Anular
                        </button>
                      )}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>

      <ConfirmDialog
        isOpen={!!anularTarget}
        onConfirm={async () => {
          const target = anularTarget
          setAnularTarget(null)
          if (!target) return
          try {
            await anularRomaneo(target.id)
            toast('success', `Romaneo N° ${target.numeroRomaneo} anulado`)
            await cargar()
          } catch (err) {
            toast('error', err instanceof Error ? err.message : 'Error al anular el romaneo')
          }
        }}
        onCancel={() => setAnularTarget(null)}
        title="Anular romaneo"
        message={`¿Anular el romaneo N° ${anularTarget?.numeroRomaneo} (garron ${anularTarget?.numeroGarron})? El animal vuelve a estar En Pie.`}
      />
    </>
  )
}
