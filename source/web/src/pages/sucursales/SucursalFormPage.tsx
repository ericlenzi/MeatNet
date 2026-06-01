import { useState, useEffect } from 'react'
import type { FormEvent } from 'react'
import { useParams, useNavigate } from 'react-router'
import { getSucursal, createSucursal, updateSucursal } from '@/services/sucursales.service'
import { getEmpresas } from '@/services/empresas.service'
import { useToast } from '@/components/ui/Toast'
import { useAuth } from '@/contexts/AuthContext'
import type { Empresa } from '@/types'
import Input from '@/components/ui/Input'
import Select from '@/components/ui/Select'
import Button from '@/components/ui/Button'
import ColorPicker from '@/components/ui/ColorPicker'
import PageHeader from '@/components/ui/PageHeader'
import Spinner from '@/components/ui/Spinner'

export default function SucursalFormPage() {
  const { id } = useParams()
  const navigate = useNavigate()
  const { toast } = useToast()
  const { user } = useAuth()
  const isEdit = !!id

  const [loading, setLoading] = useState(false)
  const [fetching, setFetching] = useState(true)
  const [empresas, setEmpresas] = useState<Empresa[]>([])
  const [form, setForm] = useState({
    CodigoSucursal: '',
    Nombre: '',
    EmpresaId: '',
    Direccion: '',
    CodigoPostal: '',
    Localidad: '',
    Provincia: '',
    Zona: '',
    Pais: '',
    Erp_Codigo: '',
    Color: '#DAE4F0',
    Activa: true,
  })
  const [errors, setErrors] = useState<Record<string, string>>({})

  useEffect(() => {
    const loadData = async () => {
      try {
        const empResponse = await getEmpresas({ PageSize: 1000 })
        const empList = empResponse.data || []
        setEmpresas(empList)

        if (!isEdit && user?.codigoEmpresa) {
          const empresaActiva = empList.find((e) => e.codigoEmpresa === user.codigoEmpresa)
          if (empresaActiva) {
            setForm((prev) => ({ ...prev, EmpresaId: empresaActiva.id }))
          }
        }

        if (isEdit && id) {
          const sucursal = await getSucursal(id)
          setForm({
            CodigoSucursal: sucursal.codigoSucursal || '',
            Nombre: sucursal.nombre || '',
            EmpresaId: sucursal.empresaId || '',
            Direccion: sucursal.direccion || '',
            CodigoPostal: sucursal.codigoPostal || '',
            Localidad: sucursal.localidad || '',
            Provincia: sucursal.provincia || '',
            Zona: sucursal.zona || '',
            Pais: sucursal.pais || '',
            Erp_Codigo: sucursal.erp_Codigo || '',
            Color: sucursal.color || '',
            Activa: sucursal.activo,
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
    if (!form.CodigoSucursal.trim()) newErrors['CodigoSucursal'] = 'Requerido'
    if (!form.Nombre.trim()) newErrors['Nombre'] = 'Requerido'
    setErrors(newErrors)
    return Object.keys(newErrors).length === 0
  }

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault()
    if (!validate()) return

    setLoading(true)
    try {
      if (isEdit && id) {
        await updateSucursal(id, {
          Nombre: form.Nombre,
          EmpresaId: form.EmpresaId,
          Activa: form.Activa,
          Direccion: form.Direccion,
          CodigoPostal: form.CodigoPostal,
          Localidad: form.Localidad,
          Provincia: form.Provincia,
          Zona: form.Zona,
          Pais: form.Pais,
          Erp_Codigo: form.Erp_Codigo,
          Color: form.Color,
        })
        toast('success', 'Sucursal actualizada')
      } else {
        await createSucursal({
          NumeroSucursal: form.CodigoSucursal,
          Nombre: form.Nombre,
          EmpresaId: form.EmpresaId,
          Direccion: form.Direccion,
          CodigoPostal: form.CodigoPostal,
          Localidad: form.Localidad,
          Provincia: form.Provincia,
          Zona: form.Zona,
          Pais: form.Pais,
          Erp_Codigo: form.Erp_Codigo,
          Color: form.Color,
        })
        toast('success', 'Sucursal creada')
      }
      navigate('/sucursales')
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
      <PageHeader title={isEdit ? 'Editar Sucursal' : 'Nueva Sucursal'} />

      <div className="mx-auto max-w-2xl">
        <form onSubmit={handleSubmit} className="rounded-lg border border-border bg-surface p-6 shadow-sm">
          <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
            <Input
              label="Codigo Sucursal"
              value={form.CodigoSucursal}
              onChange={(e) => updateField('CodigoSucursal', e.target.value)}
              error={errors['CodigoSucursal']}
              disabled={isEdit}
            />
            <Input
              label="Nombre"
              value={form.Nombre}
              onChange={(e) => updateField('Nombre', e.target.value)}
              error={errors['Nombre']}
            />
            <Select
              label="Empresa"
              value={form.EmpresaId}
              onChange={(e) => updateField('EmpresaId', e.target.value)}
              options={empresas.map((emp) => ({
                value: emp.id,
                label: `${emp.codigoEmpresa} - ${emp.nombre}`,
              }))}
              placeholder="Seleccionar..."
            />
            <Input
              label="Codigo ERP"
              value={form.Erp_Codigo}
              onChange={(e) => updateField('Erp_Codigo', e.target.value)}
            />
            <Input
              label="Direccion"
              value={form.Direccion}
              onChange={(e) => updateField('Direccion', e.target.value)}
            />
            <Input
              label="Codigo Postal"
              value={form.CodigoPostal}
              onChange={(e) => updateField('CodigoPostal', e.target.value)}
            />
            <Input
              label="Localidad"
              value={form.Localidad}
              onChange={(e) => updateField('Localidad', e.target.value)}
            />
            <Input
              label="Provincia"
              value={form.Provincia}
              onChange={(e) => updateField('Provincia', e.target.value)}
            />
            <Input
              label="Zona"
              value={form.Zona}
              onChange={(e) => updateField('Zona', e.target.value)}
            />
            <Input
              label="Pais"
              value={form.Pais}
              onChange={(e) => updateField('Pais', e.target.value)}
            />
            <ColorPicker
              label="Color"
              value={form.Color}
              onChange={(color) => updateField('Color', color)}
            />
          </div>

          {isEdit && (
            <div className="mt-4">
              <label className="flex items-center gap-2 text-sm font-medium text-text">
                <input
                  type="checkbox"
                  checked={form.Activa}
                  onChange={(e) => updateField('Activa', e.target.checked)}
                  className="h-4 w-4 rounded border-border text-primary-600 focus:ring-primary-500"
                />
                Activa
              </label>
            </div>
          )}

          <div className="mt-6 flex justify-end gap-3">
            <Button variant="secondary" type="button" onClick={() => navigate('/sucursales')}>
              Cancelar
            </Button>
            <Button type="submit" loading={loading}>
              {isEdit ? 'Guardar Cambios' : 'Crear Sucursal'}
            </Button>
          </div>
        </form>
      </div>
    </>
  )
}
