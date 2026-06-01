import { useState, useEffect } from 'react'
import type { FormEvent } from 'react'
import { useParams, useNavigate } from 'react-router'
import { getEstablecimiento, createEstablecimiento, updateEstablecimiento } from '@/services/establecimientos.service'
import { getSucursales } from '@/services/sucursales.service'
import { getEspecies } from '@/services/especies.service'
import type { Especie } from '@/services/especies.service'
import { useToast } from '@/components/ui/Toast'
import type { Sucursal } from '@/types'
import Input from '@/components/ui/Input'
import Select from '@/components/ui/Select'
import Button from '@/components/ui/Button'
import PageHeader from '@/components/ui/PageHeader'
import Spinner from '@/components/ui/Spinner'

export default function EstablecimientoFormPage() {
  const { id } = useParams()
  const navigate = useNavigate()
  const { toast } = useToast()
  const isEdit = !!id

  const [loading, setLoading] = useState(false)
  const [fetching, setFetching] = useState(true)
  const [sucursales, setSucursales] = useState<Sucursal[]>([])
  const [especies, setEspecies] = useState<Especie[]>([])
  const [form, setForm] = useState({
    CodigoEstablecimiento: '',
    Nombre: '',
    SucursalId: '',
    EspecieId: '',
    NumeroSenasa: '',
    NumeroOncca: '',
    Activo: true,
  })
  const [errors, setErrors] = useState<Record<string, string>>({})

  useEffect(() => {
    const loadData = async () => {
      try {
        const [sucRes, espRes] = await Promise.all([
          getSucursales({ PageSize: 1000 }),
          getEspecies(),
        ])
        setSucursales((sucRes.data || []).filter((s) => s.activo))
        setEspecies(espRes)

        if (isEdit && id) {
          const entity = await getEstablecimiento(id)
          setForm({
            CodigoEstablecimiento: entity.codigoEstablecimiento || '',
            Nombre: entity.nombre || '',
            SucursalId: entity.sucursalId || '',
            EspecieId: entity.especieId || '',
            NumeroSenasa: entity.numeroSenasa || '',
            NumeroOncca: entity.numeroOncca || '',
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
    if (!form.CodigoEstablecimiento.trim()) newErrors['CodigoEstablecimiento'] = 'Requerido'
    if (!form.Nombre.trim()) newErrors['Nombre'] = 'Requerido'
    if (!form.SucursalId) newErrors['SucursalId'] = 'Requerido'
    setErrors(newErrors)
    return Object.keys(newErrors).length === 0
  }

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault()
    if (!validate()) return

    setLoading(true)
    try {
      if (isEdit && id) {
        await updateEstablecimiento(id, {
          Nombre: form.Nombre,
          SucursalId: form.SucursalId,
          EspecieId: form.EspecieId,
          NumeroSenasa: form.NumeroSenasa,
          NumeroOncca: form.NumeroOncca,
          Activo: form.Activo,
        })
        toast('success', 'Establecimiento actualizado')
      } else {
        await createEstablecimiento({
          CodigoEstablecimiento: form.CodigoEstablecimiento,
          Nombre: form.Nombre,
          SucursalId: form.SucursalId,
          EspecieId: form.EspecieId,
          NumeroSenasa: form.NumeroSenasa,
          NumeroOncca: form.NumeroOncca,
        })
        toast('success', 'Establecimiento creado')
      }
      navigate('/establecimientos')
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
      <PageHeader title={isEdit ? 'Editar Establecimiento' : 'Nuevo Establecimiento'} />

      <div className="mx-auto max-w-2xl">
        <form onSubmit={handleSubmit} className="rounded-lg border border-border bg-surface p-6 shadow-sm">
          <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
            <Input
              label="Codigo"
              value={form.CodigoEstablecimiento}
              onChange={(e) => updateField('CodigoEstablecimiento', e.target.value)}
              error={errors['CodigoEstablecimiento']}
              disabled={isEdit}
            />
            <Input
              label="Nombre"
              value={form.Nombre}
              onChange={(e) => updateField('Nombre', e.target.value)}
              error={errors['Nombre']}
            />
            <Select
              label="Sucursal"
              value={form.SucursalId}
              onChange={(e) => updateField('SucursalId', e.target.value)}
              options={sucursales.map((s) => ({
                value: s.id,
                label: `${s.codigoSucursal} - ${s.nombre}`,
              }))}
              placeholder="Seleccionar sucursal..."
              error={errors['SucursalId']}
            />
            <Select
              label="Especie"
              value={form.EspecieId}
              onChange={(e) => updateField('EspecieId', e.target.value)}
              options={especies.map((e) => ({
                value: e.codigo,
                label: e.nombre,
              }))}
              placeholder="Seleccionar especie..."
            />
            <Input
              label="Numero SENASA"
              value={form.NumeroSenasa}
              onChange={(e) => updateField('NumeroSenasa', e.target.value)}
            />
            <Input
              label="Numero ONCCA"
              value={form.NumeroOncca}
              onChange={(e) => updateField('NumeroOncca', e.target.value)}
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
            <Button variant="secondary" type="button" onClick={() => navigate('/establecimientos')}>
              Cancelar
            </Button>
            <Button type="submit" loading={loading}>
              {isEdit ? 'Guardar Cambios' : 'Crear Establecimiento'}
            </Button>
          </div>
        </form>
      </div>
    </>
  )
}
