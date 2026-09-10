import { useState, useEffect } from 'react'
import type { FormEvent } from 'react'
import { useParams, useNavigate } from 'react-router'
import { getEje, createEje, updateEje, ETIQUETAS } from '@/services/ejesTipificacion.service'
import type { EjeTipificacionId } from '@/services/ejesTipificacion.service'
import { getEspecies } from '@/services/especies.service'
import { useToast } from '@/components/ui/Toast'
import type { Especie } from '@/types'
import Input from '@/components/ui/Input'
import Select from '@/components/ui/Select'
import Button from '@/components/ui/Button'
import PageHeader from '@/components/ui/PageHeader'
import Spinner from '@/components/ui/Spinner'

export default function EjeTipificacionFormPage({ eje }: { eje: EjeTipificacionId }) {
  const { codigo } = useParams()
  const navigate = useNavigate()
  const { toast } = useToast()
  const isEdit = !!codigo
  const etiqueta = ETIQUETAS[eje]

  const [loading, setLoading] = useState(false)
  const [fetching, setFetching] = useState(true)
  const [especies, setEspecies] = useState<Especie[]>([])
  const [form, setForm] = useState({
    Codigo: '',
    Nombre: '',
    EspecieId: '',
    Orden: '0',
    Activo: true,
  })
  const [errors, setErrors] = useState<Record<string, string>>({})

  useEffect(() => {
    const loadData = async () => {
      setFetching(true)
      try {
        const especiesRes = await getEspecies({ PageSize: 1000, Estado: true })
        setEspecies(especiesRes.data || [])

        if (isEdit && codigo) {
          const entity = await getEje(eje, codigo)
          setForm({
            Codigo: entity.codigo || '',
            Nombre: entity.nombre || '',
            EspecieId: entity.especieId || '',
            Orden: String(entity.orden ?? 0),
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
  }, [eje, codigo, isEdit, toast])

  const validate = (): boolean => {
    const e: Record<string, string> = {}
    if (!isEdit && !form.Codigo.trim()) e['Codigo'] = 'Requerido'
    if (!form.Nombre.trim()) e['Nombre'] = 'Requerido'
    if (!form.EspecieId) e['EspecieId'] = 'Requerido'
    setErrors(e)
    return Object.keys(e).length === 0
  }

  const handleSubmit = async (ev: FormEvent) => {
    ev.preventDefault()
    if (!validate()) return

    setLoading(true)
    try {
      if (isEdit && codigo) {
        await updateEje(eje, codigo, {
          Nombre: form.Nombre,
          EspecieId: form.EspecieId,
          Orden: Number(form.Orden || 0),
          Activo: form.Activo,
        })
        toast('success', etiqueta.singular + ' actualizada')
      } else {
        await createEje(eje, {
          Codigo: form.Codigo,
          Nombre: form.Nombre,
          EspecieId: form.EspecieId,
          Orden: Number(form.Orden || 0),
        })
        toast('success', etiqueta.singular + ' creada')
      }
      navigate('/' + eje)
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
      <PageHeader title={(isEdit ? 'Editar ' : 'Nueva ') + etiqueta.singular} />

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
            <Input
              label="Orden"
              type="number"
              value={form.Orden}
              onChange={(e) => updateField('Orden', e.target.value)}
            />
          </div>

          <p className="mt-4 text-sm text-text-light">
            El orden es la posicion en la escala, no un ranking de calidad. Se usa para listar, porque
            los codigos ordenados alfabeticamente no dicen nada. En conformacion la escala va de
            mejor a peor; en engrasamiento va de menos a mas grasa, y el optimo esta en el medio; en
            denticion va de menor a mayor edad; en contusion va de menor a mayor severidad, y el
            primero de la escala es el que el Tipificador propone por defecto.
          </p>

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
            <Button variant="secondary" type="button" onClick={() => navigate('/' + eje)}>
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
