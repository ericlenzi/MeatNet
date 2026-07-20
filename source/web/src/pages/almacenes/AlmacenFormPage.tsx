import { useState, useEffect } from 'react'
import type { FormEvent } from 'react'
import { useParams, useNavigate } from 'react-router'
import {
  getAlmacen,
  createAlmacen,
  updateAlmacen,
  getTiposAlmacenes,
} from '@/services/almacenes.service'
import type { TipoAlmacenOption } from '@/services/almacenes.service'
import { useApp } from '@/contexts/AppContext'
import { useToast } from '@/components/ui/Toast'
import Input from '@/components/ui/Input'
import Select from '@/components/ui/Select'
import Button from '@/components/ui/Button'
import PageHeader from '@/components/ui/PageHeader'
import Spinner from '@/components/ui/Spinner'

const num = (v: string): number => Number(v) || 0

export default function AlmacenFormPage() {
  const { id } = useParams()
  const navigate = useNavigate()
  const { toast } = useToast()
  const { currentEstablecimiento } = useApp()
  const isEdit = !!id

  const [fetching, setFetching] = useState(true)
  const [loading, setLoading] = useState(false)
  const [tipos, setTipos] = useState<TipoAlmacenOption[]>([])
  const [establecimientoNombre, setEstablecimientoNombre] = useState('')

  const [form, setForm] = useState({
    CodigoAlmacen: '',
    Nombre: '',
    Capacidad: '0',
    TipoAlmacenId: '',
    ERP_Codigo: '',
    Activo: true,
  })
  const [errors, setErrors] = useState<Record<string, string>>({})

  useEffect(() => {
    const load = async () => {
      try {
        const ts = await getTiposAlmacenes()
        setTipos(ts)
        if (isEdit && id) {
          const e = await getAlmacen(id)
          setForm({
            CodigoAlmacen: e.codigoAlmacen,
            Nombre: e.nombre,
            Capacidad: String(e.capacidad),
            TipoAlmacenId: e.tipoAlmacenId ?? '',
            ERP_Codigo: e.erP_Codigo ?? '',
            Activo: e.activo,
          })
          setEstablecimientoNombre(e.establecimientoNombre)
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
  }, [id, isEdit, currentEstablecimiento?.nombre, toast])

  const updateField = (field: string, value: string | boolean) => {
    setForm((p) => ({ ...p, [field]: value }))
    setErrors((p) => ({ ...p, [field]: '' }))
  }

  const validate = (): boolean => {
    const e: Record<string, string> = {}
    if (!form.CodigoAlmacen.trim()) e['CodigoAlmacen'] = 'Requerido'
    if (!form.Nombre.trim()) e['Nombre'] = 'Requerido'
    if (num(form.Capacidad) < 0) e['Capacidad'] = 'Debe ser mayor o igual a 0'
    setErrors(e)
    return Object.keys(e).length === 0
  }

  const handleSubmit = async (ev: FormEvent) => {
    ev.preventDefault()
    if (!validate()) return
    setLoading(true)
    try {
      if (isEdit && id) {
        await updateAlmacen(id, {
          Nombre: form.Nombre,
          Capacidad: num(form.Capacidad),
          TipoAlmacenId: form.TipoAlmacenId,
          ERP_Codigo: form.ERP_Codigo,
          Activo: form.Activo,
        })
        toast('success', 'Almacen actualizado')
      } else {
        await createAlmacen({
          CodigoAlmacen: form.CodigoAlmacen,
          Nombre: form.Nombre,
          Capacidad: num(form.Capacidad),
          TipoAlmacenId: form.TipoAlmacenId,
          ERP_Codigo: form.ERP_Codigo,
          EstablecimientoId: currentEstablecimiento?.id ?? '',
        })
        toast('success', 'Almacen creado')
      }
      navigate('/almacenes')
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
      <PageHeader title={isEdit ? 'Editar Almacen' : 'Nuevo Almacen'} />

      <div className="mx-auto max-w-2xl">
        <form onSubmit={handleSubmit} className="rounded-lg border border-border bg-surface p-6 shadow-sm">
          <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
            <Input
              label="Codigo"
              value={form.CodigoAlmacen}
              onChange={(e) => updateField('CodigoAlmacen', e.target.value)}
              error={errors['CodigoAlmacen']}
              disabled={isEdit}
            />
            <Input
              label="Nombre"
              value={form.Nombre}
              onChange={(e) => updateField('Nombre', e.target.value)}
              error={errors['Nombre']}
            />
            <Select
              label="Tipo de almacen"
              value={form.TipoAlmacenId}
              onChange={(e) => updateField('TipoAlmacenId', e.target.value)}
              options={tipos.map((t) => ({ value: t.codigo, label: t.nombre }))}
              placeholder="Seleccionar tipo..."
            />
            <Input
              label="Capacidad"
              type="number"
              value={form.Capacidad}
              onChange={(e) => updateField('Capacidad', e.target.value)}
              error={errors['Capacidad']}
            />
            <Input
              label="Codigo ERP"
              value={form.ERP_Codigo}
              onChange={(e) => updateField('ERP_Codigo', e.target.value)}
            />
            <Input label="Establecimiento" value={establecimientoNombre} disabled />
          </div>

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
            <Button variant="secondary" type="button" onClick={() => navigate('/almacenes')}>
              Cancelar
            </Button>
            <Button type="submit" loading={loading}>
              {isEdit ? 'Guardar Cambios' : 'Crear Almacen'}
            </Button>
          </div>
        </form>
      </div>
    </>
  )
}
