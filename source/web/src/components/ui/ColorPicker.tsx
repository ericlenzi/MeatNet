import { useState, useRef, useEffect } from 'react'

interface ColorPickerProps {
  label?: string
  value: string
  onChange: (color: string) => void
}

const presetColors = [
  '#EF4444', '#F97316', '#F59E0B', '#EAB308', '#84CC16',
  '#22C55E', '#14B8A6', '#06B6D4', '#0EA5E9', '#3B82F6',
  '#6366F1', '#8B5CF6', '#A855F7', '#D946EF', '#EC4899',
  '#F43F5E', '#78716C', '#64748B', '#1E293B', '#FFFFFF',
]

export default function ColorPicker({ label, value, onChange }: ColorPickerProps) {
  const [isOpen, setIsOpen] = useState(false)
  const ref = useRef<HTMLDivElement>(null)

  useEffect(() => {
    const handleClickOutside = (e: MouseEvent) => {
      if (ref.current && !ref.current.contains(e.target as Node)) {
        setIsOpen(false)
      }
    }
    document.addEventListener('mousedown', handleClickOutside)
    return () => document.removeEventListener('mousedown', handleClickOutside)
  }, [])

  return (
    <div className="w-full" ref={ref}>
      {label && (
        <label className="mb-1 block text-sm font-medium text-text">{label}</label>
      )}
      <div className="relative">
        <button
          type="button"
          onClick={() => setIsOpen(!isOpen)}
          className="flex w-full items-center gap-3 rounded-lg border border-border bg-white px-3 py-2 text-sm text-text hover:bg-gray-50 transition-colors focus:border-primary-500 focus:outline-none focus:ring-1 focus:ring-primary-500"
        >
          <span
            className="h-5 w-5 shrink-0 rounded border border-border"
            style={{ backgroundColor: value || '#FFFFFF' }}
          />
          <span className="flex-1 text-left">{value || 'Sin color'}</span>
          <svg className={`h-3.5 w-3.5 text-text-light transition-transform ${isOpen ? 'rotate-180' : ''}`} fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 9l-7 7-7-7" />
          </svg>
        </button>

        {isOpen && (
          <div className="absolute left-0 top-full z-50 mt-1 rounded-lg border border-border bg-white p-3 shadow-lg">
            {/* Preset colors grid */}
            <div className="mb-3 grid grid-cols-5 gap-2">
              {presetColors.map((color) => (
                <button
                  key={color}
                  type="button"
                  onClick={() => { onChange(color); setIsOpen(false) }}
                  className={`h-7 w-7 rounded-md border-2 transition-transform hover:scale-110 ${
                    value === color ? 'border-primary-600 ring-2 ring-primary-300' : 'border-border'
                  }`}
                  style={{ backgroundColor: color }}
                  title={color}
                />
              ))}
            </div>

            {/* Custom color input */}
            <div className="flex items-center gap-2 border-t border-border pt-3">
              <input
                type="color"
                value={value || '#3B82F6'}
                onChange={(e) => onChange(e.target.value.toUpperCase())}
                className="h-8 w-8 cursor-pointer rounded border border-border"
              />
              <input
                type="text"
                value={value}
                onChange={(e) => onChange(e.target.value.toUpperCase())}
                placeholder="#000000"
                maxLength={7}
                className="w-24 rounded border border-border px-2 py-1 text-sm font-mono"
              />
              {value && (
                <button
                  type="button"
                  onClick={() => { onChange(''); setIsOpen(false) }}
                  className="text-xs text-text-light hover:text-danger"
                >
                  Limpiar
                </button>
              )}
            </div>
          </div>
        )}
      </div>
    </div>
  )
}
