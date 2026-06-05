import { useState, useRef, useEffect } from 'react'
import { useApp } from '@/contexts/AppContext'

interface EspecieSelectorProps {
  disabled?: boolean
}

export default function EspecieSelector({ disabled = false }: EspecieSelectorProps) {
  const { currentEspecie, especies, isLoadingEspecies, selectEspecie } = useApp()
  const [isOpen, setIsOpen] = useState(false)
  const dropdownRef = useRef<HTMLDivElement>(null)

  const canChange = !disabled && especies.length > 1

  useEffect(() => {
    const handleClickOutside = (e: MouseEvent) => {
      if (dropdownRef.current && !dropdownRef.current.contains(e.target as Node)) {
        setIsOpen(false)
      }
    }
    document.addEventListener('mousedown', handleClickOutside)
    return () => document.removeEventListener('mousedown', handleClickOutside)
  }, [])

  if (isLoadingEspecies) {
    return <span className="text-sm text-text-light">Cargando...</span>
  }

  if (!currentEspecie) {
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
        <span className="max-w-[150px] truncate">{currentEspecie.nombre}</span>
        {canChange && (
          <svg className={`h-3.5 w-3.5 text-text-light transition-transform ${isOpen ? 'rotate-180' : ''}`} fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 9l-7 7-7-7" />
          </svg>
        )}
      </button>

      {isOpen && canChange && (
        <div className="absolute left-0 top-full z-50 mt-1 w-56 rounded-lg border border-border bg-white py-1 shadow-lg">
          {especies.map((e) => (
            <button
              key={e.id}
              onClick={() => { selectEspecie(e); setIsOpen(false) }}
              className={`flex w-full items-center px-3 py-2 text-sm transition-colors ${
                currentEspecie.id === e.id
                  ? 'bg-primary-50 text-primary-700 font-medium'
                  : 'text-text hover:bg-gray-50'
              }`}
            >
              {e.nombre}
            </button>
          ))}
        </div>
      )}
    </div>
  )
}
