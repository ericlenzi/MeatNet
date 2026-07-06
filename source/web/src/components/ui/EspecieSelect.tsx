import { useEffect } from 'react'
import Select from '@/components/ui/Select'
import type { EspecieItem } from '@/types'

interface EspecieSelectProps {
  especies: EspecieItem[]
  value: string
  onChange: (especieId: string) => void
  label?: string
  placeholder?: string
  error?: string
  disabled?: boolean
}

/**
 * Selector de especie con la regla de negocio uniforme del sistema:
 * si el establecimiento tiene una unica especie habilitada, se muestra
 * seleccionada y bloqueada (no editable). Con varias, se elige del combo.
 * Usar SIEMPRE este componente donde haya que seleccionar la especie.
 */
export default function EspecieSelect({
  especies,
  value,
  onChange,
  label = 'Especie',
  placeholder = 'Seleccionar especie...',
  error,
  disabled = false,
}: EspecieSelectProps) {
  const only = especies.length === 1 ? especies[0] : null

  // Una sola especie habilitada: autoseleccionarla y bloquear el cambio
  useEffect(() => {
    if (disabled || !only) return
    if (value !== only.id) onChange(only.id)
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [only, disabled])

  return (
    <Select
      label={label}
      value={value}
      onChange={(e) => onChange(e.target.value)}
      options={especies.map((esp) => ({ value: esp.id, label: esp.nombre }))}
      placeholder={only ? undefined : placeholder}
      error={error}
      disabled={disabled || !!only}
    />
  )
}
