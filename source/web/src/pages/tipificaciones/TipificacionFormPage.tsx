import { useState, useEffect } from 'react'
import type { FormEvent } from 'react'
import { useParams, useNavigate } from 'react-router'
import {
  getTipificacion,
  createTipificacion,
  updateTipificacion,
  getDestinosComerciales,
  getTipificacionesOficiales,
  getUnidadesMedidas,
} from '@/services/tipificaciones.service'
import { getEspecies } from '@/services/especies.service'
import { getTiposEspecies } from '@/services/tiposEspecies.service'
import { getUnidadesFaenasOptions } from '@/services/unidadesFaenas.service'
import type {
  Especie,
  TipoEspecie,
  UnidadFaena,
  CatalogoFaenaOption,
  TipificacionOficialOption,
} from '@/types'
import { useToast } from '@/components/ui/Toast'
import Input from '@/components/ui/Input'
import Select from '@/components/ui/Select'
import Button from '@/components/ui/Button'
import PageHeader from '@/components/ui/PageHeader'
import Spinner from '@/components/ui/Spinner'

const num = (v: string): number => Number(v) || 0

export default function TipificacionFormPage() {
  const { codigo } = useParams()
  const navigate = useNavigate()
  const { toast } = useToast()
  const isEdit = !!codigo

  const [fetching, setFetching] = useState(true)
  const [loading, setLoading] = useState(false)
  const [especies, setEspecies] = useState<Especie[]>([])
  const [tiposEspecies, setTiposEspecies] = useState<TipoEspecie[]>([])
  const [unidadesFaenas, setUnidadesFaenas] = useState<UnidadFaena[]>([])
  const [destinos, setDestinos] = useState<CatalogoFaenaOption[]>([])
  const [oficiales, setOficiales] = useState<TipificacionOficialOption[]>([])
  const [unidadesMedidas, setUnidadesMedidas] = useState<CatalogoFaenaOption[]>([])
  const [puntos, setPuntos] = useState(0)

  const [form, setForm] = useState({
    Codigo: '',
    Descripcion: '',
    EspecieId: '',
    TipoEspecieId: '',
    UnidadFaenaId: '',
    DestinoComercialId: '',
    TipificacionOficialId: '',
    PesoDesde: '0',
    PesoHasta: '0',
    UnidadMedidaId: '',
    Activo: true,
  })
  const [errors, setErrors] = useState<Record<string, string>>({})

  useEffect(() => {
    const load = async () => {
      try {
        const [esp, dest, um] = await Promise.all([
          getEspecies({ Estado: true, PageSize: 1000 }),
          getDestinosComerciales(),
          getUnidadesMedidas(),
        ])
        setEspecies(esp.data || [])
        setDestinos(dest)
        setUnidadesMedidas(um)
        if (isEdit && codigo) {
          const t = await getTipificacion(codigo)
          setForm({
            Codigo: t.codigo ?? '',
            Descripcion: t.descripcion ?? '',
            EspecieId: t.especieId ?? '',
            TipoEspecieId: t.tipoEspecieId ?? '',
            UnidadFaenaId: t.unidadFaenaId ?? '',
            DestinoComercialId: t.destinoComercialId ?? '',
            TipificacionOficialId: t.tipificacionOficialId ?? '',
            PesoDesde: String(t.pesoDesde),
            PesoHasta: String(t.pesoHasta),
            UnidadMedidaId: t.unidadMedidaId ?? '',
            Activo: t.activo,
          })
          setPuntos(t.puntos)
        }
      } catch {
        toast('error', 'Error al cargar datos')
      } finally {
        setFetching(false)
      }
    }
    void load()
  }, [codigo, isEdit, toast])

  // Combos dependientes de la especie: categoria, unidad de faena y tipificacion oficial
  useEffect(() => {
    const loadDependent = async () => {
      if (!form.EspecieId) {
        setTiposEspecies([])
        setUnidadesFaenas([])
        setOficiales([])
        return
      }
      try {
        const [te, uf, of] = await Promise.all([
          getTiposEspecies({ Estado: true, EspecieId: form.EspecieId, PageSize: 1000 }),
          getUnidadesFaenasOptions(form.EspecieId),
          getTipificacionesOficiales(form.EspecieId),
        ])
        setTiposEspecies(te.data || [])
        setUnidadesFaenas(uf)
        setOficiales(of)
      } catch {
        setTiposEspecies([])
        setUnidadesFaenas([])
        setOficiales([])
      }
    }
    void loadDependent()
  }, [form.EspecieId])

  const updateField = (field: string, value: string | boolean) => {
    setForm((p) => ({ ...p, [field]: value }))
    setErrors((p) => ({ ...p, [field]: '' }))
  }

  // Al cambiar de especie se limpian los combos dependientes
  const handleEspecieChange = (value: string) => {
    setForm((p) => ({
      ...p,
      EspecieId: value,
      TipoEspecieId: '',
      UnidadFaenaId: '',
      TipificacionOficialId: '',
    }))
    setErrors((p) => ({ ...p, EspecieId: '' }))
  }

  const validate = (): boolean => {
    const e: Record<string, string> = {}
    if (!form.Codigo.trim()) e['Codigo'] = 'Requerido'
    if (!form.Descripcion.trim()) e['Descripcion'] = 'Requerido'
    if (!form.EspecieId) e['EspecieId'] = 'Requerido'
    if (!form.UnidadFaenaId) e['UnidadFaenaId'] = 'Requerido'
    if (num(form.PesoHasta) > 0 && num(form.PesoDesde) > num(form.PesoHasta))
      e['PesoDesde'] = 'No puede ser mayor al peso hasta'
    setErrors(e)
    return Object.keys(e).length === 0
  }

  const handleSubmit = async (ev: FormEvent) => {
    ev.preventDefault()
    if (!validate()) return
    setLoading(true)
    try {
      const payload = {
        Descripcion: form.Descripcion,
        EspecieId: form.EspecieId,
        TipoEspecieId: form.TipoEspecieId || undefined,
        UnidadFaenaId: form.UnidadFaenaId,
        DestinoComercialId: form.DestinoComercialId || undefined,
        TipificacionOficialId: form.TipificacionOficialId || undefined,
        PesoDesde: num(form.PesoDesde),
        PesoHasta: num(form.PesoHasta),
        UnidadMedidaId: form.UnidadMedidaId || undefined,
      }
      if (isEdit && codigo) {
        await updateTipificacion(codigo, { ...payload, Activo: form.Activo })
        toast('success', 'Tipificacion actualizada')
      } else {
        await createTipificacion({ Codigo: form.Codigo, ...payload })
        toast('success', 'Tipificacion creada')
      }
      navigate('/tipificaciones')
    } catch (err) {
      toast('error', err instanceof Error ? err.message : 'Error al guardar')
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

  return (
    <>
      <PageHeader title={isEdit ? 'Editar Tipificacion' : 'Nueva Tipificacion'} />

      <div className="mx-auto max-w-3xl">
        <form onSubmit={handleSubmit} className="rounded-lg border border-border bg-surface p-6 shadow-sm">
          <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
            <Input
              label="Codigo"
              value={form.Codigo}
              onChange={(e) => updateField('Codigo', e.target.value)}
              error={errors['Codigo']}
              disabled={isEdit}
            />
            <Input
              label="Descripcion"
              value={form.Descripcion}
              onChange={(e) => updateField('Descripcion', e.target.value)}
              error={errors['Descripcion']}
            />
            <Select
              label="Especie"
              value={form.EspecieId}
              onChange={(e) => handleEspecieChange(e.target.value)}
              options={especies.map((e) => ({ value: e.codigo, label: e.nombre }))}
              placeholder="Seleccionar especie..."
              error={errors['EspecieId']}
            />
            <Select
              label="Categoria (Tipo Especie)"
              value={form.TipoEspecieId}
              onChange={(e) => updateField('TipoEspecieId', e.target.value)}
              options={tiposEspecies.map((t) => ({ value: t.id, label: t.nombre }))}
              placeholder="(Ninguna)"
            />
            <Select
              label="Unidad de faena"
              value={form.UnidadFaenaId}
              onChange={(e) => updateField('UnidadFaenaId', e.target.value)}
              options={unidadesFaenas.map((u) => ({ value: u.id, label: u.nombre }))}
              placeholder="Seleccionar unidad..."
              error={errors['UnidadFaenaId']}
            />
            <Select
              label="Destino comercial"
              value={form.DestinoComercialId}
              onChange={(e) => updateField('DestinoComercialId', e.target.value)}
              options={destinos.map((d) => ({ value: d.codigo, label: d.nombre }))}
              placeholder="(Ninguno)"
            />
            <Select
              label="Tipificacion oficial"
              value={form.TipificacionOficialId}
              onChange={(e) => updateField('TipificacionOficialId', e.target.value)}
              options={oficiales.map((o) => ({ value: o.codigo, label: o.nombre }))}
              placeholder="(Ninguna)"
            />
            <Select
              label="Unidad de medida"
              value={form.UnidadMedidaId}
              onChange={(e) => updateField('UnidadMedidaId', e.target.value)}
              options={unidadesMedidas.map((u) => ({ value: u.codigo, label: u.nombre }))}
              placeholder="(Ninguna)"
            />
            <Input
              label="Peso desde"
              type="number"
              value={form.PesoDesde}
              onChange={(e) => updateField('PesoDesde', e.target.value)}
              error={errors['PesoDesde']}
            />
            <Input
              label="Peso hasta"
              type="number"
              value={form.PesoHasta}
              onChange={(e) => updateField('PesoHasta', e.target.value)}
            />
          </div>

          {isEdit && (
            <div className="mt-4 flex items-center gap-6">
              <label className="flex items-center gap-2 text-sm text-text">
                <input
                  type="checkbox"
                  checked={form.Activo}
                  onChange={(e) => updateField('Activo', e.target.checked)}
                  className="h-4 w-4 rounded border-border text-primary-600 focus:ring-primary-500"
                />
                Activo
              </label>
              <span className="text-sm text-text-light">Puntos: {puntos}</span>
            </div>
          )}

          <div className="mt-6 flex justify-end gap-3">
            <Button variant="secondary" type="button" onClick={() => navigate('/tipificaciones')}>
              Cancelar
            </Button>
            <Button type="submit" loading={loading}>
              {isEdit ? 'Guardar Cambios' : 'Crear Tipificacion'}
            </Button>
          </div>
        </form>
      </div>
    </>
  )
}
