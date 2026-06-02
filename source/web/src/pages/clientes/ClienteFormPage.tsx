import { useState, useEffect } from 'react'
import type { FormEvent } from 'react'
import { useParams, useNavigate } from 'react-router'
import { getCliente, createCliente, updateCliente } from '@/services/clientes.service'
import { getTiposClientes } from '@/services/tiposClientes.service'
import { useToast } from '@/components/ui/Toast'
import type { TipoCliente } from '@/types'
import Input from '@/components/ui/Input'
import Select from '@/components/ui/Select'
import Button from '@/components/ui/Button'
import PageHeader from '@/components/ui/PageHeader'
import Spinner from '@/components/ui/Spinner'

export default function ClienteFormPage() {
  const { id } = useParams()
  const navigate = useNavigate()
  const { toast } = useToast()
  const isEdit = !!id

  const [loading, setLoading] = useState(false)
  const [fetching, setFetching] = useState(true)
  const [tiposCliente, setTiposCliente] = useState<TipoCliente[]>([])
  const [form, setForm] = useState({
    CodigoCliente: '',
    Nombre: '',
    TipoClienteId: '',
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
        const tipos = await getTiposClientes()
        setTiposCliente(tipos)

        if (isEdit && id) {
          const entity = await getCliente(id)
          setForm({
            CodigoCliente: entity.codigoCliente || '',
            Nombre: entity.nombre || '',
            TipoClienteId: entity.tipoClienteId || '',
            NumeroCuit: entity.numeroCuit || '',
            NumeroIngresosBrutos: entity.numeroIngresosBrutos || '',
            NumeroInscripcionRuca: entity.numeroInscripcionRuca || '',
            CodigoActividad: entity.codigoActividad || '',
            ERP_Codigo: entity.erP_Codigo || '',
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
    if (!form.CodigoCliente.trim()) newErrors['CodigoCliente'] = 'Requerido'
    if (!form.Nombre.trim()) newErrors['Nombre'] = 'Requerido'
    if (!form.TipoClienteId) newErrors['TipoClienteId'] = 'Requerido'
    setErrors(newErrors)
    return Object.keys(newErrors).length === 0
  }

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault()
    if (!validate()) return

    setLoading(true)
    try {
      if (isEdit && id) {
        await updateCliente(id, {
          Nombre: form.Nombre,
          TipoClienteId: form.TipoClienteId,
          NumeroCuit: form.NumeroCuit,
          NumeroIngresosBrutos: form.NumeroIngresosBrutos,
          NumeroInscripcionRuca: form.NumeroInscripcionRuca,
          CodigoActividad: form.CodigoActividad,
          ERP_Codigo: form.ERP_Codigo,
          Activo: form.Activo,
        })
        toast('success', 'Cliente actualizado')
      } else {
        await createCliente({
          CodigoCliente: form.CodigoCliente,
          Nombre: form.Nombre,
          TipoClienteId: form.TipoClienteId,
          NumeroCuit: form.NumeroCuit,
          NumeroIngresosBrutos: form.NumeroIngresosBrutos,
          NumeroInscripcionRuca: form.NumeroInscripcionRuca,
          CodigoActividad: form.CodigoActividad,
          ERP_Codigo: form.ERP_Codigo,
        })
        toast('success', 'Cliente creado')
      }
      navigate('/clientes')
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
      <PageHeader title={isEdit ? 'Editar Cliente' : 'Nuevo Cliente'} />

      <div className="mx-auto max-w-2xl">
        <form onSubmit={handleSubmit} className="rounded-lg border border-border bg-surface p-6 shadow-sm">
          <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
            <Input
              label="Codigo"
              value={form.CodigoCliente}
              onChange={(e) => updateField('CodigoCliente', e.target.value)}
              error={errors['CodigoCliente']}
              disabled={isEdit}
            />
            <Input
              label="Nombre"
              value={form.Nombre}
              onChange={(e) => updateField('Nombre', e.target.value)}
              error={errors['Nombre']}
            />
            <Select
              label="Tipo de Cliente"
              value={form.TipoClienteId}
              onChange={(e) => updateField('TipoClienteId', e.target.value)}
              options={tiposCliente.map((t) => ({
                value: t.codigo,
                label: t.nombre,
              }))}
              placeholder="Seleccionar..."
              error={errors['TipoClienteId']}
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
            <Button variant="secondary" type="button" onClick={() => navigate('/clientes')}>
              Cancelar
            </Button>
            <Button type="submit" loading={loading}>
              {isEdit ? 'Guardar Cambios' : 'Crear Cliente'}
            </Button>
          </div>
        </form>
      </div>
    </>
  )
}
