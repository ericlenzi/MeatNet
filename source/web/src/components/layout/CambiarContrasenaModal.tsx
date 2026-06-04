import { useState } from 'react'
import type { FormEvent } from 'react'
import { useToast } from '@/components/ui/Toast'
import api from '@/services/axios-instance'
import Modal from '@/components/ui/Modal'
import Input from '@/components/ui/Input'
import Button from '@/components/ui/Button'

interface CambiarContrasenaModalProps {
  isOpen: boolean
  onClose: () => void
  forzado?: boolean
}

export default function CambiarContrasenaModal({ isOpen, onClose, forzado = false }: CambiarContrasenaModalProps) {
  const { toast } = useToast()
  const [loading, setLoading] = useState(false)
  const [form, setForm] = useState({
    ContraseñaActual: '',
    ContraseñaNueva: '',
    ConfirmarContraseña: '',
  })
  const [errors, setErrors] = useState<Record<string, string>>({})

  const resetForm = () => {
    setForm({ ContraseñaActual: '', ContraseñaNueva: '', ConfirmarContraseña: '' })
    setErrors({})
  }

  const handleClose = () => {
    if (forzado) return
    resetForm()
    onClose()
  }

  const validate = (): boolean => {
    const newErrors: Record<string, string> = {}
    if (!form.ContraseñaActual.trim()) newErrors['ContraseñaActual'] = 'Requerido'
    if (!form.ContraseñaNueva.trim()) newErrors['ContraseñaNueva'] = 'Requerido'
    if (form.ContraseñaNueva.length < 4) newErrors['ContraseñaNueva'] = 'Minimo 4 caracteres'
    if (!form.ConfirmarContraseña.trim()) newErrors['ConfirmarContraseña'] = 'Requerido'
    if (form.ContraseñaNueva !== form.ConfirmarContraseña) newErrors['ConfirmarContraseña'] = 'Las contraseñas no coinciden'
    setErrors(newErrors)
    return Object.keys(newErrors).length === 0
  }

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault()
    if (!validate()) return

    setLoading(true)
    try {
      await api.put('/Autenticacion/CambiarContraseña', {
        ContraseñaActual: form.ContraseñaActual,
        ContraseñaNueva: form.ContraseñaNueva,
      })
      toast('success', 'Contraseña actualizada correctamente')
      resetForm()
      onClose()
    } catch (err) {
      toast('error', err instanceof Error ? err.message : 'Error al cambiar la contraseña')
    } finally {
      setLoading(false)
    }
  }

  const updateField = (field: string, value: string) => {
    setForm((prev) => ({ ...prev, [field]: value }))
    if (errors[field]) setErrors((prev) => ({ ...prev, [field]: '' }))
  }

  return (
    <Modal isOpen={isOpen} onClose={handleClose} title="Cambiar Contraseña" size="sm" closeable={!forzado}>
      <form onSubmit={handleSubmit}>
        {forzado && (
          <div className="mb-4 rounded-lg bg-amber-50 border border-amber-200 px-4 py-3 text-sm text-amber-800">
            Su contraseña es la contraseña inicial del sistema. Debe establecer una nueva contraseña para continuar.
          </div>
        )}
        <div className="space-y-4">
          <Input
            label="Contraseña Actual"
            type="password"
            value={form.ContraseñaActual}
            onChange={(e) => updateField('ContraseñaActual', e.target.value)}
            error={errors['ContraseñaActual']}
            autoComplete="current-password"
          />
          <Input
            label="Nueva Contraseña"
            type="password"
            value={form.ContraseñaNueva}
            onChange={(e) => updateField('ContraseñaNueva', e.target.value)}
            error={errors['ContraseñaNueva']}
            autoComplete="new-password"
          />
          <Input
            label="Confirmar Nueva Contraseña"
            type="password"
            value={form.ConfirmarContraseña}
            onChange={(e) => updateField('ConfirmarContraseña', e.target.value)}
            error={errors['ConfirmarContraseña']}
            autoComplete="new-password"
          />
        </div>

        <div className="mt-6 flex justify-end gap-3">
          {!forzado && (
            <Button variant="secondary" type="button" onClick={handleClose}>
              Cancelar
            </Button>
          )}
          <Button type="submit" loading={loading}>
            Cambiar Contraseña
          </Button>
        </div>
      </form>
    </Modal>
  )
}
