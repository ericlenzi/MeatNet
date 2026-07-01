import { useState, useEffect } from 'react'
import type { FormEvent } from 'react'
import { useParams, useNavigate, useLocation } from 'react-router'
import {
  getIngresoHacienda,
  createIngresoHacienda,
  updateIngresoHacienda,
  enviarAprobacionIngresoHacienda,
} from '@/services/ingresosHacienda.service'
import { getClientes, getClienteEstablecimientos } from '@/services/clientes.service'
import type { ClienteEstablecimientoItem } from '@/services/clientes.service'
import { getProvincias } from '@/services/provincias.service'
import type { ProvinciaItem } from '@/services/provincias.service'
import { getAlmacenes } from '@/services/almacenes.service'
import type { AlmacenItem } from '@/services/almacenes.service'
import { getOrigenesHaciendas } from '@/services/origenesHaciendas.service'
import { getUsosHaciendas } from '@/services/usosHaciendas.service'
import { getTiposEspecies } from '@/services/tiposEspecies.service'
import { useApp } from '@/contexts/AppContext'
import { useToast } from '@/components/ui/Toast'
import { EstadoIngreso, EstadoHacienda } from '@/types'
import type { Cliente, OrigenHacienda, UsoHacienda, TipoEspecie } from '@/types'
import Input from '@/components/ui/Input'
import Select from '@/components/ui/Select'
import Button from '@/components/ui/Button'
import PageHeader from '@/components/ui/PageHeader'
import Spinner from '@/components/ui/Spinner'

const CORRAL_CAIDOS = 'CORRAL_CAIDOS'
const CORRAL_MUERTOS = 'CORRAL_MUERTOS'

const estadoHaciendaOptions = [
  { value: EstadoHacienda.EnPie, label: 'En Pie' },
  { value: EstadoHacienda.Caidos, label: 'Caidos' },
  { value: EstadoHacienda.Muertos, label: 'Muertos' },
]

function pad(n: number): string {
  return String(n).padStart(2, '0')
}
function toLocalInput(d: Date): string {
  return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}T${pad(d.getHours())}:${pad(d.getMinutes())}`
}
function toDateInput(d: Date): string {
  return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}`
}
const num = (v: string | number): number => Number(v) || 0

interface PesadaRow { TipoEspecieId: string; PesoIngreso: string; IdPesada: string }
interface UbicacionRow { TipoEspecieId: string; AlmacenId: string; Cantidad: string; EstadoHaciendaId: string }

