import { useState, useEffect } from 'react'
import type { FormEvent } from 'react'
import { useParams, useNavigate } from 'react-router'
import {
  getRendimientoSubproducto,
  createRendimientoSubproducto,
  updateRendimientoSubproducto,
} from '@/services/rendimientosSubproductos.service'
import { getEspecies } from '@/services/especies.service'
import { getMateriales } from '@/services/materiales.service'
import type { Especie, Material } from '@/types'
import { useToast } from '@/components/ui/Toast'
import Input from '@/components/ui/Input'
import Select from '@/components/ui/Select'
import Button from '@/components/ui/Button'
import PageHeader from '@/components/ui/PageHeader'
import Spinner from '@/components/ui/Spinner'

/** Tipos de material que son subproducto: lo unico que se puede estimar por rendimiento. */
const TIPOS_SUBPRODUCTO = ['SUB_PROD', 'MENUD']

export default function RendimientoSubproductoFormPage() {
  const { id } = useParams()
  const navigate = useNavigate()
  const { toast } = useToast()
  const isEdit = !!id

  const [fetching, setFetching] = useState(true)
  const [loading, setLoading] = useState(false)
  const [especies, setEspecies] = useState<Especie[]>([])
  const [materiales, setMateriales] = useState<Material[]>([])

  const [form, setForm] = useState({
    EspecieId: '',
    MaterialId: '',
    Porcentaje: '',
    Activo: true,
  })
  const [errors, setErrors] = useState<Record<string, string>>({})

  useEffect(() => {
    const load = async () => {
      try {
        const [esp, mat] = await Promise.all([
          getEspecies({ Estado: true, PageSize: 1000 }),
          getMateriales({ Estado: true, PageSize: 1000 }),
        ])
        setEspecies(esp.data || [])
        // Solo subproductos: la carne se pesa en el romaneo, no se estima.
        setMateriales((mat.data || []).filter((m) => TIPOS_SUBPRODUCTO.includes(m.tipoMaterialId)))

        if (isEdit && id) {
          const r = await getRendimientoSubproducto(id)
          setForm({
            EspecieId: r.especieId ?? '',
            MaterialId: r.materialId ?? '',
            Porcentaje: String(r.porcentaje ?? ''),
            Activo: r.activo,
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
    if (!form.EspecieId) e['EspecieId'] = 'Requerido'
    if (!form.MaterialId) e['MaterialId'] = 'Requerido'
    const pct = Number(form.Porcentaje)
    if (!(pct > 0) || pct >= 100) e['Porcentaje'] = 'Mayor a 0 y menor a 100'
    setErrors(e)
    return Object.keys(e).length === 0
  }

  const handleSubmit = async (ev: FormEvent) => {
    ev.preventDefault()
    if (!validate()) return
    setLoading(true)
    try {
      const payload = {
        EspecieId: form.EspecieId,
        MaterialId: form.MaterialId,
        Porcentaje: Number(form.Porcentaje),
      }
      if (isEdit && id) {
        await updateRendimientoSubproducto(id, { ...payload, Activo: form.Activo })
        toast('success', 'Rendimiento actualizado')
      } else {
        await createRendimientoSubproducto(payload)
        toast('success', 'Rendimiento creado')
      }
      navigate('/rendimientos-subproductos')
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
      <PageHeader title={isEdit ? 'Editar Rendimiento' : 'Nuevo Rendimiento'} />

      <div className="mx-auto max-w-2xl">
        <form onSubmit={handleSubmit} className="rounded-lg border border-border bg-surface p-6 shadow-sm">
          <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
            <Select
              label="Especie"
              value={form.EspecieId}
              onChange={(e) => updateField('EspecieId', e.target.value)}
              options={especies.map((e) => ({ value: e.codigo, label: e.nombre }))}
              placeholder="Seleccionar especie..."
              error={errors['EspecieId']}
            />
            <Select
              label="Subproducto"
              value={form.MaterialId}
              onChange={(e) => updateField('MaterialId', e.target.value)}
              options={materiales.map((m) => ({
                value: m.id,
                label: `${m.codigoMaterial} - ${m.nombre}`,
              }))}
              placeholder="Seleccionar subproducto..."
              error={errors['MaterialId']}
            />
            <Input
              label="Porcentaje del peso de la res (%)"
              type="number"
              step="0.01"
              value={form.Porcentaje}
              onChange={(e) => updateField('Porcentaje', e.target.value)}
              error={errors['Porcentaje']}
            />
          </div>

          <p className="mt-2 text-xs text-text-light">
            La base es el <strong>peso de la res faenada</strong>, no el peso vivo: el vivo es el
            dato que a veces falta y el de faena está siempre y es medido. Los subproductos de una
            res suman una fracción de su peso, así que la suma de los rendimientos de la especie no
            puede pasar de 100%.
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
            <Button variant="secondary" type="button" onClick={() => navigate('/rendimientos-subproductos')}>
              Cancelar
            </Button>
            <Button type="submit" loading={loading}>
              {isEdit ? 'Guardar Cambios' : 'Crear Rendimiento'}
            </Button>
          </div>
        </form>
      </div>
    </>
  )
}
