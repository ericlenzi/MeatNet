import { useState, useEffect } from 'react'
import type { FormEvent } from 'react'
import { useParams, useNavigate } from 'react-router'
import {
  getDespieceMaterial,
  createDespieceMaterial,
  updateDespieceMaterial,
} from '@/services/despiecesMateriales.service'
import { getMateriales } from '@/services/materiales.service'
import type { Material } from '@/types'
import { useToast } from '@/components/ui/Toast'
import Input from '@/components/ui/Input'
import Select from '@/components/ui/Select'
import Button from '@/components/ui/Button'
import PageHeader from '@/components/ui/PageHeader'
import Spinner from '@/components/ui/Spinner'

const num = (v: string): number => Number(v) || 0

export default function DespieceMaterialFormPage() {
  const { id } = useParams()
  const navigate = useNavigate()
  const { toast } = useToast()
  const isEdit = !!id

  const [fetching, setFetching] = useState(true)
  const [loading, setLoading] = useState(false)
  const [materiales, setMateriales] = useState<Material[]>([])

  const [form, setForm] = useState({
    MaterialOrigenId: '',
    MaterialDestinoId: '',
    Cantidad: '1',
    Rendimiento: '1',
    Activo: true,
  })
  const [errors, setErrors] = useState<Record<string, string>>({})

  useEffect(() => {
    const load = async () => {
      try {
        const mats = await getMateriales({ Estado: true, PageSize: 1000 })
        setMateriales(mats.data || [])
        if (isEdit && id) {
          const d = await getDespieceMaterial(id)
          setForm({
            MaterialOrigenId: d.materialOrigenId ?? '',
            MaterialDestinoId: d.materialDestinoId ?? '',
            Cantidad: String(d.cantidad),
            Rendimiento: String(d.rendimiento),
            Activo: d.activo,
          })
        }
      } catch {
        toast('error', 'Error al cargar datos')
      } finally {
        setFetching(false)
      }
    }
    void load()
  }, [id, isEdit, toast])

  const updateField = (field: string, value: string | boolean) => {
    setForm((p) => ({ ...p, [field]: value }))
    setErrors((p) => ({ ...p, [field]: '' }))
  }

  const validate = (): boolean => {
    const e: Record<string, string> = {}
    if (!form.MaterialOrigenId) e['MaterialOrigenId'] = 'Requerido'
    if (!form.MaterialDestinoId) e['MaterialDestinoId'] = 'Requerido'
    if (form.MaterialOrigenId && form.MaterialOrigenId === form.MaterialDestinoId)
      e['MaterialDestinoId'] = 'Debe ser distinto al origen'
    if (num(form.Cantidad) < 1) e['Cantidad'] = 'Debe ser mayor o igual a 1'
    if (num(form.Rendimiento) <= 0 || num(form.Rendimiento) > 1)
      e['Rendimiento'] = 'Debe estar entre 0 y 1'
    setErrors(e)
    return Object.keys(e).length === 0
  }

  const handleSubmit = async (ev: FormEvent) => {
    ev.preventDefault()
    if (!validate()) return
    setLoading(true)
    try {
      const payload = {
        MaterialOrigenId: form.MaterialOrigenId,
        MaterialDestinoId: form.MaterialDestinoId,
        Cantidad: num(form.Cantidad),
        Rendimiento: num(form.Rendimiento),
      }
      if (isEdit && id) {
        await updateDespieceMaterial(id, { ...payload, Activo: form.Activo })
        toast('success', 'Despiece actualizado')
      } else {
        await createDespieceMaterial(payload)
        toast('success', 'Despiece creado')
      }
      navigate('/despieces-materiales')
    } catch (err) {
      toast('error', err instanceof Error ? err.message : 'Error al guardar')
    } finally {
      setLoading(false)
    }
  }

  const materialOptions = materiales.map((m) => ({
    value: m.id,
    label: `${m.codigoMaterial} - ${m.nombre}`,
  }))

  if (fetching) {
    return (
      <div className="flex items-center justify-center py-20">
        <Spinner size="lg" />
      </div>
    )
  }

  return (
    <>
      <PageHeader title={isEdit ? 'Editar Despiece' : 'Nuevo Despiece'} />

      <div className="mx-auto max-w-2xl">
        <form onSubmit={handleSubmit} className="rounded-lg border border-border bg-surface p-6 shadow-sm">
          <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
            <Select
              label="Material origen"
              value={form.MaterialOrigenId}
              onChange={(e) => updateField('MaterialOrigenId', e.target.value)}
              options={materialOptions}
              placeholder="Seleccionar material..."
              error={errors['MaterialOrigenId']}
            />
            <Select
              label="Material destino"
              value={form.MaterialDestinoId}
              onChange={(e) => updateField('MaterialDestinoId', e.target.value)}
              options={materialOptions}
              placeholder="Seleccionar material..."
              error={errors['MaterialDestinoId']}
            />
            <Input
              label="Cantidad"
              type="number"
              value={form.Cantidad}
              onChange={(e) => updateField('Cantidad', e.target.value)}
              error={errors['Cantidad']}
            />
            <Input
              label="Rendimiento (fraccion 0 a 1)"
              type="number"
              step="0.01"
              value={form.Rendimiento}
              onChange={(e) => updateField('Rendimiento', e.target.value)}
              error={errors['Rendimiento']}
            />
          </div>

          <p className="mt-2 text-xs text-text-light">
            El rendimiento es la fraccion del peso del origen que va a este destino (ej. 0,52). La
            suma de rendimientos por material origen no puede superar 1 (100%).
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
            <Button variant="secondary" type="button" onClick={() => navigate('/despieces-materiales')}>
              Cancelar
            </Button>
            <Button type="submit" loading={loading}>
              {isEdit ? 'Guardar Cambios' : 'Crear Despiece'}
            </Button>
          </div>
        </form>
      </div>
    </>
  )
}
