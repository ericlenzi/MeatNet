import { useState, useEffect } from 'react'
import type { FormEvent } from 'react'
import { useParams, useNavigate } from 'react-router'
import { getMaterial, createMaterial, updateMaterial, getTiposMateriales } from '@/services/materiales.service'
import { getUnidadesMedidas } from '@/services/tipificaciones.service'
import type { TipoMaterial, CatalogoFaenaOption } from '@/types'
import { useToast } from '@/components/ui/Toast'
import Input from '@/components/ui/Input'
import Select from '@/components/ui/Select'
import Button from '@/components/ui/Button'
import PageHeader from '@/components/ui/PageHeader'
import Spinner from '@/components/ui/Spinner'

export default function MaterialFormPage() {
  const { id } = useParams()
  const navigate = useNavigate()
  const { toast } = useToast()
  const isEdit = !!id

  const [fetching, setFetching] = useState(true)
  const [loading, setLoading] = useState(false)
  const [tipos, setTipos] = useState<TipoMaterial[]>([])
  const [unidadesMedidas, setUnidadesMedidas] = useState<CatalogoFaenaOption[]>([])

  const [form, setForm] = useState({
    CodigoMaterial: '',
    Nombre: '',
    TipoMaterialId: '',
    UnidadMedidaId: '',
    PesoTeorico: '',
    ERP_Codigo: '',
    Activo: true,
  })
  const [errors, setErrors] = useState<Record<string, string>>({})

  useEffect(() => {
    const load = async () => {
      try {
        const [tiposRes, umRes] = await Promise.all([getTiposMateriales(), getUnidadesMedidas()])
        setTipos(tiposRes)
        setUnidadesMedidas(umRes)
        if (isEdit && id) {
          const e = await getMaterial(id)
          setForm({
            CodigoMaterial: e.codigoMaterial ?? '',
            Nombre: e.nombre ?? '',
            TipoMaterialId: e.tipoMaterialId ?? '',
            UnidadMedidaId: e.unidadMedidaId ?? '',
            PesoTeorico: e.pesoTeorico ?? '',
            ERP_Codigo: e.erP_Codigo ?? '',
            Activo: e.activo,
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
    if (!isEdit && !form.CodigoMaterial.trim()) e['CodigoMaterial'] = 'Requerido'
    if (!form.Nombre.trim()) e['Nombre'] = 'Requerido'
    setErrors(e)
    return Object.keys(e).length === 0
  }

  const handleSubmit = async (ev: FormEvent) => {
    ev.preventDefault()
    if (!validate()) return
    setLoading(true)
    try {
      const payload = {
        Nombre: form.Nombre,
        TipoMaterialId: form.TipoMaterialId || undefined,
        UnidadMedidaId: form.UnidadMedidaId || undefined,
        PesoTeorico: form.PesoTeorico || undefined,
        ERP_Codigo: form.ERP_Codigo || undefined,
      }
      if (isEdit && id) {
        await updateMaterial(id, { ...payload, Activo: form.Activo })
        toast('success', 'Material actualizado')
      } else {
        await createMaterial({ ...payload, CodigoMaterial: form.CodigoMaterial.trim() })
        toast('success', 'Material creado')
      }
      navigate('/materiales')
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
      <PageHeader title={isEdit ? 'Editar Material' : 'Nuevo Material'} />

      <div className="mx-auto max-w-2xl">
        <form onSubmit={handleSubmit} className="rounded-lg border border-border bg-surface p-6 shadow-sm">
          <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
            <Input
              label="Codigo"
              value={form.CodigoMaterial}
              onChange={(e) => updateField('CodigoMaterial', e.target.value)}
              error={errors['CodigoMaterial']}
              disabled={isEdit}
            />
            <Input
              label="Nombre"
              value={form.Nombre}
              onChange={(e) => updateField('Nombre', e.target.value)}
              error={errors['Nombre']}
            />
            <Select
              label="Tipo de material"
              value={form.TipoMaterialId}
              onChange={(e) => updateField('TipoMaterialId', e.target.value)}
              options={tipos.map((t) => ({ value: t.codigo, label: t.nombre }))}
              placeholder="Seleccionar tipo..."
            />
            <Select
              label="Unidad de medida"
              value={form.UnidadMedidaId}
              onChange={(e) => updateField('UnidadMedidaId', e.target.value)}
              options={unidadesMedidas.map((u) => ({ value: u.codigo, label: u.nombre }))}
              placeholder="Seleccionar unidad..."
            />
            <Input
              label="Peso teorico"
              value={form.PesoTeorico}
              onChange={(e) => updateField('PesoTeorico', e.target.value)}
            />
            <Input
              label="Codigo ERP"
              value={form.ERP_Codigo}
              onChange={(e) => updateField('ERP_Codigo', e.target.value)}
            />
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
            <Button variant="secondary" type="button" onClick={() => navigate('/materiales')}>
              Cancelar
            </Button>
            <Button type="submit" loading={loading}>
              {isEdit ? 'Guardar Cambios' : 'Crear Material'}
            </Button>
          </div>
        </form>
      </div>
    </>
  )
}
