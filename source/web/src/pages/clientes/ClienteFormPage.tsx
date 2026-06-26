import { useState, useEffect, useCallback } from 'react'
import type { FormEvent } from 'react'
import { useParams, useNavigate } from 'react-router'
import {
  getCliente, createCliente, updateCliente,
  getClienteEstablecimientos, addClienteEstablecimiento, removeClienteEstablecimiento,
} from '@/services/clientes.service'
import type { ClienteEstablecimientoItem } from '@/services/clientes.service'
import { getTiposClientes } from '@/services/tiposClientes.service'
import { getEstablecimientos } from '@/services/establecimientos.service'
import { useToast } from '@/components/ui/Toast'
import type { TipoCliente, Establecimiento } from '@/types'
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
  const [allEstablecimientos, setAllEstablecimientos] = useState<Establecimiento[]>([])
  const [form, setForm] = useState({
    CodigoCliente: '',
    Nombre: '',
    TipoClienteId: '',
    NumeroCuit: '',
    NumeroIngresosBrutos: '',
    ERP_Codigo: '',
    Activo: true,
  })
  const [errors, setErrors] = useState<Record<string, string>>({})

  // Establecimientos
  const [clienteEstablecimientos, setClienteEstablecimientos] = useState<ClienteEstablecimientoItem[]>([])
  const [selectedEstablecimientoId, setSelectedEstablecimientoId] = useState('')
  const [addCodigoRenspa, setAddCodigoRenspa] = useState('')
  const [addNumeroCUIG, setAddNumeroCUIG] = useState('')
  const [addingEstablecimiento, setAddingEstablecimiento] = useState(false)
  const [removingEstablecimientoId, setRemovingEstablecimientoId] = useState<string | null>(null)
  const [pendingEstablecimientos, setPendingEstablecimientos] = useState<{
    establecimientoId: string
    nombre: string
    codigoEstablecimiento: string
    nombreSucursal: string
    codigoRenspa: string
    numeroCUIG: string
  }[]>([])

  useEffect(() => {
    const loadData = async () => {
      try {
        const [tipos, estsRes] = await Promise.all([
          getTiposClientes(),
          getEstablecimientos({ PageSize: 1000, Estado: true }),
        ])
        setTiposCliente(tipos)
        setAllEstablecimientos(estsRes.data || [])

        if (isEdit && id) {
          const entity = await getCliente(id)
          setForm({
            CodigoCliente: entity.codigoCliente || '',
            Nombre: entity.nombre || '',
            TipoClienteId: entity.tipoClienteId || '',
            NumeroCuit: entity.numeroCuit || '',
            NumeroIngresosBrutos: entity.numeroIngresosBrutos || '',
            ERP_Codigo: entity.erP_Codigo || '',
            Activo: entity.activo,
          })
          try {
            const ests = await getClienteEstablecimientos(id)
            setClienteEstablecimientos(ests)
          } catch {
            // establecimientos no disponibles, no bloquear la carga del formulario
          }
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
          ERP_Codigo: form.ERP_Codigo,
          Activo: form.Activo,
        })
        toast('success', 'Cliente actualizado')
      } else {
        const result = await createCliente({
          CodigoCliente: form.CodigoCliente,
          Nombre: form.Nombre,
          TipoClienteId: form.TipoClienteId,
          NumeroCuit: form.NumeroCuit,
          NumeroIngresosBrutos: form.NumeroIngresosBrutos,
          ERP_Codigo: form.ERP_Codigo,
        })
        for (const pe of pendingEstablecimientos) {
          await addClienteEstablecimiento(result.id, pe.establecimientoId, pe.codigoRenspa, pe.numeroCUIG)
        }
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

  // --- Establecimientos ---

  const assignedEstablecimientoIds = isEdit
    ? clienteEstablecimientos.map((ce) => ce.establecimientoId)
    : pendingEstablecimientos.map((pe) => pe.establecimientoId)

  const availableEstablecimientos = allEstablecimientos.filter(
    (e) => e.activo && !assignedEstablecimientoIds.includes(e.id),
  )

  const handleAddEstablecimiento = useCallback(async () => {
    if (!selectedEstablecimientoId) return

    if (isEdit && id) {
      setAddingEstablecimiento(true)
      try {
        await addClienteEstablecimiento(id, selectedEstablecimientoId, addCodigoRenspa, addNumeroCUIG)
        const updated = await getClienteEstablecimientos(id)
        setClienteEstablecimientos(updated)
        setSelectedEstablecimientoId('')
        setAddCodigoRenspa('')
        setAddNumeroCUIG('')
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
            nombreSucursal: est.sucursalNombre || '',
            codigoRenspa: addCodigoRenspa,
            numeroCUIG: addNumeroCUIG,
          },
        ])
        setSelectedEstablecimientoId('')
        setAddCodigoRenspa('')
        setAddNumeroCUIG('')
      }
    }
  }, [selectedEstablecimientoId, addCodigoRenspa, addNumeroCUIG, isEdit, id, allEstablecimientos, toast])

  const handleRemoveEstablecimiento = useCallback(async (key: string) => {
    if (isEdit && id) {
      setRemovingEstablecimientoId(key)
      try {
        await removeClienteEstablecimiento(id, key)
        const updated = await getClienteEstablecimientos(id)
        setClienteEstablecimientos(updated)
        toast('success', 'Establecimiento removido')
      } catch (err) {
        toast('error', err instanceof Error ? err.message : 'Error al remover')
      } finally {
        setRemovingEstablecimientoId(null)
      }
    } else {
      setPendingEstablecimientos((prev) => prev.filter((pe) => pe.establecimientoId !== key))
    }
  }, [isEdit, id, toast])

  const displayEstablecimientos = isEdit
    ? clienteEstablecimientos.map((ce) => ({
        key: ce.id,
        establecimientoId: ce.establecimientoId,
        codigoEstablecimiento: ce.codigoEstablecimiento,
        nombre: ce.nombre,
        nombreSucursal: ce.nombreSucursal,
        codigoRenspa: ce.codigoRenspa,
        numeroCUIG: ce.numeroCUIG,
      }))
    : pendingEstablecimientos.map((pe) => ({
        key: pe.establecimientoId,
        establecimientoId: pe.establecimientoId,
        codigoEstablecimiento: pe.codigoEstablecimiento,
        nombre: pe.nombre,
        nombreSucursal: pe.nombreSucursal,
        codigoRenspa: pe.codigoRenspa,
        numeroCUIG: pe.numeroCUIG,
      }))

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

      <div className="mx-auto max-w-2xl space-y-6">
        {/* Datos del cliente */}
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

        {/* Establecimientos del cliente */}
        <div className="rounded-lg border border-border bg-surface p-6 shadow-sm">
          <h2 className="mb-4 text-lg font-semibold text-text">Establecimientos Asignados</h2>

          <div className="mb-4 grid grid-cols-1 gap-3 sm:grid-cols-2">
            <div className="sm:col-span-2">
              <Select
                label="Establecimiento"
                value={selectedEstablecimientoId}
                onChange={(e) => setSelectedEstablecimientoId(e.target.value)}
                options={availableEstablecimientos.map((e) => ({
                  value: e.id,
                  label: `${e.codigoEstablecimiento} - ${e.nombre}`,
                }))}
                placeholder="Seleccionar establecimiento..."
              />
            </div>
            <Input
              label="RENSPA"
              value={addCodigoRenspa}
              onChange={(e) => setAddCodigoRenspa(e.target.value)}
            />
            <Input
              label="CUIG"
              value={addNumeroCUIG}
              onChange={(e) => setAddNumeroCUIG(e.target.value)}
            />
            <div className="sm:col-span-2 flex justify-end">
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
                    <th className="px-4 py-2 text-left text-xs font-semibold uppercase tracking-wider text-text-light">RENSPA</th>
                    <th className="px-4 py-2 text-left text-xs font-semibold uppercase tracking-wider text-text-light">CUIG</th>
                    <th className="px-4 py-2 text-right text-xs font-semibold uppercase tracking-wider text-text-light">Accion</th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-border">
                  {displayEstablecimientos.map((item) => (
                    <tr key={item.key} className="hover:bg-gray-50">
                      <td className="px-4 py-2 text-text">{item.codigoEstablecimiento}</td>
                      <td className="px-4 py-2 text-text">{item.nombre}</td>
                      <td className="px-4 py-2 text-text-light">{item.nombreSucursal}</td>
                      <td className="px-4 py-2 text-text-light">{item.codigoRenspa || '—'}</td>
                      <td className="px-4 py-2 text-text-light">{item.numeroCUIG || '—'}</td>
                      <td className="px-4 py-2 text-right">
                        <button
                          type="button"
                          onClick={() => handleRemoveEstablecimiento(item.key)}
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
