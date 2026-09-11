import { useState, useEffect, useMemo } from 'react'
import type { FormEvent } from 'react'
import { useParams, useNavigate } from 'react-router'
import { getPuesto, createPuesto, updatePuesto } from '@/services/puestos.service'
import { getEstablecimientos } from '@/services/establecimientos.service'
import { getCatalogoOptions } from '@/services/catalogosSimples.service'
import type { CatalogoSimple, Establecimiento } from '@/types'
import { useToast } from '@/components/ui/Toast'
import Input from '@/components/ui/Input'
import Select from '@/components/ui/Select'
import Button from '@/components/ui/Button'
import PageHeader from '@/components/ui/PageHeader'
import Spinner from '@/components/ui/Spinner'

export default function PuestoFormPage() {
  const { id } = useParams()
  const navigate = useNavigate()
  const { toast } = useToast()
  const isEdit = !!id

  const [fetching, setFetching] = useState(true)
  const [loading, setLoading] = useState(false)
  const [establecimientos, setEstablecimientos] = useState<Establecimiento[]>([])
  const [tiposPuestos, setTiposPuestos] = useState<CatalogoSimple[]>([])
  const [tiposMediciones, setTiposMediciones] = useState<CatalogoSimple[]>([])

  const [form, setForm] = useState({
    CodigoPuesto: '',
    Nombre: '',
    EstablecimientoId: '',
    EspecieId: '',
    TipoPuestoId: '',
    TipoMedicionId: '',
    Activo: true,
  })
  const [errors, setErrors] = useState<Record<string, string>>({})

  useEffect(() => {
    const load = async () => {
      try {
        const [est, tp, tm] = await Promise.all([
          getEstablecimientos({ Estado: true, PageSize: 1000 }),
          getCatalogoOptions('tipos-puestos'),
          getCatalogoOptions('tipos-mediciones'),
        ])
        setEstablecimientos(est.data || [])
        setTiposPuestos(tp)
        setTiposMediciones(tm)

        if (isEdit && id) {
          const p = await getPuesto(id)
          setForm({
            CodigoPuesto: p.codigoPuesto ?? '',
            Nombre: p.nombre ?? '',
            EstablecimientoId: p.establecimientoId ?? '',
            EspecieId: p.especieId ?? '',
            TipoPuestoId: p.tipoPuestoId ?? '',
            TipoMedicionId: p.tipoMedicionId ?? '',
            Activo: p.activo,
          })
        } else {
          // Defaults del alta: la unica clase de puesto y el primer metodo de medicion.
          setForm((prev) => ({
            ...prev,
            TipoPuestoId: tp.length === 1 ? (tp[0]?.codigo ?? '') : prev.TipoPuestoId,
            TipoMedicionId: tm[0]?.codigo ?? prev.TipoMedicionId,
          }))
        }
      } catch {
        toast('error', 'Error al cargar datos')
      } finally {
        setFetching(false)
      }
    }
    void load()
  }, [id, isEdit, toast])

  // Las especies del combo son las del establecimiento elegido: el puesto se configura por
  // Establecimiento + Especie y la planta declara con cuales opera.
  const especies = useMemo(
    () => establecimientos.find((e) => e.id === form.EstablecimientoId)?.especies ?? [],
    [establecimientos, form.EstablecimientoId],
  )

  const updateField = (field: string, value: string | boolean) => {
    setForm((p) => {
      // Al cambiar de establecimiento la especie elegida puede no estar habilitada ahi.
      if (field === 'EstablecimientoId') return { ...p, EstablecimientoId: String(value), EspecieId: '' }
      return { ...p, [field]: value }
    })
    setErrors((p) => ({ ...p, [field]: '' }))
  }

  const validate = (): boolean => {
    const e: Record<string, string> = {}
    if (!isEdit && !form.CodigoPuesto.trim()) e['CodigoPuesto'] = 'Requerido'
    if (!form.Nombre.trim()) e['Nombre'] = 'Requerido'
    if (!form.EstablecimientoId) e['EstablecimientoId'] = 'Requerido'
    if (!form.EspecieId) e['EspecieId'] = 'Requerido'
    if (!form.TipoPuestoId) e['TipoPuestoId'] = 'Requerido'
    if (!form.TipoMedicionId) e['TipoMedicionId'] = 'Requerido'
    setErrors(e)
    return Object.keys(e).length === 0
  }

  const handleSubmit = async (ev: FormEvent) => {
    ev.preventDefault()
    if (!validate()) return
    setLoading(true)
    try {
      const payload = {
        Nombre: form.Nombre.trim(),
        EstablecimientoId: form.EstablecimientoId,
        EspecieId: form.EspecieId,
        TipoPuestoId: form.TipoPuestoId,
        TipoMedicionId: form.TipoMedicionId,
      }
      if (isEdit && id) {
        await updatePuesto(id, { ...payload, Activo: form.Activo })
        toast('success', 'Puesto actualizado')
      } else {
        await createPuesto({ ...payload, CodigoPuesto: form.CodigoPuesto.trim() })
        toast('success', 'Puesto creado')
      }
      navigate('/puestos')
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
      <PageHeader title={isEdit ? 'Editar Puesto' : 'Nuevo Puesto'} />

      <div className="mx-auto max-w-2xl">
        <form onSubmit={handleSubmit} className="rounded-lg border border-border bg-surface p-6 shadow-sm">
          <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
            <Input
              label="Codigo"
              value={form.CodigoPuesto}
              onChange={(e) => updateField('CodigoPuesto', e.target.value)}
              error={errors['CodigoPuesto']}
              disabled={isEdit}
            />
            <Input
              label="Nombre"
              value={form.Nombre}
              onChange={(e) => updateField('Nombre', e.target.value)}
              error={errors['Nombre']}
            />
            <Select
              label="Establecimiento"
              value={form.EstablecimientoId}
              onChange={(e) => updateField('EstablecimientoId', e.target.value)}
              options={establecimientos.map((e) => ({ value: e.id, label: e.nombre }))}
              placeholder="Seleccionar establecimiento..."
              error={errors['EstablecimientoId']}
            />
            <Select
              label="Especie"
              value={form.EspecieId}
              onChange={(e) => updateField('EspecieId', e.target.value)}
              options={especies.map((e) => ({ value: e.id, label: e.nombre }))}
              placeholder={form.EstablecimientoId ? 'Seleccionar especie...' : 'Elija el establecimiento'}
              error={errors['EspecieId']}
              disabled={!form.EstablecimientoId}
            />
            <Select
              label="Tipo de puesto"
              value={form.TipoPuestoId}
              onChange={(e) => updateField('TipoPuestoId', e.target.value)}
              options={tiposPuestos.map((t) => ({ value: t.codigo, label: t.nombre }))}
              placeholder="Seleccionar tipo..."
              error={errors['TipoPuestoId']}
            />
            <Select
              label="Medicion por defecto"
              value={form.TipoMedicionId}
              onChange={(e) => updateField('TipoMedicionId', e.target.value)}
              options={tiposMediciones.map((t) => ({ value: t.codigo, label: t.nombre }))}
              placeholder="Seleccionar metodo..."
              error={errors['TipoMedicionId']}
            />
          </div>

          <p className="mt-2 text-xs text-text-light">
            El metodo de medicion es el que el Tipificador propone en la cabecera del romaneo. El
            operario puede cambiarlo si ese dia el palco pesa de otra forma.
          </p>

          {isEdit && (
            <div className="mt-4">
              <label className="flex items-center gap-2 text-sm text-text">
                <input
                  type="checkbox"
                  checked={form.Activo}
                  onChange={(e) => updateField('Activo', e.target.checked)}
                  className="h-4 w-4 rounded border-border text-primary-600 focus:ring-primary-500"
                />
                Activo
              </label>
            </div>
          )}

          <div className="mt-6 flex justify-end gap-3">
            <Button variant="secondary" type="button" onClick={() => navigate('/puestos')}>
              Cancelar
            </Button>
            <Button type="submit" loading={loading}>
              {isEdit ? 'Guardar Cambios' : 'Crear Puesto'}
            </Button>
          </div>
        </form>
      </div>
    </>
  )
}
