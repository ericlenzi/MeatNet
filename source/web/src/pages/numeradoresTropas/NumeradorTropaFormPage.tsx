import { useState, useEffect } from 'react'
import type { FormEvent } from 'react'
import { useParams, useNavigate } from 'react-router'
import {
  getNumeradorTropa,
  createNumeradorTropa,
  updateNumeradorTropa,
} from '@/services/numeradoresTropas.service'
import { getClientes, getClienteEstablecimientos } from '@/services/clientes.service'
import type { ClienteEstablecimientoItem } from '@/services/clientes.service'
import { getEstablecimiento } from '@/services/establecimientos.service'
import { useToast } from '@/components/ui/Toast'
import type { Cliente, EspecieItem } from '@/types'
import Input from '@/components/ui/Input'
import Select from '@/components/ui/Select'
import Button from '@/components/ui/Button'
import PageHeader from '@/components/ui/PageHeader'
import Spinner from '@/components/ui/Spinner'

export default function NumeradorTropaFormPage() {
  const { id } = useParams()
  const navigate = useNavigate()
  const { toast } = useToast()
  const isEdit = !!id

  const [fetching, setFetching] = useState(true)
  const [loading, setLoading] = useState(false)

  const [clientes, setClientes] = useState<Cliente[]>([])
  const [clienteEstablecimientos, setClienteEstablecimientos] = useState<ClienteEstablecimientoItem[]>([])
  const [especiesDisponibles, setEspeciesDisponibles] = useState<EspecieItem[]>([])

  const [selectedClienteId, setSelectedClienteId] = useState('')
  const [form, setForm] = useState({
    ClienteEstablecimientoId: '',
    EspecieCodigo: '',
    UltimoNumeroTropa: 0,
  })

  const [readonlyLabels, setReadonlyLabels] = useState({
    cliente: '',
    establecimiento: '',
    especie: '',
  })

  const [errors, setErrors] = useState<Record<string, string>>({})

  useEffect(() => {
    const loadData = async () => {
      try {
        const clientesRes = await getClientes({ PageSize: 1000, Estado: true })
        setClientes(clientesRes.data || [])

        if (isEdit && id) {
          const entity = await getNumeradorTropa(id)
          setForm({
            ClienteEstablecimientoId: entity.clienteEstablecimientoId,
            EspecieCodigo: entity.especieCodigo,
            UltimoNumeroTropa: entity.ultimoNumeroTropa,
          })
          setReadonlyLabels({
            cliente: `${entity.codigoCliente} - ${entity.nombreCliente}`,
            establecimiento: `${entity.codigoEstablecimiento} - ${entity.nombreEstablecimiento}`,
            especie: entity.especieNombre,
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

  // Cargar ClienteEstablecimientos cuando cambia el cliente
  useEffect(() => {
    if (!selectedClienteId) {
      setClienteEstablecimientos([])
      setEspeciesDisponibles([])
      setForm((prev) => ({ ...prev, ClienteEstablecimientoId: '', EspecieCodigo: '' }))
      return
    }
    const load = async () => {
      try {
        const items = await getClienteEstablecimientos(selectedClienteId)
        setClienteEstablecimientos(items)
      } catch {
        setClienteEstablecimientos([])
      }
    }
    void load()
  }, [selectedClienteId])

  // Cargar especies del establecimiento cuando cambia el ClienteEstablecimiento
  const handleClienteEstablecimientoChange = async (ceId: string) => {
    setForm((prev) => ({ ...prev, ClienteEstablecimientoId: ceId, EspecieCodigo: '' }))
    setEspeciesDisponibles([])
    if (!ceId) return
    const ce = clienteEstablecimientos.find((x) => x.id === ceId)
    if (!ce) return
    try {
      const est = await getEstablecimiento(ce.establecimientoId)
      setEspeciesDisponibles(est.especies || [])
    } catch {
      setEspeciesDisponibles([])
    }
  }

  const validate = (): boolean => {
    const newErrors: Record<string, string> = {}
    if (!form.ClienteEstablecimientoId) newErrors['ClienteEstablecimientoId'] = 'Requerido'
    if (!form.EspecieCodigo) newErrors['EspecieCodigo'] = 'Requerido'
    if (form.UltimoNumeroTropa < 0) newErrors['UltimoNumeroTropa'] = 'Debe ser mayor o igual a 0'
    setErrors(newErrors)
    return Object.keys(newErrors).length === 0
  }

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault()
    if (!validate()) return

    setLoading(true)
    try {
      if (isEdit && id) {
        await updateNumeradorTropa(id, { UltimoNumeroTropa: form.UltimoNumeroTropa })
        toast('success', 'Numerador actualizado')
      } else {
        await createNumeradorTropa({
          ClienteEstablecimientoId: form.ClienteEstablecimientoId,
          EspecieCodigo: form.EspecieCodigo,
          UltimoNumeroTropa: form.UltimoNumeroTropa,
        })
        toast('success', 'Numerador creado')
      }
      navigate('/numeradores-tropas')
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
      <PageHeader title={isEdit ? 'Editar Numerador de Tropa' : 'Nuevo Numerador de Tropa'} />

      <div className="mx-auto max-w-2xl">
        <form onSubmit={handleSubmit} className="rounded-lg border border-border bg-surface p-6 shadow-sm">
          <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
            {isEdit ? (
              <>
                <Input label="Cliente" value={readonlyLabels.cliente} disabled />
                <Input label="Establecimiento" value={readonlyLabels.establecimiento} disabled />
                <Input label="Especie" value={readonlyLabels.especie} disabled />
              </>
            ) : (
              <>
                <div className="sm:col-span-2">
                  <Select
                    label="Cliente"
                    value={selectedClienteId}
                    onChange={(e) => setSelectedClienteId(e.target.value)}
                    options={clientes.map((c) => ({
                      value: c.id,
                      label: `${c.codigoCliente} - ${c.nombre}`,
                    }))}
                    placeholder="Seleccionar cliente..."
                  />
                </div>
                <div className="sm:col-span-2">
                  <Select
                    label="Establecimiento"
                    value={form.ClienteEstablecimientoId}
                    onChange={(e) => handleClienteEstablecimientoChange(e.target.value)}
                    options={clienteEstablecimientos.map((ce) => ({
                      value: ce.id,
                      label: `${ce.codigoEstablecimiento} - ${ce.nombre}`,
                    }))}
                    placeholder={selectedClienteId ? 'Seleccionar establecimiento...' : 'Primero seleccione un cliente'}
                    error={errors['ClienteEstablecimientoId']}
                    disabled={!selectedClienteId}
                  />
                </div>
                <div className="sm:col-span-2">
                  <Select
                    label="Especie"
                    value={form.EspecieCodigo}
                    onChange={(e) => setForm((prev) => ({ ...prev, EspecieCodigo: e.target.value }))}
                    options={especiesDisponibles.map((esp) => ({
                      value: esp.id,
                      label: esp.nombre,
                    }))}
                    placeholder={form.ClienteEstablecimientoId ? 'Seleccionar especie...' : 'Primero seleccione un establecimiento'}
                    error={errors['EspecieCodigo']}
                    disabled={!form.ClienteEstablecimientoId}
                  />
                </div>
              </>
            )}

            <div className="sm:col-span-2">
              <Input
                label="Ultimo Numero Tropa"
                type="number"
                value={String(form.UltimoNumeroTropa)}
                onChange={(e) =>
                  setForm((prev) => ({ ...prev, UltimoNumeroTropa: Number(e.target.value) }))
                }
                error={errors['UltimoNumeroTropa']}
              />
            </div>
          </div>

          <div className="mt-6 flex justify-end gap-3">
            <Button variant="secondary" type="button" onClick={() => navigate('/numeradores-tropas')}>
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
