import { useState, useEffect } from 'react'
import type { FormEvent } from 'react'
import { useParams, useNavigate } from 'react-router'
import { getUsuario, createUsuario, updateUsuario } from '@/services/usuarios.service'
import { getRoles } from '@/services/roles.service'
import { getEmpresas } from '@/services/empresas.service'
import { useToast } from '@/components/ui/Toast'
import type { Rol, Empresa } from '@/types'
import Input from '@/components/ui/Input'
import Select from '@/components/ui/Select'
import Button from '@/components/ui/Button'
import PageHeader from '@/components/ui/PageHeader'
import Spinner from '@/components/ui/Spinner'

export default function UsuarioFormPage() {
  const { id } = useParams()
  const navigate = useNavigate()
  const { toast } = useToast()
  const isEdit = !!id

  const [loading, setLoading] = useState(false)
  const [fetching, setFetching] = useState(true)
  const [roles, setRoles] = useState<Rol[]>([])
  const [empresas, setEmpresas] = useState<Empresa[]>([])
  const [form, setForm] = useState({
    UserName: '',
    Nombre: '',
    Apellido: '',
    Email: '',
    Legajo: '',
    RolId: '',
    EmpresaId: '',
    Activo: true,
  })
  const [errors, setErrors] = useState<Record<string, string>>({})

  useEffect(() => {
    const loadData = async () => {
      try {
        const [rolesRes, empresasRes] = await Promise.all([
          getRoles(),
          getEmpresas({ PageSize: 1000 }),
        ])
        setRoles(rolesRes.data || [])
        setEmpresas(empresasRes.data || [])

        if (isEdit && id) {
          const usuario = await getUsuario(id)
          setForm({
            UserName: usuario.userName || '',
            Nombre: usuario.nombre || '',
            Apellido: usuario.apellido || '',
            Email: usuario.email || '',
            Legajo: usuario.legajo || '',
            RolId: usuario.rolId || '',
            EmpresaId: usuario.empresaId || '',
            Activo: usuario.activo,
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
    if (!form.UserName.trim()) newErrors['UserName'] = 'Requerido'
    if (!form.Nombre.trim()) newErrors['Nombre'] = 'Requerido'
    if (!form.Apellido.trim()) newErrors['Apellido'] = 'Requerido'
    if (!form.RolId) newErrors['RolId'] = 'Requerido'
    if (!form.EmpresaId) newErrors['EmpresaId'] = 'Requerido'
    setErrors(newErrors)
    return Object.keys(newErrors).length === 0
  }

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault()
    if (!validate()) return

    setLoading(true)
    try {
      if (isEdit && id) {
        await updateUsuario(id, form)
        toast('success', 'Usuario actualizado')
      } else {
        await createUsuario(form)
        toast('success', 'Usuario creado. Se le asigno una contraseña por defecto.')
      }
      navigate('/usuarios')
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
      <PageHeader title={isEdit ? 'Editar Usuario' : 'Nuevo Usuario'} />

      <div className="mx-auto max-w-2xl">
        <form onSubmit={handleSubmit} className="rounded-lg border border-border bg-surface p-6 shadow-sm">
          <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
            <Input
              label="Usuario"
              value={form.UserName}
              onChange={(e) => updateField('UserName', e.target.value)}
              error={errors['UserName']}
              disabled={isEdit}
            />
            <Input
              label="Nombre"
              value={form.Nombre}
              onChange={(e) => updateField('Nombre', e.target.value)}
              error={errors['Nombre']}
            />
            <Input
              label="Apellido"
              value={form.Apellido}
              onChange={(e) => updateField('Apellido', e.target.value)}
              error={errors['Apellido']}
            />
            <Input
              label="Email"
              type="email"
              value={form.Email}
              onChange={(e) => updateField('Email', e.target.value)}
            />
            <Input
              label="Legajo"
              value={form.Legajo}
              onChange={(e) => updateField('Legajo', e.target.value)}
            />
            <Select
              label="Rol"
              value={form.RolId}
              onChange={(e) => updateField('RolId', e.target.value)}
              options={roles.map((r) => ({ value: r.codigo, label: r.nombre }))}
              placeholder="Seleccionar rol..."
              error={errors['RolId']}
            />
            <Select
              label="Empresa"
              value={form.EmpresaId}
              onChange={(e) => updateField('EmpresaId', e.target.value)}
              options={empresas.map((emp) => ({
                value: emp.id,
                label: `${emp.codigoEmpresa} - ${emp.nombre}`,
              }))}
              placeholder="Seleccionar empresa..."
              error={errors['EmpresaId']}
            />
          </div>

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

          <div className="mt-6 flex justify-end gap-3">
            <Button variant="secondary" type="button" onClick={() => navigate('/usuarios')}>
              Cancelar
            </Button>
            <Button type="submit" loading={loading}>
              {isEdit ? 'Guardar Cambios' : 'Crear Usuario'}
            </Button>
          </div>
        </form>
      </div>
    </>
  )
}
