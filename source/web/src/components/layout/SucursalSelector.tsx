import { useState, useRef, useEffect } from 'react'
import { useLocation } from 'react-router'
import { useApp } from '@/contexts/AppContext'

export default function SucursalSelector() {
  const { currentSucursal, sucursales, isLoadingSucursales, selectSucursal } = useApp()
  const location = useLocation()
  const [isOpen, setIsOpen] = useState(false)
  const dropdownRef = useRef<HTMLDivElement>(null)

  const isDashboard = location.pathname === '/'
  const canChange = isDashboard && sucursales.length > 1

  useEffect(() => {
    const handleClickOutside = (e: MouseEvent) => {
      if (dropdownRef.current && !dropdownRef.current.contains(e.target as Node)) {
        setIsOpen(false)
      }
    }
    document.addEventListener('mousedown', handleClickOutside)
    return () => document.removeEventListener('mousedown', handleClickOutside)
  }, [])

  if (isLoadingSucursales) {
    return <span className="text-sm text-text-light">Cargando sucursales...</span>
  }

  if (sucursales.length === 0) {
    return <span className="text-sm text-text-light">Sin sucursales</span>
  }

  return (
    <div className="relative" ref={dropdownRef}>
      <button
        onClick={() => canChange && setIsOpen(!isOpen)}
        className={`flex items-center gap-2 rounded-lg border border-border bg-white px-3 py-1.5 text-sm text-text transition-colors ${
          canChange ? 'hover:bg-gray-50 cursor-pointer' : 'cursor-default opacity-80'
        }`}
      >
        <svg className="h-4 w-4 text-text-light" fill="none" viewBox="0 0 24 24" stroke="currentColor">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4" />
        </svg>
        <span className="max-w-[150px] truncate">
          {currentSucursal?.nombre || 'Seleccionar sucursal'}
        </span>
        {canChange && (
          <svg className={`h-3.5 w-3.5 text-text-light transition-transform ${isOpen ? 'rotate-180' : ''}`} fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 9l-7 7-7-7" />
          </svg>
        )}
      </button>

      {isOpen && canChange && (
        <div className="absolute left-0 top-full z-50 mt-1 w-64 rounded-lg border border-border bg-white py-1 shadow-lg">
          {sucursales.map((s) => (
            <button
              key={s.id}
              onClick={() => {
                selectSucursal(s)
                setIsOpen(false)
              }}
              className={`flex w-full items-center gap-2 px-3 py-2 text-sm transition-colors ${
                currentSucursal?.sucursalId === s.sucursalId
                  ? 'bg-primary-50 text-primary-700 font-medium'
                  : 'text-text hover:bg-gray-50'
              }`}
            >
              <span className="truncate">{s.nombre}</span>
              <span className="ml-auto text-xs text-text-light">{s.codigoSucursal}</span>
            </button>
          ))}
        </div>
      )}
    </div>
  )
}
