import { useState, useEffect } from 'react'
import type { FormEvent } from 'react'
import { useParams, useNavigate } from 'react-router'
import {
  getUnidadFaena,
  createUnidadFaena,
  updateUnidadFaena,
} from '@/services/unidadesFaenas.service'
import { getEspecies } from '@/services/especies.service'
import { getTiposMateriales } from '@/services/materiales.service'
import type { Especie, TipoMaterial } from '@/types'
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
  const [tipos, setTipos] = useState<TipoMaterial[]>([])

  const [form, setForm] = useState({
    Codigo: '',
    EspecieId: '',
    Nombre: '',
    CantidadCuartos: '0',
    PiezasPorAnimal: '1',
    PorDefecto: false,
    TipoMaterialId: '',
    ERP_Codigo: '',
    Activo: true,
  })
  const [errors, setErrors] = useState<Record<string, string>>({})

  useEffect(() => {
    const load = async () => {
      try {
        const [esp, tiposRes] = await Promise.all([
          getEspecies({ Estado: true, PageSize: 1000 }),
          getTiposMateriales(),
        ])
        setEspecies(esp.data || [])
        setTipos(tiposRes)
        if (isEdit && id) {
          const e = await getUnidadFaena(id)
          setForm({
            Codigo: e.codigo ?? '',
            EspecieId: e.especieId ?? '',
            Nombre: e.nombre ?? '',
            CantidadCuartos: String(e.cantidadCuartos),
            PiezasPorAnimal: String(e.piezasPorAnimal),
            PorDefecto: e.porDefecto,
            TipoMaterialId: e.tipoMaterialId ?? '',
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
    if (!isEdit && !form.Codigo.trim()) e['Codigo'] = 'Requerido'
    if (!form.EspecieId) e['EspecieId'] = 'Requerido'
    if (!form.Nombre.trim()) e['Nombre'] = 'Requerido'

    const cuartos = num(form.CantidadCuartos)
    const piezas = num(form.PiezasPorAnimal)
    if (cuartos < 0 || cuartos > 4) e['CantidadCuartos'] = 'Entre 0 y 4 (0 para decomisos)'
    if (piezas < 1 || piezas > 4) e['PiezasPorAnimal'] = 'Entre 1 y 4'
    // Un animal tiene cuatro cuartos: las piezas que salen de uno no pueden sumar mas.
    if (!e['CantidadCuartos'] && !e['PiezasPorAnimal'] && cuartos * piezas > 4) {
      const suman = 'No cierra: suman ' + cuartos * piezas + ' cuartos y un animal tiene 4'
      e['CantidadCuartos'] = suman
      e['PiezasPorAnimal'] = suman
    }

    if (!form.TipoMaterialId) e['TipoMaterialId'] = 'Requerido'
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
        Nombre: form.Nombre,
        CantidadCuartos: num(form.CantidadCuartos),
        PiezasPorAnimal: num(form.PiezasPorAnimal),
        PorDefecto: form.PorDefecto,
        TipoMaterialId: form.TipoMaterialId,
        ERP_Codigo: form.ERP_Codigo,
      }
      if (isEdit && id) {
        await updateUnidadFaena(id, { ...payload, Activo: form.Activo })
        toast('success', 'Unidad de faena actualizada')
      } else {
        await createUnidadFaena({ ...payload, Codigo: form.Codigo.trim() })
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
              label="Codigo"
              value={form.Codigo}
              onChange={(e) => updateField('Codigo', e.target.value)}
              error={errors['Codigo']}
              disabled={isEdit}
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
            <Select
              label="Tipo de material"
              value={form.TipoMaterialId}
              onChange={(e) => updateField('TipoMaterialId', e.target.value)}
              options={tipos.map((t) => ({ value: t.codigo, label: t.nombre }))}
              placeholder="Seleccionar tipo..."
              error={errors['TipoMaterialId']}
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
