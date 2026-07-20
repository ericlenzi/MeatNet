import { useState, useEffect } from 'react'
import type { FormEvent } from 'react'
import { useParams, useNavigate } from 'react-router'
import {
  getNumerador,
  createNumerador,
  updateNumerador,
} from '@/services/numeradores.service'
import { getEspecies } from '@/services/especies.service'
import { useApp } from '@/contexts/AppContext'
import { useToast } from '@/components/ui/Toast'
import type { Especie } from '@/types'
import Input from '@/components/ui/Input'
import Select from '@/components/ui/Select'
import Button from '@/components/ui/Button'
import PageHeader from '@/components/ui/PageHeader'
import Spinner from '@/components/ui/Spinner'

const num = (v: string): number => Number(v) || 0

export default function NumeradorFormPage() {
  const { id } = useParams()
  const navigate = useNavigate()
  const { toast } = useToast()
  const { currentEstablecimiento } = useApp()
  const isEdit = !!id

  const [fetching, setFetching] = useState(true)
  const [loading, setLoading] = useState(false)
  const [especies, setEspecies] = useState<Especie[]>([])
  const [establecimientoNombre, setEstablecimientoNombre] = useState('')

  const [form, setForm] = useState({
    EspecieCodigo: '',
    Codigo: '',
    Descripcion: '',
    TipoNumerador: 'ROMANEO',
    UltimoNumero: '0',
    Activo: true,
  })
  const [errors, setErrors] = useState<Record<string, string>>({})

  useEffect(() => {
    const load = async () => {
      try {
        const esp = await getEspecies({ Estado: true, PageSize: 1000 })
        setEspecies(esp.data || [])
        if (isEdit && id) {
          const n = await getNumerador(id)
          setForm({
            EspecieCodigo: n.especieCodigo ?? '',
            Codigo: n.codigo ?? '',
            Descripcion: n.descripcion ?? '',
            TipoNumerador: n.tipoNumerador ?? '',
            UltimoNumero: String(n.ultimoNumero),
            Activo: n.activo,
          })
          setEstablecimientoNombre(n.establecimientoNombre)
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
    if (!isEdit && !form.EspecieCodigo) e['EspecieCodigo'] = 'Requerido'
    if (!form.Codigo.trim()) e['Codigo'] = 'Requerido'
    if (!form.TipoNumerador.trim()) e['TipoNumerador'] = 'Requerido'
    if (num(form.UltimoNumero) < 0) e['UltimoNumero'] = 'Debe ser mayor o igual a 0'
    setErrors(e)
    return Object.keys(e).length === 0
  }

  const handleSubmit = async (ev: FormEvent) => {
    ev.preventDefault()
    if (!validate()) return
    setLoading(true)
    try {
      if (isEdit && id) {
        await updateNumerador(id, {
          Descripcion: form.Descripcion,
          TipoNumerador: form.TipoNumerador,
          UltimoNumero: num(form.UltimoNumero),
          Activo: form.Activo,
        })
        toast('success', 'Numerador actualizado')
      } else {
        await createNumerador({
          EstablecimientoId: currentEstablecimiento?.id ?? '',
          EspecieCodigo: form.EspecieCodigo,
          Codigo: form.Codigo,
          Descripcion: form.Descripcion,
          TipoNumerador: form.TipoNumerador,
          UltimoNumero: num(form.UltimoNumero),
        })
        toast('success', 'Numerador creado')
      }
      navigate('/numeradores')
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
      <PageHeader title={isEdit ? 'Editar Numerador' : 'Nuevo Numerador'} />

      <div className="mx-auto max-w-2xl">
        <form onSubmit={handleSubmit} className="rounded-lg border border-border bg-surface p-6 shadow-sm">
          <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
            <Input label="Establecimiento" value={establecimientoNombre} disabled />
            {isEdit ? (
              <Input
                label="Especie"
                value={especies.find((e) => e.codigo === form.EspecieCodigo)?.nombre ?? form.EspecieCodigo}
                disabled
              />
            ) : (
              <Select
                label="Especie"
                value={form.EspecieCodigo}
                onChange={(e) => updateField('EspecieCodigo', e.target.value)}
                options={especies.map((e) => ({ value: e.codigo, label: e.nombre }))}
                placeholder="Seleccionar especie..."
                error={errors['EspecieCodigo']}
              />
            )}
            <Input
              label="Codigo"
              value={form.Codigo}
              onChange={(e) => updateField('Codigo', e.target.value)}
              error={errors['Codigo']}
              disabled={isEdit}
            />
            <Input
              label="Tipo de numerador"
              value={form.TipoNumerador}
              onChange={(e) => updateField('TipoNumerador', e.target.value)}
              error={errors['TipoNumerador']}
            />
            <Input
              label="Descripcion"
              value={form.Descripcion}
              onChange={(e) => updateField('Descripcion', e.target.value)}
            />
            <Input
              label="Ultimo numero"
              type="number"
              value={form.UltimoNumero}
              onChange={(e) => updateField('UltimoNumero', e.target.value)}
              error={errors['UltimoNumero']}
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
            <Button variant="secondary" type="button" onClick={() => navigate('/numeradores')}>
              Cancelar
            </Button>
            <Button type="submit" loading={loading}>
              {isEdit ? 'Guardar Cambios' : 'Crear Numerador'}
            </Button>
          </div>
        </form>
      </div>
    </>
  )
}
