import { useState, useEffect } from 'react'
import type { FormEvent } from 'react'
import { useParams, useNavigate } from 'react-router'
import {
  getUnidadFaena,
  createUnidadFaena,
  updateUnidadFaena,
} from '@/services/unidadesFaenas.service'
import { getEspecies } from '@/services/especies.service'
import type { Especie } from '@/types'
import { useToast } from '@/components/ui/Toast'
import Input from '@/components/ui/Input'
import Select from '@/components/ui/Select'
import Button from '@/components/ui/Button'
import PageHeader from '@/components/ui/PageHeader'
import Spinner from '@/components/ui/Spinner'

const num = (v: string): number => Number(v) || 0

export default function UnidadFaenaFormPage() {
  const { id } = useParams()
  const navigate = useNavigate()
  const { toast } = useToast()
  const isEdit = !!id

  const [fetching, setFetching] = useState(true)
  const [loading, setLoading] = useState(false)
  const [especies, setEspecies] = useState<Especie[]>([])

  const [form, setForm] = useState({
    EspecieId: '',
    Numero: '1',
    Nombre: '',
    CantidadCuartos: '0',
    PiezasPorAnimal: '1',
    PorDefecto: false,
    CodigoMaterial: '',
    ERP_Codigo: '',
    Activo: true,
  })
  const [errors, setErrors] = useState<Record<string, string>>({})

  useEffect(() => {
    const load = async () => {
      try {
        const esp = await getEspecies({ Estado: true, PageSize: 1000 })
        setEspecies(esp.data || [])
        if (isEdit && id) {
          const e = await getUnidadFaena(id)
          setForm({
            EspecieId: e.especieId ?? '',
            Numero: String(e.numero),
            Nombre: e.nombre ?? '',
            CantidadCuartos: String(e.cantidadCuartos),
            PiezasPorAnimal: String(e.piezasPorAnimal),
            PorDefecto: e.porDefecto,
            CodigoMaterial: e.codigoMaterial ?? '',
            ERP_Codigo: e.erP_Codigo ?? '',
            Activo: e.activo,
          })
        }
      } catch {
        toast('error', 'Error al cargar datos')
      } finally {
        setFetching(false)
      }
    }
    void load()
  }, [id, isEdit, toast])

  const updateField = (field: string, value: string | boolean) => {
    setForm((p) => ({ ...p, [field]: value }))
    setErrors((p) => ({ ...p, [field]: '' }))
  }

  const validate = (): boolean => {
    const e: Record<string, string> = {}
    if (!form.EspecieId) e['EspecieId'] = 'Requerido'
    if (!form.Nombre.trim()) e['Nombre'] = 'Requerido'
    if (num(form.Numero) < 1) e['Numero'] = 'Debe ser mayor o igual a 1'
    if (num(form.CantidadCuartos) < 0) e['CantidadCuartos'] = 'Debe ser mayor o igual a 0'
    if (num(form.PiezasPorAnimal) < 1) e['PiezasPorAnimal'] = 'Debe ser mayor o igual a 1'
    setErrors(e)
    return Object.keys(e).length === 0
  }

  const handleSubmit = async (ev: FormEvent) => {
    ev.preventDefault()
    if (!validate()) return
    setLoading(true)
    try {
      const payload = {
        EspecieId: form.EspecieId,
        Numero: num(form.Numero),
        Nombre: form.Nombre,
        CantidadCuartos: num(form.CantidadCuartos),
        PiezasPorAnimal: num(form.PiezasPorAnimal),
        PorDefecto: form.PorDefecto,
        CodigoMaterial: form.CodigoMaterial,
        ERP_Codigo: form.ERP_Codigo,
      }
      if (isEdit && id) {
        await updateUnidadFaena(id, { ...payload, Activo: form.Activo })
        toast('success', 'Unidad de faena actualizada')
      } else {
        await createUnidadFaena(payload)
        toast('success', 'Unidad de faena creada')
      }
      navigate('/unidades-faenas')
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
      <PageHeader title={isEdit ? 'Editar Unidad de Faena' : 'Nueva Unidad de Faena'} />

      <div className="mx-auto max-w-2xl">
        <form onSubmit={handleSubmit} className="rounded-lg border border-border bg-surface p-6 shadow-sm">
          <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
            <Select
              label="Especie"
              value={form.EspecieId}
              onChange={(e) => updateField('EspecieId', e.target.value)}
              options={especies.map((e) => ({ value: e.codigo, label: e.nombre }))}
              placeholder="Seleccionar especie..."
              error={errors['EspecieId']}
            />
            <Input
              label="Numero"
              type="number"
              value={form.Numero}
              onChange={(e) => updateField('Numero', e.target.value)}
              error={errors['Numero']}
            />
            <Input
              label="Nombre"
              value={form.Nombre}
              onChange={(e) => updateField('Nombre', e.target.value)}
              error={errors['Nombre']}
            />
            <Input
              label="Cantidad de cuartos"
              type="number"
              value={form.CantidadCuartos}
              onChange={(e) => updateField('CantidadCuartos', e.target.value)}
              error={errors['CantidadCuartos']}
            />
            <Input
              label="Piezas por animal"
              type="number"
              value={form.PiezasPorAnimal}
              onChange={(e) => updateField('PiezasPorAnimal', e.target.value)}
              error={errors['PiezasPorAnimal']}
            />
            <Input
              label="Codigo material"
              value={form.CodigoMaterial}
              onChange={(e) => updateField('CodigoMaterial', e.target.value)}
            />
            <Input
              label="Codigo ERP"
              value={form.ERP_Codigo}
              onChange={(e) => updateField('ERP_Codigo', e.target.value)}
            />
          </div>

          <div className="mt-4">
            <label className="flex items-center gap-2 text-sm text-text">
              <input
                type="checkbox"
                checked={form.PorDefecto}
                onChange={(e) => updateField('PorDefecto', e.target.checked)}
                className="h-4 w-4 rounded border-border text-primary-600 focus:ring-primary-500"
              />
              Unidad por defecto de la especie
            </label>
            <p className="ml-6 mt-0.5 text-xs text-text-light">
              El Tipificador la propone por defecto. Solo una por especie.
            </p>
          </div>

          {isEdit && (
            <div className="mt-4">
              <label className="flex items-center gap-2 text-sm text-text">
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
            <Button variant="secondary" type="button" onClick={() => navigate('/unidades-faenas')}>
              Cancelar
            </Button>
            <Button type="submit" loading={loading}>
              {isEdit ? 'Guardar Cambios' : 'Crear Unidad'}
            </Button>
          </div>
        </form>
      </div>
    </>
  )
}
