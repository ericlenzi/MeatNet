import { useState, useEffect } from 'react'
import type { FormEvent } from 'react'
import { useParams, useNavigate } from 'react-router'
import { getCategoria, createCategoria, updateCategoria, getTiposSexos } from '@/services/categorias.service'
import { getEspecies } from '@/services/especies.service'
import { useToast } from '@/components/ui/Toast'
import type { Especie, TipoSexo } from '@/types'
import Input from '@/components/ui/Input'
import Select from '@/components/ui/Select'
import Button from '@/components/ui/Button'
import PageHeader from '@/components/ui/PageHeader'
import Spinner from '@/components/ui/Spinner'

export default function CategoriaFormPage() {
  const { id } = useParams()
  const navigate = useNavigate()
  const { toast } = useToast()
  const isEdit = !!id

  const [loading, setLoading] = useState(false)
  const [fetching, setFetching] = useState(true)
  const [especies, setEspecies] = useState<Especie[]>([])
  const [tiposSexos, setTiposSexos] = useState<TipoSexo[]>([])
  const [form, setForm] = useState({
    Id: '',
    Nombre: '',
    EspecieId: '',
    TipoSexoId: '',
    CodigoMaterialDesde: '',
    CodigoMaterialHasta: '',
    Activo: true,
  })
  const [errors, setErrors] = useState<Record<string, string>>({})

  useEffect(() => {
    const loadData = async () => {
      try {
        const [especiesRes, sexosRes] = await Promise.all([
          getEspecies({ PageSize: 1000, Estado: true }),
          getTiposSexos(),
        ])
        setEspecies(especiesRes.data || [])
        setTiposSexos(sexosRes)

        if (isEdit && id) {
          const entity = await getCategoria(id)
          setForm({
            Id: entity.id || '',
            Nombre: entity.nombre || '',
            EspecieId: entity.especieId || '',
            TipoSexoId: entity.tipoSexoId || '',
            CodigoMaterialDesde: entity.codigoMaterialDesde || '',
            CodigoMaterialHasta: entity.codigoMaterialHasta || '',
            Activo: entity.activo,
          })
        }
      } catch {
        toast('error', 'Error al cargar datos')
      } finally {
        setFetching(false)
      }
    }
    void loadData()
  }, [id, isEdit, toast])

  const validate = (): boolean => {
    const newErrors: Record<string, string> = {}
    if (!form.Id.trim()) newErrors['Id'] = 'Requerido'
    if (!form.Nombre.trim()) newErrors['Nombre'] = 'Requerido'
    if (!form.EspecieId) newErrors['EspecieId'] = 'Requerido'
    setErrors(newErrors)
    return Object.keys(newErrors).length === 0
  }

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault()
    if (!validate()) return

    setLoading(true)
    try {
      if (isEdit && id) {
        await updateCategoria(id, {
          Nombre: form.Nombre,
          EspecieId: form.EspecieId,
          TipoSexoId: form.TipoSexoId || undefined,
          CodigoMaterialDesde: form.CodigoMaterialDesde || undefined,
          CodigoMaterialHasta: form.CodigoMaterialHasta || undefined,
          Activo: form.Activo,
        })
        toast('success', 'Categoria actualizada')
      } else {
        await createCategoria({
          Id: form.Id,
          Nombre: form.Nombre,
          EspecieId: form.EspecieId,
          TipoSexoId: form.TipoSexoId || undefined,
          CodigoMaterialDesde: form.CodigoMaterialDesde || undefined,
          CodigoMaterialHasta: form.CodigoMaterialHasta || undefined,
        })
        toast('success', 'Categoria creada')
      }
      navigate('/categorias')
    } catch (err) {
      toast('error', err instanceof Error ? err.message : 'Error al guardar')
    } finally {
      setLoading(false)
    }
  }

  const updateField = (field: string, value: string | boolean) => {
    setForm((prev) => ({ ...prev, [field]: value }))
    if (errors[field]) setErrors((prev) => ({ ...prev, [field]: '' }))
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
      <PageHeader title={isEdit ? 'Editar Categoria' : 'Nueva Categoria'} />

      <div className="mx-auto max-w-2xl">
        <form onSubmit={handleSubmit} className="rounded-lg border border-border bg-surface p-6 shadow-sm">
          <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
            <Input
              label="Codigo"
              value={form.Id}
              onChange={(e) => updateField('Id', e.target.value)}
              error={errors['Id']}
              disabled={isEdit}
            />
            <Input
              label="Nombre"
              value={form.Nombre}
              onChange={(e) => updateField('Nombre', e.target.value)}
              error={errors['Nombre']}
            />
            <Select
              label="Especie"
              value={form.EspecieId}
              onChange={(e) => updateField('EspecieId', e.target.value)}
              options={especies.map((e) => ({ value: e.codigo, label: e.nombre }))}
              placeholder="Seleccionar especie..."
              error={errors['EspecieId']}
            />
            <Select
              label="Tipo Sexo"
              value={form.TipoSexoId}
              onChange={(e) => updateField('TipoSexoId', e.target.value)}
              options={tiposSexos.map((t) => ({ value: t.codigo, label: t.nombre }))}
              placeholder="Seleccionar sexo..."
            />
            <Input
              label="Cod. Material Desde"
              value={form.CodigoMaterialDesde}
              onChange={(e) => updateField('CodigoMaterialDesde', e.target.value)}
            />
            <Input
              label="Cod. Material Hasta"
              value={form.CodigoMaterialHasta}
              onChange={(e) => updateField('CodigoMaterialHasta', e.target.value)}
            />
          </div>

          {isEdit && (
            <div className="mt-4">
              <label className="flex items-center gap-2 text-sm font-medium text-text">
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
            <Button variant="secondary" type="button" onClick={() => navigate('/categorias')}>
              Cancelar
            </Button>
            <Button type="submit" loading={loading}>
              {isEdit ? 'Guardar Cambios' : 'Crear Categoria'}
            </Button>
          </div>
        </form>
      </div>
    </>
  )
}