export default function IngresoHaciendaFormPage() {
  const { id } = useParams()
  const navigate = useNavigate()
  const location = useLocation()
  const backTo = (location.state as { from?: string } | null)?.from ?? '/operaciones/ingreso-hacienda'
  const { toast } = useToast()
  const { currentEstablecimiento, especies, currentEspecie } = useApp()
  const isEdit = !!id

  const [fetching, setFetching] = useState(true)
  const [loading, setLoading] = useState(false)
  const [estadoIngresoId, setEstadoIngresoId] = useState<string>(EstadoIngreso.Borrador)
  const [establecimientoNombre, setEstablecimientoNombre] = useState('')

  const [clientes, setClientes] = useState<Cliente[]>([])
  const [clienteEstablecimientos, setClienteEstablecimientos] = useState<ClienteEstablecimientoItem[]>([])
  const [provincias, setProvincias] = useState<ProvinciaItem[]>([])
  const [origenes, setOrigenes] = useState<OrigenHacienda[]>([])
  const [usos, setUsos] = useState<UsoHacienda[]>([])
  const [tiposEspecies, setTiposEspecies] = useState<TipoEspecie[]>([])
  const [almacenes, setAlmacenes] = useState<AlmacenItem[]>([])

  const [form, setForm] = useState({
    FechaHoraIngreso: toLocalInput(new Date()),
    EspecieId: '',
    NumeroDte: '',
    FechaEmisionDte: toDateInput(new Date()),
    ClienteId: '',
    ClienteEstablecimientoId: '',
    ProvinciaId: '',
    Localidad: '',
    OrigenHaciendaId: '',
    UsoHaciendaId: '',
    Transportista: '',
    Chofer: '',
    PatenteCamion: '',
    PatenteJaula: '',
  })
  const [pesadas, setPesadas] = useState<PesadaRow[]>([])
  const [ubicaciones, setUbicaciones] = useState<UbicacionRow[]>([])
  const [errors, setErrors] = useState<Record<string, string>>({})

  const readOnly = isEdit && estadoIngresoId !== EstadoIngreso.Borrador

  useEffect(() => {
    const load = async () => {
      try {
        const [clientesRes, prov, orig, uso, tiposRes, alm] = await Promise.all([
          getClientes({ PageSize: 1000, Estado: true }),
          getProvincias(),
          getOrigenesHaciendas(),
          getUsosHaciendas(),
          getTiposEspecies({ PageSize: 1000, Estado: true }),
          getAlmacenes({ EstablecimientoId: currentEstablecimiento?.id, Estado: true }),
        ])
        setClientes(clientesRes.data || [])
        setProvincias(prov)
        setOrigenes(orig)
        setUsos(uso)
        setTiposEspecies(tiposRes.data || [])
        setAlmacenes(alm)

        if (isEdit && id) {
          const e = await getIngresoHacienda(id)
          setEstadoIngresoId(e.estadoIngresoId)
          setEstablecimientoNombre(e.establecimientoNombre)
          setForm({
            FechaHoraIngreso: toLocalInput(new Date(e.fechaHoraIngreso)),
            EspecieId: e.especieId ?? '',
            NumeroDte: e.numeroDte ?? '',
            FechaEmisionDte: toDateInput(new Date(e.fechaEmisionDte)),
            ClienteId: e.clienteId,
            ClienteEstablecimientoId: e.clienteEstablecimientoId,
            ProvinciaId: String(e.provinciaId),
            Localidad: e.localidad ?? '',
            OrigenHaciendaId: e.origenHaciendaId ?? '',
            UsoHaciendaId: e.usoHaciendaId ?? '',
            Transportista: e.transportista ?? '',
            Chofer: e.chofer ?? '',
            PatenteCamion: e.patenteCamion ?? '',
            PatenteJaula: e.patenteJaula ?? '',
          })
          setPesadas(e.pesadas.map((p) => ({ TipoEspecieId: p.tipoEspecieId, PesoIngreso: String(p.pesoIngreso), IdPesada: p.idPesada ?? '' })))
          setUbicaciones(e.ubicaciones.map((u) => ({
            TipoEspecieId: u.tipoEspecieId,
            AlmacenId: u.almacenId,
            Cantidad: String(u.cantidad),
            EstadoHaciendaId: u.estadoHaciendaId,
          })))
          if (e.clienteId) {
            const ces = await getClienteEstablecimientos(e.clienteId)
            setClienteEstablecimientos(ces)
          }
        } else {
          setEstablecimientoNombre(currentEstablecimiento?.nombre ?? '')
        }
      } catch {
        toast('error', 'Error al cargar datos')
      } finally {
        setFetching(false)
      }
    }
    void load()
  }, [id, isEdit, currentEstablecimiento?.id, currentEstablecimiento?.nombre, toast])

  // Ingreso nuevo: por defecto la especie activa del establecimiento (como en el Header)
  useEffect(() => {
    if (isEdit || fetching) return
    if (!form.EspecieId && currentEspecie) {
      setForm((p) => ({ ...p, EspecieId: currentEspecie.id }))
    }
  }, [isEdit, fetching, currentEspecie, form.EspecieId])

  const handleEspecieChange = (especieId: string) => {
    if (especieId === form.EspecieId) return
    setForm((p) => ({ ...p, EspecieId: especieId }))
    setErrors((p) => ({ ...p, EspecieId: '' }))
    // Al cambiar la especie, el detalle depende de ella: se reinicia
    setPesadas([])
    setUbicaciones([])
  }

  const handleClienteChange = async (clienteId: string) => {
    setForm((p) => ({ ...p, ClienteId: clienteId, ClienteEstablecimientoId: '' }))
    setClienteEstablecimientos([])
    if (!clienteId) return
    try {
      const ces = await getClienteEstablecimientos(clienteId)
      setClienteEstablecimientos(ces)
    } catch {
      setClienteEstablecimientos([])
    }
  }

  const updateField = (field: string, value: string) => {
    setForm((p) => ({ ...p, [field]: value }))
    setErrors((p) => ({ ...p, [field]: '' }))
  }

  // --- Calculos ---
  const pesoTeoricoById = new Map(tiposEspecies.map((t) => [t.id, t.pesoTeorico]))
  const pesoPorTipo = new Map<string, number>()
  pesadas.forEach((p) => pesoPorTipo.set(p.TipoEspecieId, (pesoPorTipo.get(p.TipoEspecieId) ?? 0) + num(p.PesoIngreso)))
  const cantidadPorTipo = new Map<string, number>()
  ubicaciones.forEach((u) => cantidadPorTipo.set(u.TipoEspecieId, (cantidadPorTipo.get(u.TipoEspecieId) ?? 0) + num(u.Cantidad)))

  const pesoPromedio = (tipoEspecieId: string): number => {
    const peso = pesoPorTipo.get(tipoEspecieId) ?? 0
    const cant = cantidadPorTipo.get(tipoEspecieId) ?? 0
    return cant > 0 ? peso / cant : 0
  }

  const corralOptions = (estadoHaciendaId: string) => {
    let filtered = almacenes
    if (estadoHaciendaId === EstadoHacienda.EnPie) {
      filtered = almacenes.filter((a) => a.tipoAlmacenId !== CORRAL_CAIDOS && a.tipoAlmacenId !== CORRAL_MUERTOS)
    } else if (estadoHaciendaId === EstadoHacienda.Caidos) {
      filtered = almacenes.filter((a) => a.tipoAlmacenId === CORRAL_CAIDOS)
    } else if (estadoHaciendaId === EstadoHacienda.Muertos) {
      filtered = almacenes.filter((a) => a.tipoAlmacenId === CORRAL_MUERTOS)
    }
    return filtered.map((a) => ({ value: a.id, label: `${a.codigoAlmacen} - ${a.nombre} (cap. ${a.cantidadAnimales})` }))
  }

  // Los tipos de especie a pesar se limitan a la Especie elegida en el ingreso
  const tipoEspecieOptions = tiposEspecies
    .filter((t) => !form.EspecieId || t.especieId === form.EspecieId)
    .map((t) => ({ value: t.id, label: t.nombre }))

  // En Corrales solo se pueden ubicar los tipos de especie cargados en el registro de pesadas
  const tipoEspecieIdsPesadas = new Set(pesadas.map((p) => p.TipoEspecieId).filter(Boolean))
  const tipoEspecieOptionsUbicacion = tipoEspecieOptions.filter((o) => tipoEspecieIdsPesadas.has(o.value))

  // No se puede quitar (ni cambiar el tipo de) una pesada cuyo tipo de especie tiene ubicacion
  // en corrales, salvo que otra pesada mantenga ese mismo tipo de especie.
  const tipoEspecieIdsUbicaciones = new Set(ubicaciones.map((u) => u.TipoEspecieId).filter(Boolean))
  const pesadaBloqueada = (row: PesadaRow): boolean =>
    !!row.TipoEspecieId &&
    tipoEspecieIdsUbicaciones.has(row.TipoEspecieId) &&
    pesadas.filter((p) => p.TipoEspecieId === row.TipoEspecieId).length === 1

  // --- Rows handlers ---
  const addPesada = () => setPesadas((p) => [...p, { TipoEspecieId: '', PesoIngreso: '', IdPesada: '' }])
  const removePesada = (i: number) => {
    const row = pesadas[i]
    if (row && pesadaBloqueada(row)) {
      toast('error', 'No se puede eliminar: el tipo de especie tiene ubicacion en corrales. Quite primero la ubicacion.')
      return
    }
    setPesadas((p) => p.filter((_, idx) => idx !== i))
  }
  const updatePesada = (i: number, field: keyof PesadaRow, value: string) => {
    const row = pesadas[i]
    if (field === 'TipoEspecieId' && row && value !== row.TipoEspecieId && pesadaBloqueada(row)) {
      toast('error', 'No se puede cambiar el tipo de especie: tiene ubicacion en corrales. Quite primero la ubicacion.')
      return
    }
    setPesadas((p) => p.map((r, idx) => (idx === i ? { ...r, [field]: value } : r)))
  }

  const addUbicacion = () => setUbicaciones((u) => [...u, { TipoEspecieId: '', AlmacenId: '', Cantidad: '', EstadoHaciendaId: EstadoHacienda.EnPie }])
  const removeUbicacion = (i: number) => setUbicaciones((u) => u.filter((_, idx) => idx !== i))
  const updateUbicacion = (i: number, field: keyof UbicacionRow, value: string) =>
    setUbicaciones((u) => u.map((row, idx) => {
      if (idx !== i) return row
      const next = { ...row, [field]: value }
      if (field === 'EstadoHaciendaId') next.AlmacenId = '' // reset corral al cambiar estado
      return next
    }))

  const validate = (requireUbicaciones: boolean): boolean => {
    const e: Record<string, string> = {}
    if (!form.FechaHoraIngreso) e['FechaHoraIngreso'] = 'Requerido'
    if (!form.EspecieId) e['EspecieId'] = 'Requerido'
    if (!form.ClienteId) e['ClienteId'] = 'Requerido'
    if (!form.ClienteEstablecimientoId) e['ClienteEstablecimientoId'] = 'Requerido'
    if (!form.ProvinciaId) e['ProvinciaId'] = 'Requerido'
    if (requireUbicaciones && ubicaciones.length === 0) e['Ubicaciones'] = 'Cargue al menos una ubicacion'
    setErrors(e)
    if (Object.keys(e).length > 0) toast('error', 'Revise los campos requeridos')
    return Object.keys(e).length === 0
  }

  const buildPayload = () => ({
    FechaHoraIngreso: new Date(form.FechaHoraIngreso).toISOString(),
    EspecieId: form.EspecieId,
    NumeroDte: form.NumeroDte,
    FechaEmisionDte: new Date(form.FechaEmisionDte).toISOString(),
    ClienteId: form.ClienteId,
    ClienteEstablecimientoId: form.ClienteEstablecimientoId,
    ProvinciaId: num(form.ProvinciaId),
    Localidad: form.Localidad,
    OrigenHaciendaId: form.OrigenHaciendaId || null,
    UsoHaciendaId: form.UsoHaciendaId || null,
    Transportista: form.Transportista,
    Chofer: form.Chofer,
    PatenteCamion: form.PatenteCamion,
    PatenteJaula: form.PatenteJaula,
    Pesadas: pesadas
      .filter((p) => p.TipoEspecieId)
      .map((p) => ({ TipoEspecieId: p.TipoEspecieId, PesoIngreso: num(p.PesoIngreso), IdPesada: p.IdPesada })),
    Ubicaciones: ubicaciones
      .filter((u) => u.TipoEspecieId && u.AlmacenId)
      .map((u) => ({
        TipoEspecieId: u.TipoEspecieId,
        AlmacenId: u.AlmacenId,
        Cantidad: num(u.Cantidad),
        EstadoHaciendaId: u.EstadoHaciendaId,
      })),
  })

  const persist = async (): Promise<string> => {
    const payload = buildPayload()
    if (isEdit && id) {
      await updateIngresoHacienda(id, payload)
      return id
    }
    const res = await createIngresoHacienda({ ...payload, EstablecimientoId: currentEstablecimiento?.id ?? '' })
    return res.id
  }

  const handleSubmit = async (ev: FormEvent) => {
    ev.preventDefault()
    if (!validate(false)) return
    setLoading(true)
    try {
      await persist()
      toast('success', isEdit ? 'Ingreso actualizado' : 'Ingreso creado')
      navigate(backTo)
    } catch (err) {
      toast('error', err instanceof Error ? err.message : 'Error al guardar')
    } finally {
      setLoading(false)
    }
  }

  const handleEnviarAprobacion = async () => {
    if (!validate(true)) return
    setLoading(true)
    try {
      const savedId = await persist()
      await enviarAprobacionIngresoHacienda(savedId)
      toast('success', 'Ingreso enviado a aprobacion')
      navigate(backTo)
    } catch (err) {
      toast('error', err instanceof Error ? err.message : 'Error al enviar a aprobacion')
    } finally {
      setLoading(false)
    }
  }

  if (fetching) {
    return (
      <div className="flex items-center justify-center py-20">
        <Spinner size="lg" />
      </div>
    )
  }

  const cardClass = 'rounded-lg border border-border bg-surface p-6 shadow-sm'
  const sectionTitle = 'mb-3 text-sm font-semibold uppercase tracking-wide text-text-light'
  const dividerSection = 'mt-6 border-t border-border pt-6'

  return (
    <>
      <PageHeader
        title={isEdit ? `Ingreso de Hacienda${readOnly ? ' (solo lectura)' : ''}` : 'Nuevo Ingreso de Hacienda'}
      />

      <form onSubmit={handleSubmit} className="mx-auto max-w-4xl space-y-5">
        {/* CUADRO 1 - Detalle Ingreso */}
        <div className={cardClass}>
          <h2 className="mb-4 text-base font-semibold text-text">Detalle Ingreso</h2>

          <p className={sectionTitle}>Datos de ingreso</p>
          <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
            <Input
              label="Fecha y hora de ingreso"
              type="datetime-local"
              value={form.FechaHoraIngreso}
              onChange={(e) => updateField('FechaHoraIngreso', e.target.value)}
              error={errors['FechaHoraIngreso']}
              disabled={readOnly}
            />
            <Input label="Establecimiento" value={establecimientoNombre} disabled />
            <Select
              label="Especie"
              value={form.EspecieId}
              onChange={(e) => handleEspecieChange(e.target.value)}
              options={especies.map((esp) => ({ value: esp.id, label: esp.nombre }))}
              placeholder="Seleccionar especie..."
              error={errors['EspecieId']}
              disabled={readOnly}
            />
          </div>

          <p className={`${sectionTitle} ${dividerSection}`}>Datos de origen</p>
          <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
            <Input label="N° DT-e" value={form.NumeroDte} onChange={(e) => updateField('NumeroDte', e.target.value)} disabled={readOnly} />
            <Input label="Fecha emision DT-e" type="date" value={form.FechaEmisionDte} onChange={(e) => updateField('FechaEmisionDte', e.target.value)} disabled={readOnly} />
            <Select
              label="Cliente"
              value={form.ClienteId}
              onChange={(e) => handleClienteChange(e.target.value)}
              options={clientes.map((c) => ({ value: c.id, label: `${c.codigoCliente} - ${c.nombre}` }))}
              placeholder="Seleccionar cliente..."
              error={errors['ClienteId']}
              disabled={readOnly}
            />
            <Select
              label="Procedencia (Establecimiento / RENSPA)"
              value={form.ClienteEstablecimientoId}
              onChange={(e) => updateField('ClienteEstablecimientoId', e.target.value)}
              options={clienteEstablecimientos.map((ce) => ({
                value: ce.id,
                label: `${ce.nombre}${ce.codigoRenspa ? ` - RENSPA ${ce.codigoRenspa}` : ''}`,
              }))}
              placeholder={form.ClienteId ? 'Seleccionar procedencia...' : 'Primero seleccione un cliente'}
              error={errors['ClienteEstablecimientoId']}
              disabled={readOnly || !form.ClienteId}
            />
            <Select
              label="Provincia"
              value={form.ProvinciaId}
              onChange={(e) => updateField('ProvinciaId', e.target.value)}
              options={provincias.map((p) => ({ value: String(p.id), label: p.nombre }))}
              placeholder="Seleccionar provincia..."
              error={errors['ProvinciaId']}
              disabled={readOnly}
            />
            <Input label="Localidad" value={form.Localidad} onChange={(e) => updateField('Localidad', e.target.value)} disabled={readOnly} />
            <Select
              label="Origen de hacienda"
              value={form.OrigenHaciendaId}
              onChange={(e) => updateField('OrigenHaciendaId', e.target.value)}
              options={origenes.map((o) => ({ value: o.codigo, label: o.nombre }))}
              placeholder="Seleccionar origen..."
              disabled={readOnly}
            />
            <Select
              label="Uso de hacienda"
              value={form.UsoHaciendaId}
              onChange={(e) => updateField('UsoHaciendaId', e.target.value)}
              options={usos.map((u) => ({ value: u.codigo, label: u.nombre }))}
              placeholder="Seleccionar uso..."
              disabled={readOnly}
            />
          </div>

          <p className={`${sectionTitle} ${dividerSection}`}>Datos de transporte</p>
          <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
            <Input label="Transportista" value={form.Transportista} onChange={(e) => updateField('Transportista', e.target.value)} disabled={readOnly} />
            <Input label="Chofer" value={form.Chofer} onChange={(e) => updateField('Chofer', e.target.value)} disabled={readOnly} />
            <Input label="Patente camion" value={form.PatenteCamion} onChange={(e) => updateField('PatenteCamion', e.target.value)} disabled={readOnly} />
            <Input label="Patente jaula" value={form.PatenteJaula} onChange={(e) => updateField('PatenteJaula', e.target.value)} disabled={readOnly} />
          </div>
        </div>

        {/* CUADRO 2 - Registro de pesadas */}
        <div className={cardClass}>
          <div className="mb-4 flex items-center justify-between">
            <h2 className="text-base font-semibold text-text">Registro de pesadas</h2>
            {!readOnly && <Button type="button" size="sm" variant="secondary" onClick={addPesada}>+ Agregar</Button>}
          </div>

          {pesadas.length === 0 && <p className="text-sm text-text-light">Sin pesadas cargadas.</p>}

          <div className="space-y-2">
            {pesadas.map((p, i) => {
              const peso = num(p.PesoIngreso)
              const teorico = pesoTeoricoById.get(p.TipoEspecieId) ?? 0
              const estimado = teorico > 0 ? Math.round(peso / teorico) : 0
              const bloqueada = pesadaBloqueada(p)
              return (
                <div key={i} className="grid grid-cols-1 items-end gap-3 sm:grid-cols-12">
                  <div className="sm:col-span-4">
                    <Select
                      label={i === 0 ? 'Tipo especie' : undefined}
                      value={p.TipoEspecieId}
                      onChange={(e) => updatePesada(i, 'TipoEspecieId', e.target.value)}
                      options={tipoEspecieOptions}
                      placeholder="Seleccionar..."
                      disabled={readOnly || bloqueada}
                    />
                  </div>
                  <div className="sm:col-span-2">
                    <Input label={i === 0 ? 'Peso ingreso' : undefined} type="number" value={p.PesoIngreso} onChange={(e) => updatePesada(i, 'PesoIngreso', e.target.value)} disabled={readOnly} />
                  </div>
                  <div className="sm:col-span-1">
                    <Input label={i === 0 ? 'UM' : undefined} value="KG" disabled />
                  </div>
                  <div className="sm:col-span-2">
                    <Input label={i === 0 ? 'Cant. estimada (UN)' : undefined} value={String(estimado)} disabled />
                  </div>
                  <div className="sm:col-span-2">
                    <Input label={i === 0 ? 'ID Pesada (ticket)' : undefined} value={p.IdPesada} onChange={(e) => updatePesada(i, 'IdPesada', e.target.value)} disabled={readOnly} />
                  </div>
                  {!readOnly && (
                    <div className="sm:col-span-1">
                      <Button
                        type="button"
                        size="sm"
                        variant="ghost"
                        onClick={() => removePesada(i)}
                        disabled={bloqueada}
                        title={bloqueada ? 'Tiene ubicacion en corrales; quite primero la ubicacion' : undefined}
                      >
                        ✕
                      </Button>
                    </div>
                  )}
                </div>
              )
            })}
          </div>
        </div>

        {/* CUADRO 3 - Ubicacion en Corrales */}
        <div className={cardClass}>
          <div className="mb-4 flex items-center justify-between">
            <h2 className="text-base font-semibold text-text">Ubicación en Corrales</h2>
            {!readOnly && <Button type="button" size="sm" variant="secondary" onClick={addUbicacion}>+ Agregar</Button>}
          </div>
          {errors['Ubicaciones'] && <p className="mb-2 text-xs text-danger">{errors['Ubicaciones']}</p>}
          {tipoEspecieOptionsUbicacion.length === 0 && <p className="mb-2 text-xs text-text-light">Cargue tipos de especie en el registro de pesadas para poder ubicarlos.</p>}
          {ubicaciones.length === 0 && <p className="text-sm text-text-light">Sin ubicaciones cargadas.</p>}

          <div className="space-y-2">
            {ubicaciones.map((u, i) => (
              <div key={i} className="grid grid-cols-1 items-end gap-3 sm:grid-cols-12">
                <div className="sm:col-span-3">
                  <Select
                    label={i === 0 ? 'Tipo especie' : undefined}
                    value={u.TipoEspecieId}
                    onChange={(e) => updateUbicacion(i, 'TipoEspecieId', e.target.value)}
                    options={tipoEspecieOptionsUbicacion}
                    placeholder="Seleccionar..."
                    disabled={readOnly}
                  />
                </div>
                <div className="sm:col-span-2">
                  <Select
                    label={i === 0 ? 'Estado' : undefined}
                    value={u.EstadoHaciendaId}
                    onChange={(e) => updateUbicacion(i, 'EstadoHaciendaId', e.target.value)}
                    options={estadoHaciendaOptions}
                    disabled={readOnly}
                  />
                </div>
                <div className="sm:col-span-3">
                  <Select
                    label={i === 0 ? 'Corral' : undefined}
                    value={u.AlmacenId}
                    onChange={(e) => updateUbicacion(i, 'AlmacenId', e.target.value)}
                    options={corralOptions(u.EstadoHaciendaId)}
                    placeholder="Seleccionar corral..."
                    disabled={readOnly}
                  />
                </div>
                <div className="sm:col-span-2">
                  <Input label={i === 0 ? 'Cantidad (UN)' : undefined} type="number" value={u.Cantidad} onChange={(e) => updateUbicacion(i, 'Cantidad', e.target.value)} disabled={readOnly} />
                </div>
                <div className="sm:col-span-1">
                  <Input label={i === 0 ? 'Prom. (kg)' : undefined} value={num(Math.round(pesoPromedio(u.TipoEspecieId))).toLocaleString('es-AR')} disabled />
                </div>
                {!readOnly && (
                  <div className="sm:col-span-1">
                    <Button type="button" size="sm" variant="ghost" onClick={() => removeUbicacion(i)}>✕</Button>
                  </div>
                )}
              </div>
            ))}
          </div>
        </div>

        {/* Acciones */}
        <div className="flex justify-end gap-3">
          <Button variant="secondary" type="button" onClick={() => navigate(backTo)}>
            {readOnly ? 'Volver' : 'Cancelar'}
          </Button>
          {!readOnly && (
            <>
              <Button type="submit" variant="secondary" loading={loading}>
                {isEdit ? 'Guardar cambios' : 'Guardar borrador'}
              </Button>
              <Button type="button" loading={loading} onClick={handleEnviarAprobacion}>
                Enviar a aprobacion
              </Button>
            </>
          )}
        </div>
      </form>
    </>
  )
}
