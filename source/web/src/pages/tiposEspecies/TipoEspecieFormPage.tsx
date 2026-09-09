import { useState, useEffect } from 'react'
import type { FormEvent } from 'react'
import { useParams, useNavigate } from 'react-router'
import { getTipoEspecie, createTipoEspecie, updateTipoEspecie, getTiposSexos } from '@/services/tiposEspecies.service'
import { getEspecies } from '@/services/especies.service'
import { useToast } from '@/components/ui/Toast'
import type { Especie, TipoSexo } from '@/types'
import Input from '@/components/ui/Input'
import Select from '@/components/ui/Select'
import Button from '@/components/ui/Button'
import PageHeader from '@/components/ui/PageHeader'
import Spinner from '@/components/ui/Spinner'

export default function TipoEspecieFormPage() {
  const { codigo } = useParams()
  const navigate = useNavigate()
  const { toast } = useToast()
  const isEdit = !!codigo

  const [loading, setLoading] = useState(false)
  const [fetching, setFetching] = useState(true)
  const [especies, setEspecies] = useState<Especie[]>([])
  const [tiposSexos, setTiposSexos] = useState<TipoSexo[]>([])
  const [form, setForm] = useState({
    Codigo: '',
    Nombre: '',
    EspecieId: '',
    TipoSexoId: '',
    PesoTeoricoReferencia: '',
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

        if (isEdit && codigo) {
          const entity = await getTipoEspecie(codigo)
          setForm({
            Codigo: entity.codigo || '',
            Nombre: entity.nombre || '',
            EspecieId: entity.especieId || '',
            TipoSexoId: entity.tipoSexoId || '',
            PesoTeoricoReferencia:
              entity.pesoTeoricoReferencia != null ? String(entity.pesoTeoricoReferencia) : '',
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
  }, [codigo, isEdit, toast])

  const validate = (): boolean => {
    const newErrors: Record<string, string> = {}
    if (!form.Codigo.trim()) newErrors['Codigo'] = 'Requerido'
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
      if (isEdit && codigo) {
        await updateTipoEspecie(codigo, {
          Nombre: form.Nombre,
          EspecieId: form.EspecieId,
          TipoSexoId: form.TipoSexoId || undefined,
          PesoTeoricoReferencia: form.PesoTeoricoReferencia
            ? Number(form.PesoTeoricoReferencia)
            : undefined,
          Activo: form.Activo,
        })
        toast('success', 'Tipo de especie actualizado')
      } else {
        await createTipoEspecie({
          Codigo: form.Codigo,
          Nombre: form.Nombre,
          EspecieId: form.EspecieId,
          TipoSexoId: form.TipoSexoId || undefined,
          PesoTeoricoReferencia: form.PesoTeoricoReferencia
            ? Number(form.PesoTeoricoReferencia)
            : undefined,
        })
        toast('success', 'Tipo de especie creado')
      }
      navigate('/tipos-especies')
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
      <PageHeader title={isEdit ? 'Editar Tipo de Especie' : 'Nuevo Tipo de Especie'} />

      <div className="mx-auto max-w-2xl">
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
              label="Peso Teorico de Referencia"
              type="number"
              value={form.PesoTeoricoReferencia}
              onChange={(e) => updateField('PesoTeoricoReferencia', e.target.value)}
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

          <p className="mt-4 text-sm text-text-light">
            El peso de referencia es el sugerido del rubro: se propone cuando una empresa da de
            alta la categoria. El peso que usan los calculos es el que cada empresa configura en
            Categorias de la Empresa.
          </p>

          <div className="mt-6 flex justify-end gap-3">
            <Button variant="secondary" type="button" onClick={() => navigate('/tipos-especies')}>
              Cancelar
            </Button>
            <Button type="submit" loading={loading}>
              {isEdit ? 'Guardar Cambios' : 'Crear Tipo de Especie'}
            </Button>
          </div>
        </form>
      </div>
    </>
  )
}
