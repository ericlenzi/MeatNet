import { useState, useEffect, useCallback } from 'react'
import type { FormEvent } from 'react'
import { useParams, useNavigate } from 'react-router'
import {
  getUsuario, createUsuario, updateUsuario,
  getUsuarioSucursales, addUsuarioSucursal, setMainUsuarioSucursal, removeUsuarioSucursal,
  getUsuarioEstablecimientos, addUsuarioEstablecimiento, setMainUsuarioEstablecimiento, removeUsuarioEstablecimiento,
} from '@/services/usuarios.service'
import type { UsuarioSucursalItem, UsuarioEstablecimientoItem } from '@/services/usuarios.service'
import { getRoles } from '@/services/roles.service'
import { getSucursales } from '@/services/sucursales.service'
import { getEstablecimientos } from '@/services/establecimientos.service'
import { useToast } from '@/components/ui/Toast'
import type { Rol, Sucursal, Establecimiento } from '@/types'
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
  const isEdit = !!id

  const [loading, setLoading] = useState(false)
  const [fetching, setFetching] = useState(true)
  const [roles, setRoles] = useState<Rol[]>([])
  const [form, setForm] = useState({
    UserName: '',
    Nombre: '',
    Apellido: '',
    Email: '',
    Legajo: '',
    RolId: '',
    Activo: true,
  })
  const [errors, setErrors] = useState<Record<string, string>>({})

  // Sucursales
  const [usuarioSucursales, setUsuarioSucursales] = useState<UsuarioSucursalItem[]>([])
  const [allSucursales, setAllSucursales] = useState<Sucursal[]>([])
  const [selectedSucursalId, setSelectedSucursalId] = useState('')
  const [addingSucursal, setAddingSucursal] = useState(false)
  const [removingSucursalId, setRemovingSucursalId] = useState<string | null>(null)
  const [settingMainSucursalId, setSettingMainSucursalId] = useState<string | null>(null)
  const [pendingSucursales, setPendingSucursales] = useState<{ sucursalId: string; nombre: string; codigoSucursal: string; esMain: boolean }[]>([])

  // Establecimientos
  const [usuarioEstablecimientos, setUsuarioEstablecimientos] = useState<UsuarioEstablecimientoItem[]>([])
  const [allEstablecimientos, setAllEstablecimientos] = useState<Establecimiento[]>([])
  const [selectedEstablecimientoId, setSelectedEstablecimientoId] = useState('')
  const [addingEstablecimiento, setAddingEstablecimiento] = useState(false)
  const [removingEstablecimientoId, setRemovingEstablecimientoId] = useState<string | null>(null)
  const [settingMainEstablecimientoId, setSettingMainEstablecimientoId] = useState<string | null>(null)
  const [pendingEstablecimientos, setPendingEstablecimientos] = useState<{ establecimientoId: string; nombre: string; codigoEstablecimiento: string; sucursalId: string; esMain: boolean }[]>([])

  useEffect(() => {
    const loadData = async () => {
      try {
        const [rolesRes, sucursalesRes, establecimientosRes] = await Promise.all([
          getRoles(),
          getSucursales({ PageSize: 1000 }),
          getEstablecimientos({ PageSize: 1000, Estado: true }),
        ])
        setRoles(rolesRes.data || [])
        setAllSucursales(sucursalesRes.data || [])
        setAllEstablecimientos(establecimientosRes.data || [])

        if (isEdit && id) {
          const [usuario, userSucs, userEsts] = await Promise.all([
            getUsuario(id),
            getUsuarioSucursales(id),
            getUsuarioEstablecimientos(id),
          ])
          setForm({
            UserName: usuario.userName || '',
            Nombre: usuario.nombre || '',
            Apellido: usuario.apellido || '',
            Email: usuario.email || '',
            Legajo: usuario.legajo || '',
            RolId: usuario.rolId || '',
            Activo: usuario.activo,
          })
          setUsuarioSucursales(userSucs)
          setUsuarioEstablecimientos(userEsts)
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
        for (const ps of pendingSucursales) {
          await addUsuarioSucursal(result.id, ps.sucursalId, ps.esMain)
        }
        for (const pe of pendingEstablecimientos) {
          await addUsuarioEstablecimiento(result.id, pe.establecimientoId, pe.esMain)
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

  // --- Sucursales ---

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

  const handleSetMainSucursal = useCallback(async (key: string, sucursalId: string) => {
    if (isEdit && id) {
      setSettingMainSucursalId(key)
      try {
        await setMainUsuarioSucursal(id, key)
        const updated = await getUsuarioSucursales(id)
        setUsuarioSucursales(updated)
      } catch (err) {
        toast('error', err instanceof Error ? err.message : 'Error al establecer principal')
      } finally {
        setSettingMainSucursalId(null)
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
        const [updatedSucs, updatedEsts] = await Promise.all([
          getUsuarioSucursales(id),
          getUsuarioEstablecimientos(id),
        ])
        setUsuarioSucursales(updatedSucs)
        setUsuarioEstablecimientos(updatedEsts)
        toast('success', 'Sucursal removida')
      } catch (err) {
        toast('error', err instanceof Error ? err.message : 'Error al remover')
      } finally {
        setRemovingSucursalId(null)
      }
    } else {
      const removedSucId = sucursalId
      setPendingSucursales((prev) => {
        const remaining = prev.filter((ps) => ps.sucursalId !== removedSucId)
        if (esMain && remaining.length > 0 && !remaining.some((ps) => ps.esMain)) {
          remaining[0] = { sucursalId: remaining[0]!.sucursalId, nombre: remaining[0]!.nombre, codigoSucursal: remaining[0]!.codigoSucursal, esMain: true }
        }
        return remaining
      })
      // Remove pending establecimientos of that sucursal
      setPendingEstablecimientos((prev) =>
        prev.filter((pe) => pe.sucursalId !== removedSucId),
      )
    }
  }, [isEdit, id, toast])

  const displaySucursales = isEdit
    ? usuarioSucursales.map((us) => ({ key: us.id, sucursalId: us.sucursalId, nombre: us.nombre, codigoSucursal: us.codigoSucursal, esMain: us.esMain }))
    : pendingSucursales.map((ps) => ({ key: ps.sucursalId, sucursalId: ps.sucursalId, nombre: ps.nombre, codigoSucursal: ps.codigoSucursal, esMain: ps.esMain }))

  // --- Establecimientos ---

  const activeSucursalIds = isEdit
    ? usuarioSucursales.map((us) => us.sucursalId)
    : pendingSucursales.map((ps) => ps.sucursalId)

  const assignedEstablecimientoIds = isEdit
    ? usuarioEstablecimientos.map((ue) => ue.establecimientoId)
    : pendingEstablecimientos.map((pe) => pe.establecimientoId)

  const availableEstablecimientos = allEstablecimientos.filter(
    (e) => e.activo && activeSucursalIds.includes(e.sucursalId) && !assignedEstablecimientoIds.includes(e.id),
  )

  const handleAddEstablecimiento = useCallback(async () => {
    if (!selectedEstablecimientoId) return

    if (isEdit && id) {
      setAddingEstablecimiento(true)
      try {
        await addUsuarioEstablecimiento(id, selectedEstablecimientoId, false)
        const updated = await getUsuarioEstablecimientos(id)
        setUsuarioEstablecimientos(updated)
        setSelectedEstablecimientoId('')
        toast('success', 'Establecimiento asignado')
      } catch (err) {
        toast('error', err instanceof Error ? err.message : 'Error al asignar')
      } finally {
        setAddingEstablecimiento(false)
      }
    } else {
      const est = allEstablecimientos.find((e) => e.id === selectedEstablecimientoId)
      if (est) {
        setPendingEstablecimientos((prev) => [
          ...prev,
          {
            establecimientoId: est.id,
            nombre: est.nombre,
            codigoEstablecimiento: est.codigoEstablecimiento,
            sucursalId: est.sucursalId,
            esMain: prev.length === 0,
          },
        ])
        setSelectedEstablecimientoId('')
      }
    }
  }, [selectedEstablecimientoId, isEdit, id, allEstablecimientos, toast])

  const handleSetMainEstablecimiento = useCallback(async (key: string, establecimientoId: string) => {
    if (isEdit && id) {
      setSettingMainEstablecimientoId(key)
      try {
        await setMainUsuarioEstablecimiento(id, key)
        const updated = await getUsuarioEstablecimientos(id)
        setUsuarioEstablecimientos(updated)
      } catch (err) {
        toast('error', err instanceof Error ? err.message : 'Error al establecer principal')
      } finally {
        setSettingMainEstablecimientoId(null)
      }
    } else {
      setPendingEstablecimientos((prev) =>
        prev.map((pe) => ({ ...pe, esMain: pe.establecimientoId === establecimientoId })),
      )
    }
  }, [isEdit, id, toast])

  const handleRemoveEstablecimiento = useCallback(async (key: string, establecimientoId: string, esMain: boolean) => {
    if (isEdit && id) {
      setRemovingEstablecimientoId(key)
      try {
        await removeUsuarioEstablecimiento(id, key)
        const updated = await getUsuarioEstablecimientos(id)
        setUsuarioEstablecimientos(updated)
        toast('success', 'Establecimiento removido')
      } catch (err) {
        toast('error', err instanceof Error ? err.message : 'Error al remover')
      } finally {
        setRemovingEstablecimientoId(null)
      }
    } else {
      setPendingEstablecimientos((prev) => {
        const remaining = prev.filter((pe) => pe.establecimientoId !== establecimientoId)
        if (esMain && remaining.length > 0 && !remaining.some((pe) => pe.esMain)) {
          const r = remaining[0]!
          remaining[0] = { establecimientoId: r.establecimientoId, nombre: r.nombre, codigoEstablecimiento: r.codigoEstablecimiento, sucursalId: r.sucursalId, esMain: true }
        }
        return remaining
      })
    }
  }, [isEdit, id, toast])

  const displayEstablecimientos = isEdit
    ? usuarioEstablecimientos.map((ue) => ({
        key: ue.id,
        establecimientoId: ue.establecimientoId,
        nombre: ue.nombre,
        codigoEstablecimiento: ue.codigoEstablecimiento,
        nombreSucursal: ue.nombreSucursal,
        esMain: ue.esMain,
      }))
    : pendingEstablecimientos.map((pe) => {
        const suc = allSucursales.find((s) => s.id === pe.sucursalId)
        return {
          key: pe.establecimientoId,
          establecimientoId: pe.establecimientoId,
          nombre: pe.nombre,
          codigoEstablecimiento: pe.codigoEstablecimiento,
          nombreSucursal: suc?.nombre || '',
          esMain: pe.esMain,
        }
      })

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
                              onClick={() => handleSetMainSucursal(item.key, item.sucursalId)}
                              disabled={settingMainSucursalId === item.key}
                              className="text-xs text-text-light hover:text-primary-600 transition-colors disabled:opacity-50"
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

        {/* Establecimientos del usuario */}
        <div className="rounded-lg border border-border bg-surface p-6 shadow-sm">
          <h2 className="mb-4 text-lg font-semibold text-text">Establecimientos Asignados</h2>

          <div className="mb-4 flex items-end gap-3">
            <div className="flex-1">
              <Select
                label="Agregar establecimiento"
                value={selectedEstablecimientoId}
                onChange={(e) => setSelectedEstablecimientoId(e.target.value)}
                options={availableEstablecimientos.map((e) => ({
                  value: e.id,
                  label: `${e.codigoEstablecimiento} - ${e.nombre}`,
                }))}
                placeholder={activeSucursalIds.length === 0 ? 'Asigne sucursales primero' : 'Seleccionar establecimiento...'}
                disabled={activeSucursalIds.length === 0}
              />
            </div>
            <Button
              type="button"
              size="md"
              onClick={handleAddEstablecimiento}
              disabled={!selectedEstablecimientoId}
              loading={addingEstablecimiento}
            >
              Agregar
            </Button>
          </div>

          {displayEstablecimientos.length === 0 ? (
            <p className="py-4 text-center text-sm text-text-light">
              No tiene establecimientos asignados
            </p>
          ) : (
            <div className="overflow-hidden rounded-lg border border-border">
              <table className="w-full text-sm">
                <thead>
                  <tr className="border-b border-border bg-gray-50">
                    <th className="px-4 py-2 text-left text-xs font-semibold uppercase tracking-wider text-text-light">Codigo</th>
                    <th className="px-4 py-2 text-left text-xs font-semibold uppercase tracking-wider text-text-light">Nombre</th>
                    <th className="px-4 py-2 text-left text-xs font-semibold uppercase tracking-wider text-text-light">Sucursal</th>
                    <th className="px-4 py-2 text-left text-xs font-semibold uppercase tracking-wider text-text-light">Principal</th>
                    <th className="px-4 py-2 text-right text-xs font-semibold uppercase tracking-wider text-text-light">Accion</th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-border">
                  {displayEstablecimientos.map((item) => (
                    <tr key={item.key} className="hover:bg-gray-50">
                      <td className="px-4 py-2 text-text">{item.codigoEstablecimiento}</td>
                      <td className="px-4 py-2 text-text">{item.nombre}</td>
                      <td className="px-4 py-2 text-text-light">{item.nombreSucursal}</td>
                      <td className="px-4 py-2">
                        {item.esMain
                          ? <Badge variant="info">Principal</Badge>
                          : displayEstablecimientos.length > 1 && (
                            <button
                              type="button"
                              onClick={() => handleSetMainEstablecimiento(item.key, item.establecimientoId)}
                              disabled={settingMainEstablecimientoId === item.key}
                              className="text-xs text-text-light hover:text-primary-600 transition-colors disabled:opacity-50"
                            >
                              Marcar principal
                            </button>
                          )
                        }
                      </td>
                      <td className="px-4 py-2 text-right">
                        <button
                          type="button"
                          onClick={() => handleRemoveEstablecimiento(item.key, item.establecimientoId, item.esMain)}
                          disabled={removingEstablecimientoId === item.key}
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
