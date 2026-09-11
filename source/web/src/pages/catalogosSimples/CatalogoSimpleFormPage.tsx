import { useState, useEffect } from 'react'
import type { FormEvent } from 'react'
import { useParams, useNavigate } from 'react-router'
import {
  getCatalogoItem,
  createCatalogoItem,
  updateCatalogoItem,
  ETIQUETAS,
} from '@/services/catalogosSimples.service'
import type { CatalogoSimpleId } from '@/services/catalogosSimples.service'
import { useToast } from '@/components/ui/Toast'
import Input from '@/components/ui/Input'
import Button from '@/components/ui/Button'
import PageHeader from '@/components/ui/PageHeader'
import Spinner from '@/components/ui/Spinner'

export default function CatalogoSimpleFormPage({ catalogo }: { catalogo: CatalogoSimpleId }) {
  const { codigo } = useParams()
  const navigate = useNavigate()
  const { toast } = useToast()
  const isEdit = !!codigo

  const [fetching, setFetching] = useState(isEdit)
  const [loading, setLoading] = useState(false)
  const [form, setForm] = useState({ Codigo: '', Nombre: '', Activo: true })
  const [errors, setErrors] = useState<Record<string, string>>({})

  const etiqueta = ETIQUETAS[catalogo]

  useEffect(() => {
    if (!isEdit || !codigo) return
    getCatalogoItem(catalogo, codigo)
      .then((item) =>
        setForm({ Codigo: item.codigo ?? '', Nombre: item.nombre ?? '', Activo: item.activo }),
      )
      .catch(() => toast('error', 'Error al cargar datos'))
      .finally(() => setFetching(false))
  }, [catalogo, codigo, isEdit, toast])

  const updateField = (field: string, value: string | boolean) => {
    setForm((p) => ({ ...p, [field]: value }))
    setErrors((p) => ({ ...p, [field]: '' }))
  }

  const validate = (): boolean => {
    const e: Record<string, string> = {}
    if (!isEdit && !form.Codigo.trim()) e['Codigo'] = 'Requerido'
    if (!form.Nombre.trim()) e['Nombre'] = 'Requerido'
    setErrors(e)
    return Object.keys(e).length === 0
  }

  const handleSubmit = async (ev: FormEvent) => {
    ev.preventDefault()
    if (!validate()) return
    setLoading(true)
    try {
      if (isEdit && codigo) {
        await updateCatalogoItem(catalogo, codigo, { Nombre: form.Nombre.trim(), Activo: form.Activo })
        toast('success', etiqueta.singular + ' actualizado')
      } else {
        await createCatalogoItem(catalogo, {
          Codigo: form.Codigo.trim(),
          Nombre: form.Nombre.trim(),
        })
        toast('success', etiqueta.singular + ' creado')
      }
      navigate('/' + catalogo)
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
      <PageHeader title={(isEdit ? 'Editar ' : 'Nuevo ') + etiqueta.singular} />

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
            <Button variant="secondary" type="button" onClick={() => navigate('/' + catalogo)}>
              Cancelar
            </Button>
            <Button type="submit" loading={loading}>
              {isEdit ? 'Guardar Cambios' : 'Crear'}
            </Button>
          </div>
        </form>
      </div>
    </>
  )
}
