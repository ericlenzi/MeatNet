import { useState, useRef, useEffect } from 'react'
import { useApp } from '@/contexts/AppContext'

interface EstablecimientoSelectorProps {
  disabled?: boolean
}

export default function EstablecimientoSelector({ disabled = false }: EstablecimientoSelectorProps) {
  const { currentEstablecimiento, establecimientos, isLoadingEstablecimientos, selectEstablecimiento } = useApp()
  const [isOpen, setIsOpen] = useState(false)
  const dropdownRef = useRef<HTMLDivElement>(null)

  const canChange = !disabled && establecimientos.length > 1

  useEffect(() => {
    const handleClickOutside = (e: MouseEvent) => {
      if (dropdownRef.current && !dropdownRef.current.contains(e.target as Node)) {
        setIsOpen(false)
      }
    }
    document.addEventListener('mousedown', handleClickOutside)
    return () => document.removeEventListener('mousedown', handleClickOutside)
  }, [])

  if (isLoadingEstablecimientos) {
    return <span className="text-sm text-text-light">Cargando...</span>
  }

  if (establecimientos.length === 0) {
    return null
  }

  return (
    <div className="relative" ref={dropdownRef}>
      <button
        onClick={() => canChange && setIsOpen(!isOpen)}
        className={`flex items-center gap-2 rounded-lg border border-border bg-white px-3 py-1.5 text-sm text-text transition-colors ${
          canChange ? 'hover:bg-gray-50 cursor-pointer' : 'cursor-default opacity-80'
        }`}
      >
        <span className="max-w-[150px] truncate">
          {currentEstablecimiento?.nombre || 'Seleccionar establecimiento'}
        </span>
        {canChange && (
          <svg className={`h-3.5 w-3.5 text-text-light transition-transform ${isOpen ? 'rotate-180' : ''}`} fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 9l-7 7-7-7" />
          </svg>
        )}
      </button>

      {isOpen && canChange && (
        <div className="absolute left-0 top-full z-50 mt-1 w-64 rounded-lg border border-border bg-white py-1 shadow-lg">
          {establecimientos.map((e) => (
            <button
              key={e.id}
              onClick={() => {
                selectEstablecimiento(e)
                setIsOpen(false)
              }}
              className={`flex w-full items-center gap-2 px-3 py-2 text-sm transition-colors ${
                currentEstablecimiento?.id === e.id
                  ? 'bg-primary-50 text-primary-700 font-medium'
                  : 'text-text hover:bg-gray-50'
              }`}
            >
              <span className="truncate">{e.nombre}</span>
              <span className="ml-auto text-xs text-text-light">{e.codigoEstablecimiento}</span>
            </button>
          ))}
        </div>
      )}
    </div>
  )
}
