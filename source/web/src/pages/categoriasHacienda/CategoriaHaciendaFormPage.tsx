import { useState, useEffect, useMemo } from 'react'
import type { FormEvent } from 'react'
import { useParams, useNavigate } from 'react-router'
import {
  getEmpresaTipoEspecie,
  getEmpresasTiposEspecies,
  createEmpresaTipoEspecie,
  updateEmpresaTipoEspecie,
} from '@/services/empresasTiposEspecies.service'
import { getTiposEspecies } from '@/services/tiposEspecies.service'
import { useToast } from '@/components/ui/Toast'
import type { EmpresaTipoEspecie, TipoEspecie } from '@/types'
import Input from '@/components/ui/Input'
import Select from '@/components/ui/Select'
import Button from '@/components/ui/Button'
import PageHeader from '@/components/ui/PageHeader'
import Spinner from '@/components/ui/Spinner'

// Alta: se elige una categoria del catalogo global entre las que la empresa todavia no tiene,
// y el peso teorico viene precargado con el de referencia. Edicion: la categoria ya no se
// cambia (seria otra fila), solo sus parametros.
export default function CategoriaHaciendaFormPage() {
  const { id } = useParams()
  const navigate = useNavigate()
  const { toast } = useToast()
  const isEdit = !!id

  const [loading, setLoading] = useState(false)
  const [fetching, setFetching] = useState(true)
  const [disponibles, setDisponibles] = useState<TipoEspecie[]>([])
  const [actual, setActual] = useState<EmpresaTipoEspecie | null>(null)
  const [form, setForm] = useState({
    TipoEspecieId: '',
    PesoTeorico: '',
    ERP_Codigo: '',
    Activo: true,
  })
  const [errors, setErrors] = useState<Record<string, string>>({})

  useEffect(() => {
    const loadData = async () => {
      try {
        if (isEdit && id) {
          setActual(await getEmpresaTipoEspecie(id))
        } else {
          // El catalogo trae todas las categorias del rubro; se ofrecen las que la empresa aun
          // no configuro, porque una categoria no se puede dar de alta dos veces.
          const [catalogo, configuradas] = await Promise.all([
            getTiposEspecies({ PageSize: 1000, Estado: true }),
            getEmpresasTiposEspecies({ PageSize: 1000 }),
          ])
          const yaEstan = new Set((configuradas.data || []).map((c) => c.tipoEspecieId))
          setDisponibles((catalogo.data || []).filter((t) => !yaEstan.has(t.codigo)))
        }
      } catch {
        toast('error', 'Error al cargar datos')
      } finally {
        setFetching(false)
      }
    }
    void loadData()
  }, [id, isEdit, toast])

  useEffect(() => {
    if (!actual) return
    setForm({
      TipoEspecieId: actual.tipoEspecieId,
      PesoTeorico: actual.pesoTeorico != null ? String(actual.pesoTeorico) : '',
      ERP_Codigo: actual.erP_Codigo || '',
      Activo: actual.activo,
    })
  }, [actual])

  const seleccionada = useMemo(
    () => disponibles.find((t) => t.codigo === form.TipoEspecieId) || null,
    [disponibles, form.TipoEspecieId],
  )

  const referencia = isEdit ? actual?.pesoTeoricoReferencia : seleccionada?.pesoTeoricoReferencia

  const handleCategoriaChange = (codigo: string) => {
    const elegida = disponibles.find((t) => t.codigo === codigo)
    setForm((prev) => ({
      ...prev,
      TipoEspecieId: codigo,
      // El peso de referencia es una propuesta: se precarga y despues queda desacoplado.
      PesoTeorico: elegida ? String(elegida.pesoTeoricoReferencia) : prev.PesoTeorico,
    }))
    if (errors['TipoEspecieId']) setErrors((prev) => ({ ...prev, TipoEspecieId: '' }))
  }

  const validate = (): boolean => {
    const newErrors: Record<string, string> = {}
    if (!isEdit && !form.TipoEspecieId) newErrors['TipoEspecieId'] = 'Requerido'
    if (form.PesoTeorico !== '' && Number(form.PesoTeorico) < 0)
      newErrors['PesoTeorico'] = 'No puede ser negativo'
    setErrors(newErrors)
    return Object.keys(newErrors).length === 0
  }

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault()
    if (!validate()) return

    setLoading(true)
    try {
      if (isEdit && id) {
        await updateEmpresaTipoEspecie(id, {
          PesoTeorico: Number(form.PesoTeorico || 0),
          ERP_Codigo: form.ERP_Codigo || undefined,
          Activo: form.Activo,
        })
        toast('success', 'Categoria actualizada')
      } else {
        await createEmpresaTipoEspecie({
          TipoEspecieId: form.TipoEspecieId,
          PesoTeorico: form.PesoTeorico ? Number(form.PesoTeorico) : undefined,
          ERP_Codigo: form.ERP_Codigo || undefined,
        })
        toast('success', 'Categoria agregada')
      }
      navigate('/categorias-hacienda')
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

  const sinDisponibles = !isEdit && disponibles.length === 0

  return (
    <>
      <PageHeader title={isEdit ? 'Editar Categoria' : 'Agregar Categoria'} />

      <div className="mx-auto max-w-2xl">
        <form onSubmit={handleSubmit} className="rounded-lg border border-border bg-surface p-6 shadow-sm">
          {sinDisponibles ? (
            <p className="text-sm text-text-light">
              La empresa ya opera con todas las categorias del catalogo. Si falta una, la tiene
              que dar de alta el administrador de la plataforma.
            </p>
          ) : (
            <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
              {isEdit ? (
                <Input label="Categoria" value={actual ? actual.tipoEspecieId + ' - ' + actual.nombre : ''} disabled />
              ) : (
                <Select
                  label="Categoria"
                  value={form.TipoEspecieId}
                  onChange={(e) => handleCategoriaChange(e.target.value)}
                  options={disponibles.map((t) => ({
                    value: t.codigo,
                    label: t.codigo + ' - ' + t.nombre,
                  }))}
                  placeholder="Seleccionar categoria..."
                  error={errors['TipoEspecieId']}
                />
              )}
              <Input
                label="Peso Teorico"
                type="number"
                value={form.PesoTeorico}
                onChange={(e) => updateField('PesoTeorico', e.target.value)}
                error={errors['PesoTeorico']}
              />
              <Input
                label="Codigo ERP"
                value={form.ERP_Codigo}
                onChange={(e) => updateField('ERP_Codigo', e.target.value)}
              />
            </div>
          )}

          {!sinDisponibles && referencia != null && (
            <p className="mt-3 text-sm text-text-light">
              Peso teorico de referencia del catalogo: {referencia} kg. Se propone al agregar la
              categoria y despues queda desacoplado: si el catalogo cambia, este valor no se toca.
            </p>
          )}

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
            <Button variant="secondary" type="button" onClick={() => navigate('/categorias-hacienda')}>
              {sinDisponibles ? 'Volver' : 'Cancelar'}
            </Button>
            {!sinDisponibles && (
              <Button type="submit" loading={loading}>
                {isEdit ? 'Guardar Cambios' : 'Agregar Categoria'}
              </Button>
            )}
          </div>
        </form>
      </div>
    </>
  )
}
