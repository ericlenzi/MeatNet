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
import { getEjes } from '@/services/ejesTipificacion.service'
import { getDestinosComerciales } from '@/services/tipificaciones.service'
import { useToast } from '@/components/ui/Toast'
import { EstadoListaMatanza } from '@/types'
import type {
  RenglonesEjecucion,
  RenglonEjecucionItem,
  TipificacionCandidata,
  RomaneoJornadaItem,
  CatalogoFaenaOption,
  EjeTipificacion,
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
  tipoContusionId: string
  // Decomiso parcial: la inspeccion retira kilos de esta media res y la pieza sigue a camara.
  decomisoParcial: boolean
  motivoDecomisoId: string
  pesoDecomisado: string
}

const nuevaPieza = (almacenDestinoId = '', tipoContusionId = ''): PiezaState => ({
  peso: '',
  tipificacionId: '',
  almacenDestinoId,
  tipoContusionId,
  decomisoParcial: false,
  motivoDecomisoId: '',
  pesoDecomisado: '',
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
  // Los cuatro datos del palco: se determinan mirando la res, asi que se cargan por romaneo.
  // Si la especie de la jornada no tiene valores cargados, el combo no se muestra y el dato no se
  // exige. Es la misma regla que aplica el backend (R-E20), asi que vacuno los pide y porcino no.
  const [conformaciones, setConformaciones] = useState<EjeTipificacion[]>([])
  const [gradosEngrasamiento, setGradosEngrasamiento] = useState<EjeTipificacion[]>([])
  const [denticiones, setDenticiones] = useState<EjeTipificacion[]>([])
  const [tiposContusiones, setTiposContusiones] = useState<EjeTipificacion[]>([])
  // Motivos de decomiso de la especie. Si el catalogo esta vacio, la jornada no puede registrar
  // decomisos y ni el check ni los combos aparecen.
  const [motivosDecomiso, setMotivosDecomiso] = useState<EjeTipificacion[]>([])
  const [destinos, setDestinos] = useState<CatalogoFaenaOption[]>([])
  const [jornada, setJornada] = useState<RomaneoJornadaItem[]>([])

  const [renglonId, setRenglonId] = useState('')
  const [unidadFaenaId, setUnidadFaenaId] = useState('')
  const [destinoId, setDestinoId] = useState('')
  const [conformacionId, setConformacionId] = useState('')
  const [gradoEngrasamientoId, setGradoEngrasamientoId] = useState('')
  const [denticionId, setDenticionId] = useState('')
  // Decomiso total: la inspeccion condena la res entera. El animal se faena y se pesa igual, pero
  // no se clasifica ni entra a camara (R-E23).
  const [decomisoTotal, setDecomisoTotal] = useState(false)
  const [motivoDecomisoId, setMotivoDecomisoId] = useState('')
  const [garron, setGarron] = useState<number>(1)
  const [piezas, setPiezas] = useState<PiezaState[]>([nuevaPieza()])

  const [candidatas, setCandidatas] = useState<TipificacionCandidata[]>([])
  const [sticky, setSticky] = useState<StickyTip | null>(null)
  // Confirmacion explicita para registrar con el peso fuera del rango de la tipificacion.
  const [forzarFueraRango, setForzarFueraRango] = useState(false)

  const [loading, setLoading] = useState(true)
  const [saving, setSaving] = useState(false)
  const [anularTarget, setAnularTarget] = useState<RomaneoJornadaItem | null>(null)

  const renglonSel = useMemo(
    () => data?.renglones.find((r) => r.renglonId === renglonId) ?? null,
    [data, renglonId],
  )
  const ufSel = useMemo(
    () => unidadesFaenas.find((u) => u.id === unidadFaenaId),
    [unidadesFaenas, unidadFaenaId],
  )
  const nroPiezas = piezasEsperadas(ufSel)

  // Camara por defecto de cada pieza = la del animal programado (cámara del renglón/LM).
  const defaultCamara = renglonSel?.almacenDestinoId ?? ''
  const defaultCamaraRef = useRef(defaultCamara)
  defaultCamaraRef.current = defaultCamara

  // La contusion arranca en el primer valor de la escala, que es "sin contusion": es el caso
  // normal y la linea no puede frenarse a elegirlo res por res. El tipificador lo cambia cuando
  // ve el golpe. El catalogo viene ordenado por Orden, asi que el primero es el menos severo.
  const defaultContusion = tiposContusiones[0]?.codigo ?? ''
  const defaultContusionRef = useRef(defaultContusion)
  defaultContusionRef.current = defaultContusion

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

      const [ufs, dest, jorn, conf, grad, dent, cont, motivos] = await Promise.all([
        getUnidadesFaenasOptions(rengl.especieId),
        getDestinosComerciales(),
        getRomaneosJornada(listaMatanzaId),
        getEjes('conformaciones', { Estado: true, EspecieId: rengl.especieId, PageSize: 200 }),
        getEjes('grados-engrasamiento', { Estado: true, EspecieId: rengl.especieId, PageSize: 200 }),
        getEjes('denticiones', { Estado: true, EspecieId: rengl.especieId, PageSize: 200 }),
        getEjes('tipos-contusiones', { Estado: true, EspecieId: rengl.especieId, PageSize: 200 }),
        getEjes('motivos-decomisos', { Estado: true, EspecieId: rengl.especieId, PageSize: 200 }),
      ])
      setUnidadesFaenas(ufs)
      setDestinos(dest)
      setJornada(jorn)
      setConformaciones(conf.data || [])
      setGradosEngrasamiento(grad.data || [])
      setDenticiones(dent.data || [])
      setTiposContusiones(cont.data || [])
      setMotivosDecomiso(motivos.data || [])

      // Default del destino comercial: el marcado Favorito (si no hay, "Todos").
      setDestinoId((prev) => {
        if (prev && dest.some((d) => d.id === prev)) return prev
        return dest.find((d) => d.favorito)?.id ?? ''
      })

      setUnidadFaenaId((prev) => {
        if (prev && ufs.some((u) => u.id === prev)) return prev
        // Default: la unidad marcada PorDefecto para la especie; si no hay, la primera.
        const preferido = ufs.find((u) => u.porDefecto)
        return (preferido ?? ufs[0])?.id ?? ''
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

  // El catalogo de contusiones llega despues del primer render: al llegar se completa el valor
  // por defecto de las piezas que todavia no tienen uno elegido.
  useEffect(() => {
    if (!defaultContusion) return
    setPiezas((prev) =>
      prev.map((p) => (p.tipoContusionId ? p : { ...p, tipoContusionId: defaultContusion })),
    )
  }, [defaultContusion])

  // Ajustar la cantidad de piezas al cambiar la unidad de faena (las nuevas heredan la camara y la
  // contusion por defecto)
  useEffect(() => {
    setPiezas((prev) =>
      Array.from(
        { length: nroPiezas },
        (_, i) => prev[i] ?? nuevaPieza(defaultCamaraRef.current, defaultContusionRef.current),
      ),
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
        if (cancel) return
        setCandidatas(res.candidatas)
        // La tipificacion se propone ANTES del peso: al llegar el gancho el tipificador ya sabe
        // que esta tipificando. Se toma la de mayor Puntos (el backend ordena por Puntos desc),
        // o sea la mas usada para esta combinacion. El peso despues solo valida el rango.
        setSticky(null)
        const sugerida = res.candidatas[0]?.id ?? ''
        setPiezas((prev) => prev.map((p) => ({ ...p, tipificacionId: sugerida })))
      } catch {
        if (!cancel) setCandidatas([])
      }
    })()
    return () => {
      cancel = true
    }
  }, [renglonSel, unidadFaenaId, destinoId, data])

  // La tipificacion ya viene propuesta antes del peso. Al cargar el peso solo se reacomoda si el
  // operador NO eligio manualmente (sin sticky) y otra candidata cubre ese peso. Si ninguna lo
  // cubre se mantiene la elegida y la pieza queda fuera de rango (requiere confirmacion).
  const ajustarPorPeso = useCallback(
    (idActual: string, peso: number): string => {
      const actual = candidatas.find((c) => c.id === idActual)
      if (actual && peso >= actual.pesoDesde && peso <= actual.pesoHasta) return idActual
      if (sticky) return idActual
      const match = candidatas.find((c) => peso >= c.pesoDesde && peso <= c.pesoHasta)
      return match?.id ?? idActual
    },
    [sticky, candidatas],
  )

  // Peso cargado que cae fuera del rango de la tipificacion elegida.
  const piezaFueraRango = useCallback(
    (p: PiezaState): boolean => {
      const peso = Number(p.peso)
      if (!(peso > 0) || !p.tipificacionId) return false
      const c = candidatas.find((x) => x.id === p.tipificacionId)
      return !!c && (peso < c.pesoDesde || peso > c.pesoHasta)
    },
    [candidatas],
  )
  // La res condenada no se tipifica, asi que no hay rango contra el cual comparar su peso.
  const hayFueraRango = !decomisoTotal && piezas.some(piezaFueraRango)
  const detalleFueraRango = (() => {
    const p = piezas.find(piezaFueraRango)
    const c = p && candidatas.find((x) => x.id === p.tipificacionId)
    if (!p || !c) return null
    return `${p.peso} kg queda fuera del rango ${c.pesoDesde}–${c.pesoHasta} kg de "${c.descripcion}".`
  })()

  const onPesoChange = (idx: number, value: string) => {
    setPiezas((prev) => {
      const current = prev[idx]
      if (!current) return prev
      const next = [...prev]
      const peso = Number(value)
      const tipificacionId =
        peso > 0 ? ajustarPorPeso(current.tipificacionId, peso) : current.tipificacionId
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

  const onContusionChange = (idx: number, tipoContusionId: string) => {
    setPiezas((prev) => {
      const current = prev[idx]
      if (!current) return prev
      const next = [...prev]
      next[idx] = { ...current, tipoContusionId }
      return next
    })
  }

  // Decomiso parcial de una media res: motivo y kilos van juntos, asi que al destildar se
  // limpian los dos y la pieza vuelve a viajar sin decomiso.
  const onDecomisoParcialChange = (idx: number, decomisoParcial: boolean) => {
    setPiezas((prev) => {
      const current = prev[idx]
      if (!current) return prev
      const next = [...prev]
      next[idx] = decomisoParcial
        ? { ...current, decomisoParcial: true }
        : { ...current, decomisoParcial: false, motivoDecomisoId: '', pesoDecomisado: '' }
      return next
    })
  }

  const onDecomisoPiezaChange = (idx: number, campo: 'motivoDecomisoId' | 'pesoDecomisado', valor: string) => {
    setPiezas((prev) => {
      const current = prev[idx]
      if (!current) return prev
      const next = [...prev]
      next[idx] = { ...current, [campo]: valor }
      return next
    })
  }

  const onTipificacionChange = (idx: number, id: string) => {
    setPiezas((prev) => {
      const current = prev[idx]
      if (!current) return prev
      const next = [...prev]
      next[idx] = { ...current, tipificacionId: id }
      return next
    })
    const c = candidatas.find((x) => x.id === id)
    if (c) setSticky({ codigo: c.codigo, pesoDesde: c.pesoDesde, pesoHasta: c.pesoHasta })
  }

  const resetCaptura = (proximoGarron: number) => {
    setGarron(proximoGarron)
    setForzarFueraRango(false)
    // El decomiso NO persiste entre romaneos, al reves que los datos del palco: es la excepcion
    // de la jornada, y dejarlo tildado condenaria la res siguiente sin que nadie lo pida.
    setDecomisoTotal(false)
    setMotivoDecomisoId('')
    // La proxima pieza arranca con la tipificacion ya propuesta (la de mayor Puntos).
    const sugerida = candidatas[0]?.codigo ?? ''
    setPiezas(
      Array.from({ length: nroPiezas }, () => ({
        ...nuevaPieza(defaultCamaraRef.current, defaultContusionRef.current),
        tipificacionId: sugerida,
      })),
    )
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
    // La res condenada se pesa igual: esos kilos son la merma sanitaria de la jornada.
    if (piezas.some((p) => Number(p.peso) <= 0)) {
      toast('error', 'Cada pieza requiere un peso mayor a cero.')
      return
    }
    if (piezas.some((p) => !p.almacenDestinoId)) {
      toast('error', 'Cada pieza requiere una camara de destino.')
      return
    }
    if (decomisoTotal) {
      // La res condenada no se clasifica: solo se pide el motivo.
      if (!motivoDecomisoId) {
        toast('error', 'Indique el motivo por el que se condena la res.')
        return
      }
    } else {
      if (piezas.some((p) => !p.tipificacionId)) {
        toast('error', 'Cada pieza requiere una tipificacion.')
        return
      }
      // Los cuatro datos del palco son obligatorios cuando la especie los tiene cargados. El
      // backend valida lo mismo; esto solo evita el viaje y avisa antes.
      if (conformaciones.length > 0 && !conformacionId) {
        toast('error', 'Indique la conformacion de la res.')
        return
      }
      if (gradosEngrasamiento.length > 0 && !gradoEngrasamientoId) {
        toast('error', 'Indique el grado de engrasamiento de la res.')
        return
      }
      if (denticiones.length > 0 && !denticionId) {
        toast('error', 'Indique la denticion del animal.')
        return
      }
      if (tiposContusiones.length > 0 && piezas.some((p) => !p.tipoContusionId)) {
        toast('error', 'Indique la contusion de cada media res.')
        return
      }
      // Decomiso parcial: motivo y kilos van juntos, y los kilos no pueden alcanzar el peso de
      // la pieza; eso ya seria una condena y va por decomiso total.
      const parcial = piezas.find((p) => p.decomisoParcial)
      if (parcial) {
        if (!parcial.motivoDecomisoId) {
          toast('error', 'Indique el motivo del decomiso parcial de la pieza.')
          return
        }
        const kg = Number(parcial.pesoDecomisado)
        if (!(kg > 0)) {
          toast('error', 'Indique los kilos decomisados de la pieza.')
          return
        }
        if (kg >= Number(parcial.peso)) {
          toast('error', 'Los kilos decomisados no pueden alcanzar el peso de la pieza. Si se condena la res entera, use el decomiso total.')
          return
        }
      }
    }
    if (hayFueraRango && !forzarFueraRango) {
      toast('error', 'Hay un peso fuera del rango de su tipificacion. Confirme para registrarlo igual.')
      return
    }
    setSaving(true)
    try {
      const res = await crearRomaneo({
        ListaMatanzaId: data.listaMatanzaId,
        ListaMatanzaDetalleId: renglonSel.renglonId,
        UnidadFaenaId: unidadFaenaId,
        NumeroGarron: garron,
        // La res condenada viaja sin clasificacion: el backend descarta lo que llegue igual,
        // pero mandar el formulario vacio deja claro que no se tipifico (R-E23).
        ConformacionId: decomisoTotal ? undefined : conformacionId || undefined,
        GradoEngrasamientoId: decomisoTotal ? undefined : gradoEngrasamientoId || undefined,
        DenticionId: decomisoTotal ? undefined : denticionId || undefined,
        DecomisoTotal: decomisoTotal,
        MotivoDecomisoId: decomisoTotal ? motivoDecomisoId : undefined,
        Piezas: piezas.map((p) => ({
          AlmacenDestinoId: p.almacenDestinoId,
          TipificacionId: decomisoTotal ? undefined : p.tipificacionId,
          TipoContusionId: decomisoTotal ? undefined : p.tipoContusionId || undefined,
          Peso: Number(p.peso),
          ForzarFueraRango: !decomisoTotal && piezaFueraRango(p) && forzarFueraRango,
          MotivoDecomisoId:
            !decomisoTotal && p.decomisoParcial ? p.motivoDecomisoId : undefined,
          PesoDecomisado:
            !decomisoTotal && p.decomisoParcial ? Number(p.pesoDecomisado) : undefined,
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
            onClick={() => navigate(`/operaciones/monitor-faena/${data.listaMatanzaId}`)}
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
                <option key={u.id} value={u.id}>{u.nombre}</option>
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
                <option key={d.id} value={d.id}>{d.nombre}</option>
              ))}
            </select>
          </div>

          {!decomisoTotal && conformaciones.length > 0 && (
            <div>
              <label className="mb-1 block text-sm font-medium text-text">Conformacion *</label>
              <select
                className={`w-full rounded-lg border px-3 py-2 text-sm ${
                  conformacionId ? 'border-border' : 'border-danger bg-red-50'
                }`}
                value={conformacionId}
                onChange={(e) => setConformacionId(e.target.value)}
              >
                <option value="">Seleccionar...</option>
                {conformaciones.map((c) => (
                  <option key={c.codigo} value={c.codigo}>{c.codigo} - {c.nombre}</option>
                ))}
              </select>
            </div>
          )}

          {!decomisoTotal && gradosEngrasamiento.length > 0 && (
            <div>
              <label className="mb-1 block text-sm font-medium text-text">Engrasamiento *</label>
              <select
                className={`w-full rounded-lg border px-3 py-2 text-sm ${
                  gradoEngrasamientoId ? 'border-border' : 'border-danger bg-red-50'
                }`}
                value={gradoEngrasamientoId}
                onChange={(e) => setGradoEngrasamientoId(e.target.value)}
              >
                <option value="">Seleccionar...</option>
                {gradosEngrasamiento.map((g) => (
                  <option key={g.codigo} value={g.codigo}>{g.codigo} - {g.nombre}</option>
                ))}
              </select>
            </div>
          )}

          {!decomisoTotal && denticiones.length > 0 && (
            <div>
              <label className="mb-1 block text-sm font-medium text-text">Denticion *</label>
              <select
                className={`w-full rounded-lg border px-3 py-2 text-sm ${
                  denticionId ? 'border-border' : 'border-danger bg-red-50'
                }`}
                value={denticionId}
                onChange={(e) => setDenticionId(e.target.value)}
              >
                <option value="">Seleccionar...</option>
                {denticiones.map((d) => (
                  <option key={d.codigo} value={d.codigo}>{d.codigo} - {d.nombre}</option>
                ))}
              </select>
            </div>
          )}

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

        {/* Decomiso total: la inspeccion condena la res entera. Se pesa igual (son los kilos
            condenados de la jornada), pero no se tipifica ni entra a camara. */}
        {motivosDecomiso.length > 0 && (
          <div
            className={`mt-4 rounded-lg border px-3 py-2 ${
              decomisoTotal ? 'border-danger/40 bg-red-50' : 'border-border bg-background'
            }`}
          >
            <label className="flex items-center gap-2 text-sm font-medium text-text">
              <input
                type="checkbox"
                checked={decomisoTotal}
                onChange={(e) => setDecomisoTotal(e.target.checked)}
              />
              <span>Decomiso total: la inspeccion condena la res entera</span>
            </label>
            {decomisoTotal && (
              <div className="mt-2 grid grid-cols-1 gap-3 sm:grid-cols-2">
                <div>
                  <label className="mb-1 block text-xs text-text-light">Motivo *</label>
                  <select
                    className={`w-full rounded-lg border px-3 py-2 text-sm ${
                      motivoDecomisoId ? 'border-border' : 'border-danger bg-red-50'
                    }`}
                    value={motivoDecomisoId}
                    onChange={(e) => setMotivoDecomisoId(e.target.value)}
                  >
                    <option value="">Seleccionar...</option>
                    {motivosDecomiso.map((m) => (
                      <option key={m.codigo} value={m.codigo}>{m.nombre}</option>
                    ))}
                  </select>
                </div>
                <p className="self-end text-xs text-text-light">
                  La res se pesa igual: esos kilos son la merma sanitaria de la jornada. No se
                  tipifica ni genera existencia de camara al liberar.
                </p>
              </div>
            )}
          </div>
        )}

        {/* Piezas */}
        <div className="mt-4">
          <h3 className="mb-2 text-sm font-semibold text-text">
            Piezas ({nroPiezas === 1 ? 'res completa' : `${nroPiezas} medias reses`})
          </h3>
          {!decomisoTotal && renglonSel && unidadFaenaId && candidatas.length === 0 && (
            <div className="mb-2 rounded-lg border border-danger/40 bg-red-50 px-3 py-2 text-sm text-danger">
              No hay tipificacion configurada para esta combinacion (categoria / unidad de faena /
              destino comercial). Revisela en el ABM de Tipificaciones antes de romanear.
            </div>
          )}
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
                    className={`w-full rounded-lg border px-3 py-2 text-sm font-mono ${
                      piezaFueraRango(p) ? 'border-danger bg-red-50' : 'border-border'
                    }`}
                    value={p.peso}
                    onChange={(e) => onPesoChange(idx, e.target.value)}
                  />
                </div>
                <div
                  className={
                    decomisoTotal
                      ? 'sm:col-span-9'
                      : tiposContusiones.length > 0
                        ? 'sm:col-span-3'
                        : 'sm:col-span-4'
                  }
                >
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
                {!decomisoTotal && tiposContusiones.length > 0 && (
                  <div className="sm:col-span-2">
                    <label className="mb-1 block text-xs text-text-light">Contusion *</label>
                    <select
                      className={`w-full rounded-lg border px-3 py-2 text-sm ${
                        p.tipoContusionId ? 'border-border' : 'border-danger bg-red-50'
                      }`}
                      value={p.tipoContusionId}
                      onChange={(e) => onContusionChange(idx, e.target.value)}
                    >
                      <option value="">Seleccionar...</option>
                      {tiposContusiones.map((t) => (
                        <option key={t.codigo} value={t.codigo}>{t.nombre}</option>
                      ))}
                    </select>
                  </div>
                )}
                {!decomisoTotal && (
                <div className={tiposContusiones.length > 0 ? 'sm:col-span-4' : 'sm:col-span-5'}>
                  <label className="mb-1 block text-xs text-text-light">Tipificacion</label>
                  <select
                    className="w-full rounded-lg border border-border px-3 py-2 text-sm"
                    value={p.tipificacionId}
                    onChange={(e) => onTipificacionChange(idx, e.target.value)}
                  >
                    <option value="">Seleccionar...</option>
                    {candidatas.map((c) => (
                      <option key={c.id} value={c.id}>
                        {c.descripcion} ({c.pesoDesde}–{c.pesoHasta} kg
                        {c.destinoComercialNombre ? ` · ${c.destinoComercialNombre}` : ''})
                      </option>
                    ))}
                  </select>
                </div>
                )}

                {/* Decomiso parcial: la inspeccion retira kilos de esta media res y la pieza
                    sigue su curso a camara. Los kilos no descuentan el peso: se informan como
                    merma sanitaria. */}
                {!decomisoTotal && motivosDecomiso.length > 0 && (
                  <div className="sm:col-span-12">
                    <label className="flex items-center gap-2 text-xs text-text-light">
                      <input
                        type="checkbox"
                        checked={p.decomisoParcial}
                        onChange={(e) => onDecomisoParcialChange(idx, e.target.checked)}
                      />
                      <span>Decomiso parcial de esta pieza</span>
                    </label>
                    {p.decomisoParcial && (
                      <div className="mt-2 grid grid-cols-1 gap-3 sm:grid-cols-12">
                        <div className="sm:col-span-6">
                          <label className="mb-1 block text-xs text-text-light">Motivo *</label>
                          <select
                            className={`w-full rounded-lg border px-3 py-2 text-sm ${
                              p.motivoDecomisoId ? 'border-border' : 'border-danger bg-red-50'
                            }`}
                            value={p.motivoDecomisoId}
                            onChange={(e) =>
                              onDecomisoPiezaChange(idx, 'motivoDecomisoId', e.target.value)
                            }
                          >
                            <option value="">Seleccionar...</option>
                            {motivosDecomiso.map((m) => (
                              <option key={m.codigo} value={m.codigo}>{m.nombre}</option>
                            ))}
                          </select>
                        </div>
                        <div className="sm:col-span-3">
                          <label className="mb-1 block text-xs text-text-light">Kg decomisados *</label>
                          <input
                            type="number"
                            step="0.01"
                            min={0}
                            className={`w-full rounded-lg border px-3 py-2 text-sm font-mono ${
                              Number(p.pesoDecomisado) > 0
                                ? 'border-border'
                                : 'border-danger bg-red-50'
                            }`}
                            value={p.pesoDecomisado}
                            onChange={(e) =>
                              onDecomisoPiezaChange(idx, 'pesoDecomisado', e.target.value)
                            }
                          />
                        </div>
                        <p className="self-end text-xs text-text-light sm:col-span-3">
                          No descuentan el peso de la pieza.
                        </p>
                      </div>
                    )}
                  </div>
                )}
              </div>
            ))}
          </div>
        </div>

        {hayFueraRango && (
          <div className="mt-3 rounded-lg border border-amber-300 bg-amber-50 px-3 py-2 text-sm text-amber-900">
            <p className="font-medium">Peso fuera de rango</p>
            <p className="mt-0.5">{detalleFueraRango}</p>
            <label className="mt-2 flex items-center gap-2">
              <input
                type="checkbox"
                checked={forzarFueraRango}
                onChange={(e) => setForzarFueraRango(e.target.checked)}
              />
              <span>Confirmo el registro; la pieza quedara marcada como fuera de rango.</span>
            </label>
          </div>
        )}

        <div className="mt-4 flex justify-end">
          <Button
            onClick={() => void guardar()}
            loading={saving}
            disabled={
              !enEjecucion ||
              !renglonSel ||
              (!decomisoTotal && candidatas.length === 0) ||
              (hayFueraRango && !forzarFueraRango)
            }
          >
            Registrar romaneo
          </Button>
        </div>
      </div>

      {/* Grilla de la jornada */}
      <div className="rounded-lg border border-border bg-surface p-6 shadow-sm">
        <h3 className="mb-3 text-sm font-semibold text-text">Reses faenadas de la jornada ({jornada.length})</h3>
        {jornada.length === 0 ? (
          <p className="text-sm text-text-light">Todavia no se registraron reses.</p>
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
                    <td className="py-2 pr-3">
                      {r.unidadFaenaNombre}
                      {r.decomisoTotal && (
                        <span
                          className="ml-1 rounded bg-red-100 px-1 text-xs text-danger"
                          title="Res condenada entera: no entra a camara"
                        >
                          decomiso{r.motivoDecomisoNombre ? `: ${r.motivoDecomisoNombre}` : ''}
                        </span>
                      )}
                    </td>
                    <td className="py-2 pr-3">
                      {r.piezas.map((p, i) => (
                        <span key={i}>
                          {i > 0 && '  ·  '}
                          {p.letra ? `${p.letra}: ` : ''}
                          {p.peso}kg → {p.almacenDestinoNombre ?? '—'}
                          {p.tipoContusionNombre && (
                            <span className="ml-1 text-xs text-text-light">
                              ({p.tipoContusionNombre})
                            </span>
                          )}
                          {p.motivoDecomisoNombre && (
                            <span
                              className="ml-1 rounded bg-red-100 px-1 text-xs text-danger"
                              title="Decomiso parcial: kilos retirados por la inspeccion"
                            >
                              -{p.pesoDecomisado}kg {p.motivoDecomisoNombre}
                            </span>
                          )}
                          {p.pesoFueraRango && (
                            <span
                              className="ml-1 rounded bg-amber-100 px-1 text-xs text-amber-800"
                              title="Peso fuera del rango de la tipificacion, registrado con confirmacion"
                            >
                              fuera de rango
                            </span>
                          )}
                        </span>
                      ))}
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
