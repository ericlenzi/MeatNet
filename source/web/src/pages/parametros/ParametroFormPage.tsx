import { useState, useEffect } from 'react'
import type { FormEvent } from 'react'
import { useParams, useNavigate } from 'react-router'
import { getParametro, createParametro, updateParametro } from '@/services/parametros.service'
import { useToast } from '@/components/ui/Toast'
import Input from '@/components/ui/Input'
import Button from '@/components/ui/Button'
import PageHeader from '@/components/ui/PageHeader'
import Spinner from '@/components/ui/Spinner'

export default function ParametroFormPage() {
  const { codigo } = useParams()
  const navigate = useNavigate()
  const { toast } = useToast()
  const isEdit = !!codigo

  const [loading, setLoading] = useState(false)
  const [fetching, setFetching] = useState(true)
  const [form, setForm] = useState({
    Codigo: '',
    Nombre: '',
    Valor: '',
    Activo: true,
  })
  const [errors, setErrors] = useState<Record<string, string>>({})

  useEffect(() => {
    const loadData = async () => {
      try {
        if (isEdit && codigo) {
          const entity = await getParametro(codigo)
          setForm({
            Codigo: entity.codigo || '',
            Nombre: entity.nombre || '',
            Valor: entity.valor || '',
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
    setErrors(newErrors)
    return Object.keys(newErrors).length === 0
  }

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault()
    if (!validate()) return

    setLoading(true)
    try {
      if (isEdit && codigo) {
        await updateParametro(codigo, {
          Nombre: form.Nombre,
          Valor: form.Valor,
          Activo: form.Activo,
        })
        toast('success', 'Parametro actualizado')
      } else {
        await createParametro({
          Codigo: form.Codigo,
          Nombre: form.Nombre,
          Valor: form.Valor,
        })
        toast('success', 'Parametro creado')
      }
      navigate('/parametros')
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
      <PageHeader title={isEdit ? 'Editar Parametro' : 'Nuevo Parametro'} />

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
            <div className="sm:col-span-2">
              <Input
                label="Valor"
                value={form.Valor}
                onChange={(e) => updateField('Valor', e.target.value)}
              />
            </div>
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
            <Button variant="secondary" type="button" onClick={() => navigate('/parametros')}>
              Cancelar
            </Button>
            <Button type="submit" loading={loading}>
              {isEdit ? 'Guardar Cambios' : 'Crear Parametro'}
            </Button>
          </div>
        </form>
      </div>
    </>
  )
}
