import { useState, useEffect, useCallback } from 'react'
import type { FormEvent } from 'react'
import { useParams, useNavigate } from 'react-router'
import { getUsuario, createUsuario, updateUsuario, getUsuarioSucursales, addUsuarioSucursal, setMainUsuarioSucursal, removeUsuarioSucursal } from '@/services/usuarios.service'
import type { UsuarioSucursalItem } from '@/services/usuarios.service'
import { getRoles } from '@/services/roles.service'
import { getEmpresas } from '@/services/empresas.service'
import { getSucursales } from '@/services/sucursales.service'
import { useToast } from '@/components/ui/Toast'
import { useAuth } from '@/contexts/AuthContext'
import type { Rol, Empresa, Sucursal } from '@/types'
import Input from '@/components/ui/Input'
import Select from '@/components/ui/Select'
import Button from '@/components/ui/Button'
import Badge from '@/components/ui/Badge'
import PageHeader from '@/components/ui/PageHeader'
import Spinner from '@/components/ui/Spinner'

export default function UsuarioFormPage() {
  const { id } = useParams()
  const navigate = useNavigate()
  const { toast } = useToast()
  const { user } = useAuth()
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

  // Sucursales del usuario
  const [usuarioSucursales, setUsuarioSucursales] = useState<UsuarioSucursalItem[]>([])
  const [allSucursales, setAllSucursales] = useState<Sucursal[]>([])
  const [selectedSucursalId, setSelectedSucursalId] = useState('')
  const [addingSucursal, setAddingSucursal] = useState(false)
  const [removingSucursalId, setRemovingSucursalId] = useState<string | null>(null)
  const [settingMainId, setSettingMainId] = useState<string | null>(null)
  // Track sucursales to add when creating (no userId yet)
  const [pendingSucursales, setPendingSucursales] = useState<{ sucursalId: string; nombre: string; codigoSucursal: string; esMain: boolean }[]>([])

  useEffect(() => {
    const loadData = async () => {
      try {
        const [rolesRes, empresasRes, sucursalesRes] = await Promise.all([
          getRoles(),
          getEmpresas({ PageSize: 1000 }),
          getSucursales({ PageSize: 1000 }),
        ])
        setRoles(rolesRes.data || [])
        const empList = (empresasRes.data || []).filter((e) => e.tipoEmpresaId === 'PRP')
        setEmpresas(empList)
        setAllSucursales(sucursalesRes.data || [])

        const empresaActiva = user?.codigoEmpresa
          ? empList.find((e) => e.codigoEmpresa === user.codigoEmpresa)
          : undefined

        if (!isEdit && empresaActiva) {
          setForm((prev) => ({ ...prev, EmpresaId: empresaActiva.id }))
        }

        if (isEdit && id) {
          const [usuario, userSucs] = await Promise.all([
            getUsuario(id),
            getUsuarioSucursales(id),
          ])
          setForm({
            UserName: usuario.userName || '',
            Nombre: usuario.nombre || '',
            Apellido: usuario.apellido || '',
            Email: usuario.email || '',
            Legajo: usuario.legajo || '',
            RolId: usuario.rolId || '',
            EmpresaId: empresaActiva?.id || usuario.empresaId || '',
            Activo: usuario.activo,
          })
          setUsuarioSucursales(userSucs)
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
    if (!isEdit && pendingSucursales.length === 0) newErrors['Sucursales'] = 'Debe asignar al menos una sucursal'
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
        const result = await createUsuario(form)
        // Add pending sucursales
        for (const ps of pendingSucursales) {
          await addUsuarioSucursal(result.id, ps.sucursalId, ps.esMain)
        }
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

  // Sucursales available to add (not already assigned)
  const assignedSucursalIds = isEdit
    ? usuarioSucursales.map((us) => us.sucursalId)
    : pendingSucursales.map((ps) => ps.sucursalId)

  const availableSucursales = allSucursales.filter(
    (s) => s.activo && !assignedSucursalIds.includes(s.id),
  )

  const handleAddSucursal = useCallback(async () => {
    if (!selectedSucursalId) return

    if (isEdit && id) {
      setAddingSucursal(true)
      try {
        await addUsuarioSucursal(id, selectedSucursalId, false)
        const updated = await getUsuarioSucursales(id)
        setUsuarioSucursales(updated)
        setSelectedSucursalId('')
        toast('success', 'Sucursal asignada')
      } catch (err) {
        toast('error', err instanceof Error ? err.message : 'Error al asignar')
      } finally {
        setAddingSucursal(false)
      }
    } else {
      const suc = allSucursales.find((s) => s.id === selectedSucursalId)
      if (suc) {
        setPendingSucursales((prev) => [
          ...prev,
          { sucursalId: suc.id, nombre: suc.nombre, codigoSucursal: suc.codigoSucursal, esMain: prev.length === 0 },
        ])
        setSelectedSucursalId('')
      }
    }
  }, [selectedSucursalId, isEdit, id, allSucursales, toast])

  const handleSetMain = useCallback(async (key: string, sucursalId: string) => {
    if (isEdit && id) {
      setSettingMainId(key)
      try {
        await setMainUsuarioSucursal(id, key)
        const updated = await getUsuarioSucursales(id)
        setUsuarioSucursales(updated)
      } catch (err) {
        toast('error', err instanceof Error ? err.message : 'Error al establecer principal')
      } finally {
        setSettingMainId(null)
      }
    } else {
      setPendingSucursales((prev) =>
        prev.map((ps) => ({ ...ps, esMain: ps.sucursalId === sucursalId })),
      )
    }
  }, [isEdit, id, toast])

  const handleRemoveSucursal = useCallback(async (key: string, sucursalId: string, esMain: boolean) => {
    if (isEdit && id) {
      setRemovingSucursalId(key)
      try {
        await removeUsuarioSucursal(id, key)
        const updated = await getUsuarioSucursales(id)
        setUsuarioSucursales(updated)
        toast('success', 'Sucursal removida')
      } catch (err) {
        toast('error', err instanceof Error ? err.message : 'Error al remover')
      } finally {
        setRemovingSucursalId(null)
      }
    } else {
      setPendingSucursales((prev) => {
        const remaining = prev.filter((ps) => ps.sucursalId !== sucursalId)
        if (esMain && remaining.length > 0 && !remaining.some((ps) => ps.esMain)) {
          remaining[0] = { ...remaining[0], esMain: true }
        }
        return remaining
      })
    }
  }, [isEdit, id, toast])

  const displaySucursales = isEdit
    ? usuarioSucursales.map((us) => ({ key: us.id, sucursalId: us.sucursalId, nombre: us.nombre, codigoSucursal: us.codigoSucursal, esMain: us.esMain }))
    : pendingSucursales.map((ps) => ({ key: ps.sucursalId, sucursalId: ps.sucursalId, nombre: ps.nombre, codigoSucursal: ps.codigoSucursal, esMain: ps.esMain }))

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

      <div className="mx-auto max-w-2xl space-y-6">
        {/* Datos del usuario */}
        <form onSubmit={handleSubmit} className="rounded-lg border border-border bg-surface p-6 shadow-sm">
          <h2 className="mb-4 text-lg font-semibold text-text">Datos del Usuario</h2>
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
              disabled
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

        {/* Sucursales del usuario */}
        <div className={`rounded-lg border bg-surface p-6 shadow-sm ${errors['Sucursales'] ? 'border-danger' : 'border-border'}`}>
          <h2 className="mb-4 text-lg font-semibold text-text">Sucursales Asignadas</h2>
          {errors['Sucursales'] && (
            <div className="mb-4 rounded-lg bg-red-50 px-4 py-3 text-sm text-danger">
              {errors['Sucursales']}
            </div>
          )}

          {/* Agregar sucursal */}
          <div className="mb-4 flex items-end gap-3">
            <div className="flex-1">
              <Select
                label="Agregar sucursal"
                value={selectedSucursalId}
                onChange={(e) => setSelectedSucursalId(e.target.value)}
                options={availableSucursales.map((s) => ({
                  value: s.id,
                  label: `${s.codigoSucursal} - ${s.nombre}`,
                }))}
                placeholder="Seleccionar sucursal..."
              />
            </div>
            <Button
              type="button"
              size="md"
              onClick={handleAddSucursal}
              disabled={!selectedSucursalId}
              loading={addingSucursal}
            >
              Agregar
            </Button>
          </div>

          {/* Lista de sucursales asignadas */}
          {displaySucursales.length === 0 ? (
            <p className="py-4 text-center text-sm text-text-light">
              No tiene sucursales asignadas
            </p>
          ) : (
            <div className="overflow-hidden rounded-lg border border-border">
              <table className="w-full text-sm">
                <thead>
                  <tr className="border-b border-border bg-gray-50">
                    <th className="px-4 py-2 text-left text-xs font-semibold uppercase tracking-wider text-text-light">Codigo</th>
                    <th className="px-4 py-2 text-left text-xs font-semibold uppercase tracking-wider text-text-light">Nombre</th>
                    <th className="px-4 py-2 text-left text-xs font-semibold uppercase tracking-wider text-text-light">Principal</th>
                    <th className="px-4 py-2 text-right text-xs font-semibold uppercase tracking-wider text-text-light">Accion</th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-border">
                  {displaySucursales.map((item) => (
                    <tr key={item.key} className="hover:bg-gray-50">
                      <td className="px-4 py-2 text-text">{item.codigoSucursal}</td>
                      <td className="px-4 py-2 text-text">{item.nombre}</td>
                      <td className="px-4 py-2">
                        {item.esMain
                          ? <Badge variant="info">Principal</Badge>
                          : displaySucursales.length > 1 && (
                            <button
                              type="button"
                              onClick={() => handleSetMain(item.key, item.sucursalId)}
                              disabled={settingMainId === item.key}
                              className="text-xs text-text-light hover:text-primary-600 transition-colors disabled:opacity-50"
                              title="Marcar como principal"
                            >
                              Marcar principal
                            </button>
                          )
                        }
                      </td>
                      <td className="px-4 py-2 text-right">
                        <button
                          type="button"
                          onClick={() => handleRemoveSucursal(item.key, item.sucursalId, item.esMain)}
                          disabled={removingSucursalId === item.key}
                          className="rounded p-1.5 text-text-light hover:bg-red-50 hover:text-danger transition-colors disabled:opacity-50"
                          title="Quitar"
                        >
                          <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                          </svg>
                        </button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>
      </div>
    </>
  )
}
