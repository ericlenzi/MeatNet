import { useState, useEffect } from 'react'
import type { FormEvent } from 'react'
import { useParams, useNavigate } from 'react-router'
import { getEmpresa, createEmpresa, updateEmpresa } from '@/services/empresas.service'
import { getTiposEmpresas } from '@/services/tiposEmpresas.service'
import type { TipoEmpresa } from '@/types'
import { useToast } from '@/components/ui/Toast'
import Input from '@/components/ui/Input'
import Select from '@/components/ui/Select'
import Button from '@/components/ui/Button'
import PageHeader from '@/components/ui/PageHeader'
import Spinner from '@/components/ui/Spinner'

export default function EmpresaFormPage() {
  const { id } = useParams()
  const navigate = useNavigate()
  const { toast } = useToast()
  const isEdit = !!id

  const [loading, setLoading] = useState(false)
  const [fetching, setFetching] = useState(true)
  const [tiposEmpresa, setTiposEmpresa] = useState<TipoEmpresa[]>([])
  const [form, setForm] = useState({
    CodigoEmpresa: '',
    Nombre: '',
    TipoEmpresaId: '',
    NumeroCuit: '',
    NumeroIngresosBrutos: '',
    NumeroInscripcionRuca: '',
    CodigoActividad: '',
    ERP_Codigo: '',
    Activo: true,
  })
  const [errors, setErrors] = useState<Record<string, string>>({})

  useEffect(() => {
    const loadData = async () => {
      try {
        const tipos = await getTiposEmpresas()
        setTiposEmpresa(tipos)
        if (isEdit && id) {
          const data = await getEmpresa(id)
          setForm({
            CodigoEmpresa: data.codigoEmpresa || '',
            Nombre: data.nombre || '',
            TipoEmpresaId: data.tipoEmpresaId || '',
            NumeroCuit: data.numeroCuit || '',
            NumeroIngresosBrutos: data.numeroIngresosBrutos || '',
            NumeroInscripcionRuca: data.numeroInscripcionRuca || '',
            CodigoActividad: data.codigoActividad || '',
            ERP_Codigo: data.erP_Codigo || '',
            Activo: data.activo,
          })
        }
      } catch {
        toast('error', 'Error al cargar los datos')
      } finally {
        setFetching(false)
      }
    }
    loadData()
  }, [id, isEdit, toast])

  const validate = (): boolean => {
    const newErrors: Record<string, string> = {}
    if (!form.CodigoEmpresa.trim()) newErrors['CodigoEmpresa'] = 'Requerido'
    if (!form.Nombre.trim()) newErrors['Nombre'] = 'Requerido'
    if (!form.TipoEmpresaId) newErrors['TipoEmpresaId'] = 'Requerido'
    setErrors(newErrors)
    return Object.keys(newErrors).length === 0
  }

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault()
    if (!validate()) return

    setLoading(true)
    try {
      if (isEdit && id) {
        await updateEmpresa(id, form)
        toast('success', 'Empresa actualizada')
      } else {
        await createEmpresa(form)
        toast('success', 'Empresa creada')
      }
      navigate('/empresas')
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
      <PageHeader title={isEdit ? 'Editar Empresa' : 'Nueva Empresa'} />

      <div className="mx-auto max-w-2xl">
        <form onSubmit={handleSubmit} className="rounded-lg border border-border bg-surface p-6 shadow-sm">
          <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
            <Input
              label="Codigo"
              value={form.CodigoEmpresa}
              onChange={(e) => updateField('CodigoEmpresa', e.target.value)}
              error={errors['CodigoEmpresa']}
              disabled={isEdit}
            />
            <Input
              label="Nombre"
              value={form.Nombre}
              onChange={(e) => updateField('Nombre', e.target.value)}
              error={errors['Nombre']}
            />
            <Select
              label="Tipo Empresa"
              value={form.TipoEmpresaId}
              onChange={(e) => updateField('TipoEmpresaId', e.target.value)}
              options={tiposEmpresa.map((t) => ({ value: t.codigo, label: t.nombre }))}
              placeholder="Seleccionar..."
              error={errors['TipoEmpresaId']}
            />
            <Input
              label="CUIT"
              value={form.NumeroCuit}
              onChange={(e) => updateField('NumeroCuit', e.target.value)}
            />
            <Input
              label="Ingresos Brutos"
              value={form.NumeroIngresosBrutos}
              onChange={(e) => updateField('NumeroIngresosBrutos', e.target.value)}
            />
            <Input
              label="Inscripcion RUCA"
              value={form.NumeroInscripcionRuca}
              onChange={(e) => updateField('NumeroInscripcionRuca', e.target.value)}
            />
            <Input
              label="Codigo Actividad"
              value={form.CodigoActividad}
              onChange={(e) => updateField('CodigoActividad', e.target.value)}
            />
            <Input
              label="Codigo ERP"
              value={form.ERP_Codigo}
              onChange={(e) => updateField('ERP_Codigo', e.target.value)}
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
            <Button variant="secondary" type="button" onClick={() => navigate('/empresas')}>
              Cancelar
            </Button>
            <Button type="submit" loading={loading}>
              {isEdit ? 'Guardar Cambios' : 'Crear Empresa'}
            </Button>
          </div>
        </form>
      </div>
    </>
  )
}
