import { useState, useEffect } from 'react'
import type { FormEvent } from 'react'
import { useParams, useNavigate } from 'react-router'
import { getEstablecimiento, createEstablecimiento, updateEstablecimiento } from '@/services/establecimientos.service'
import { getSucursales } from '@/services/sucursales.service'
import { getEspecies } from '@/services/especies.service'
import { getEmpresas } from '@/services/empresas.service'
import { useToast } from '@/components/ui/Toast'
import { useAuth } from '@/contexts/AuthContext'
import type { Sucursal, Especie, Empresa } from '@/types'
import type { EspecieItem } from '@/types/establecimiento'
import Input from '@/components/ui/Input'
import Select from '@/components/ui/Select'
import Button from '@/components/ui/Button'
import PageHeader from '@/components/ui/PageHeader'
import Spinner from '@/components/ui/Spinner'

export default function EstablecimientoFormPage() {
  const { id } = useParams()
  const navigate = useNavigate()
  const { toast } = useToast()
  const { user } = useAuth()
  const isEdit = !!id

  const [loading, setLoading] = useState(false)
  const [fetching, setFetching] = useState(true)
  const [sucursales, setSucursales] = useState<Sucursal[]>([])
  const [allEspecies, setAllEspecies] = useState<Especie[]>([])
  const [empresas, setEmpresas] = useState<Empresa[]>([])
  const [form, setForm] = useState({
    CodigoEstablecimiento: '',
    Nombre: '',
    EmpresaId: '',
    SucursalId: '',
    NumeroSenasa: '',
    NumeroRuca: '',
    Activo: true,
  })
  const [selectedEspecies, setSelectedEspecies] = useState<EspecieItem[]>([])
  const [selectedEspecieId, setSelectedEspecieId] = useState('')
  const [errors, setErrors] = useState<Record<string, string>>({})
  const [removingEspecieId, setRemovingEspecieId] = useState<string | null>(null)

  useEffect(() => {
    const loadData = async () => {
      try {
        const [sucRes, espRes, empRes] = await Promise.all([
          getSucursales({ PageSize: 1000 }),
          getEspecies({ PageSize: 1000, Estado: true }),
          getEmpresas({ PageSize: 1000 }),
        ])
        setSucursales((sucRes.data || []).filter((s) => s.activo))
        setAllEspecies(espRes.data || [])
        const empList = empRes.data || []
        setEmpresas(empList)

        const empresaActiva = user?.codigoEmpresa
          ? empList.find((e) => e.codigoEmpresa === user.codigoEmpresa)
          : undefined

        if (empresaActiva) {
          setForm((prev) => ({ ...prev, EmpresaId: empresaActiva.id }))
        }

        if (isEdit && id) {
          const entity = await getEstablecimiento(id)
          setForm({
            CodigoEstablecimiento: entity.codigoEstablecimiento || '',
            Nombre: entity.nombre || '',
            EmpresaId: empresaActiva?.id || '',
            SucursalId: entity.sucursalId || '',
            NumeroSenasa: entity.numeroSenasa || '',
            NumeroRuca: entity.numeroRuca || '',
            Activo: entity.activo,
          })
          setSelectedEspecies(entity.especies || [])
        }
      } catch {
        toast('error', 'Error al cargar datos')
      } finally {
        setFetching(false)
      }
    }
    void loadData()
  }, [id, isEdit, toast])

  const availableEspecies = allEspecies.filter(
    (e) => !selectedEspecies.some((s) => s.id === e.codigo),
  )

  const handleAddEspecie = () => {
    if (!selectedEspecieId) return
    const especie = allEspecies.find((e) => e.codigo === selectedEspecieId)
    if (especie) {
      setSelectedEspecies((prev) => [...prev, { id: especie.codigo, nombre: especie.nombre }])
      setSelectedEspecieId('')
      if (errors['Especies']) setErrors((prev) => ({ ...prev, Especies: '' }))
    }
  }

  const handleRemoveEspecie = (especieId: string) => {
    if (selectedEspecies.length <= 1) {
      toast('error', 'El establecimiento debe tener al menos una especie asignada.')
      return
    }
    setRemovingEspecieId(especieId)
    setSelectedEspecies((prev) => prev.filter((e) => e.id !== especieId))
    setRemovingEspecieId(null)
  }

  const validate = (): boolean => {
    const newErrors: Record<string, string> = {}
    if (!form.CodigoEstablecimiento.trim()) newErrors['CodigoEstablecimiento'] = 'Requerido'
    if (!form.Nombre.trim()) newErrors['Nombre'] = 'Requerido'
    if (!form.SucursalId) newErrors['SucursalId'] = 'Requerido'
    if (selectedEspecies.length === 0) newErrors['Especies'] = 'Debe asignar al menos una especie'
    setErrors(newErrors)
    return Object.keys(newErrors).length === 0
  }

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault()
    if (!validate()) return

    const especieIds = selectedEspecies.map((e) => e.id)

    setLoading(true)
    try {
      if (isEdit && id) {
        await updateEstablecimiento(id, {
          Nombre: form.Nombre,
          SucursalId: form.SucursalId,
          EspecieIds: especieIds,
          NumeroSenasa: form.NumeroSenasa,
          NumeroRuca: form.NumeroRuca,
          Activo: form.Activo,
        })
        toast('success', 'Establecimiento actualizado')
      } else {
        await createEstablecimiento({
          CodigoEstablecimiento: form.CodigoEstablecimiento,
          Nombre: form.Nombre,
          SucursalId: form.SucursalId,
          EspecieIds: especieIds,
          NumeroSenasa: form.NumeroSenasa,
          NumeroRuca: form.NumeroRuca,
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

      <div className="mx-auto max-w-2xl space-y-6">
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
              label="Empresa"
              value={form.EmpresaId}
              onChange={(e) => updateField('EmpresaId', e.target.value)}
              options={empresas.map((emp) => ({
                value: emp.id,
                label: `${emp.codigoEmpresa} - ${emp.nombre}`,
              }))}
              placeholder="Seleccionar..."
              disabled
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
            <Input
              label="Numero SENASA"
              value={form.NumeroSenasa}
              onChange={(e) => updateField('NumeroSenasa', e.target.value)}
            />
            <Input
              label="Numero RUCA"
              value={form.NumeroRuca}
              onChange={(e) => updateField('NumeroRuca', e.target.value)}
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

        {/* Especies */}
        <div className={`rounded-lg border bg-surface p-6 shadow-sm ${errors['Especies'] ? 'border-danger' : 'border-border'}`}>
          <h2 className="mb-4 text-lg font-semibold text-text">Especies</h2>

          {errors['Especies'] && (
            <div className="mb-4 rounded-lg bg-red-50 px-4 py-3 text-sm text-danger">
              {errors['Especies']}
            </div>
          )}

          <div className="mb-4 flex items-end gap-3">
            <div className="flex-1">
              <Select
                label="Agregar especie"
                value={selectedEspecieId}
                onChange={(e) => setSelectedEspecieId(e.target.value)}
                options={availableEspecies.map((e) => ({
                  value: e.codigo,
                  label: e.nombre,
                }))}
                placeholder="Seleccionar especie..."
              />
            </div>
            <Button
              type="button"
              size="md"
              onClick={handleAddEspecie}
              disabled={!selectedEspecieId}
            >
              Agregar
            </Button>
          </div>

          {selectedEspecies.length === 0 ? (
            <p className="py-4 text-center text-sm text-text-light">
              No hay especies asignadas
            </p>
          ) : (
            <div className="overflow-hidden rounded-lg border border-border">
              <table className="w-full text-sm">
                <thead>
                  <tr className="border-b border-border bg-gray-50">
                    <th className="px-4 py-2 text-left text-xs font-semibold uppercase tracking-wider text-text-light">Codigo</th>
                    <th className="px-4 py-2 text-left text-xs font-semibold uppercase tracking-wider text-text-light">Nombre</th>
                    <th className="px-4 py-2 text-right text-xs font-semibold uppercase tracking-wider text-text-light">Accion</th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-border">
                  {selectedEspecies.map((e) => (
                    <tr key={e.id} className="hover:bg-gray-50">
                      <td className="px-4 py-2 text-text-light">{e.id}</td>
                      <td className="px-4 py-2 text-text">{e.nombre}</td>
                      <td className="px-4 py-2 text-right">
                        <button
                          type="button"
                          onClick={() => handleRemoveEspecie(e.id)}
                          disabled={removingEspecieId === e.id}
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
