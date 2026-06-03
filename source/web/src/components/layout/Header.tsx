import { useState, useRef, useEffect } from 'react'
import { useAuth } from '@/contexts/AuthContext'
import { useApp } from '@/contexts/AppContext'
import SucursalSelector from './SucursalSelector'
import EstablecimientoSelector from './EstablecimientoSelector'
import EspecieSelector from './EspecieSelector'
import CambiarContrasenaModal from './CambiarContrasenaModal'
import DatosPersonalesModal from './DatosPersonalesModal'

interface HeaderProps {
  onMenuToggle: () => void
}

export default function Header({ onMenuToggle }: HeaderProps) {
  const { user, logout } = useAuth()
  const { establecimientos, isLoadingEstablecimientos, currentEstablecimiento } = useApp()
  const showEstablecimiento = !isLoadingEstablecimientos && establecimientos.length > 0
  const [userMenuOpen, setUserMenuOpen] = useState(false)
  const [cambiarContrasenaOpen, setCambiarContrasenaOpen] = useState(false)
  const [datosPersonalesOpen, setDatosPersonalesOpen] = useState(false)
  const menuRef = useRef<HTMLDivElement>(null)

  useEffect(() => {
    const handleClickOutside = (e: MouseEvent) => {
      if (menuRef.current && !menuRef.current.contains(e.target as Node)) {
        setUserMenuOpen(false)
      }
    }
    document.addEventListener('mousedown', handleClickOutside)
    return () => document.removeEventListener('mousedown', handleClickOutside)
  }, [])

  return (
    <header className="sticky top-0 z-30 flex h-16 items-center justify-between border-b border-border bg-surface px-4 shadow-sm lg:px-6">
      {/* Left: hamburger (mobile) + empresa + sucursal selector */}
      <div className="flex items-center gap-4">
        <button
          onClick={onMenuToggle}
          className="rounded-lg p-2 text-text-light hover:bg-gray-100 hover:text-text transition-colors lg:hidden"
        >
          <svg className="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 6h16M4 12h16M4 18h16" />
          </svg>
        </button>
        {user?.nombreEmpresa && (
          <span className="hidden items-center gap-1.5 text-sm text-text-light sm:flex">
            <svg className="h-4 w-4 shrink-0 text-text-light" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5} d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4" />
            </svg>
            <span className="font-medium text-text">Empresa:</span>{' '}
            {user.nombreEmpresa}
          </span>
        )}
        {' | '}
        <span className="hidden items-center gap-1.5 text-sm font-medium text-text sm:flex">
          <svg className="h-4 w-4 shrink-0 text-text-light" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5} d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z" />
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5} d="M15 11a3 3 0 11-6 0 3 3 0 016 0z" />
          </svg>
          Sucursal:
        </span>
        <SucursalSelector />
        {showEstablecimiento && (
          <>
            {' | '}
            <span className="hidden items-center gap-1.5 text-sm font-medium text-text sm:flex">
              <svg className="h-4 w-4 shrink-0 text-text-light" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5} d="M8 14v3m4-3v3m4-3v3M3 21h18M3 10h18M3 7l9-4 9 4M4 10h16v11H4V10z" />
              </svg>
              Establecimiento:
            </span>
            <EstablecimientoSelector />
          </>
        )}
        {currentEstablecimiento && (
          <>
            {' | '}
            <span className="hidden items-center gap-1.5 text-sm font-medium text-text sm:flex">
              <svg className="h-4 w-4 shrink-0 text-text-light" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5} d="M7 7h.01M7 3h5c.512 0 1.024.195 1.414.586l7 7a2 2 0 010 2.828l-7 7a2 2 0 01-2.828 0l-7-7A1.994 1.994 0 013 12V7a4 4 0 014-4z" />
              </svg>
              Especie:
            </span>
            <EspecieSelector />
          </>
        )}
      </div>

      {/* Right: user menu */}
      <div className="relative" ref={menuRef}>
        <button
          onClick={() => setUserMenuOpen(!userMenuOpen)}
          className="flex items-center gap-2 rounded-lg px-2 py-1.5 hover:bg-gray-100 transition-colors"
        >
          <div className="flex h-8 w-8 items-center justify-center rounded-full bg-primary-100 text-xs font-semibold text-primary-700">
            {user?.nombreCompleto?.split(' ').map((n) => n[0]).join('').slice(0, 2).toUpperCase() || 'U'}
          </div>
          <span className="hidden text-sm font-medium text-text sm:block">
            {user?.nombreCompleto || user?.userName}
          </span>
          <svg className={`hidden h-3.5 w-3.5 text-text-light transition-transform sm:block ${userMenuOpen ? 'rotate-180' : ''}`} fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 9l-7 7-7-7" />
          </svg>
        </button>

        {userMenuOpen && (
          <div className="absolute right-0 top-full z-50 mt-1 w-56 rounded-lg border border-border bg-white py-1 shadow-lg">
            {/* User info */}
            <div className="border-b border-border px-4 py-3">
              <p className="text-sm font-medium text-text">{user?.nombreCompleto}</p>
              <p className="text-xs text-text-light">{user?.userName}</p>
            </div>

            {/* Menu items */}
            <button
              onClick={() => {
                setUserMenuOpen(false)
                setDatosPersonalesOpen(true)
              }}
              className="flex w-full items-center gap-3 px-4 py-2.5 text-sm text-text hover:bg-gray-50 transition-colors"
            >
              <svg className="h-4 w-4 text-text-light" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5} d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
              </svg>
              Datos Personales
            </button>

            <button
              onClick={() => {
                setUserMenuOpen(false)
                setCambiarContrasenaOpen(true)
              }}
              className="flex w-full items-center gap-3 px-4 py-2.5 text-sm text-text hover:bg-gray-50 transition-colors"
            >
              <svg className="h-4 w-4 text-text-light" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5} d="M15 7a2 2 0 012 2m4 0a6 6 0 01-7.743 5.743L11 17H9v2H7v2H4a1 1 0 01-1-1v-2.586a1 1 0 01.293-.707l5.964-5.964A6 6 0 1121 9z" />
              </svg>
              Cambiar Contraseña
            </button>

            <div className="border-t border-border mt-1 pt-1">
              <button
                onClick={() => {
                  setUserMenuOpen(false)
                  logout()
                }}
                className="flex w-full items-center gap-3 px-4 py-2.5 text-sm text-danger hover:bg-red-50 transition-colors"
              >
                <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5} d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" />
                </svg>
                Salir del Sistema
              </button>
            </div>
          </div>
        )}
      </div>

      <DatosPersonalesModal
        isOpen={datosPersonalesOpen}
        onClose={() => setDatosPersonalesOpen(false)}
      />

      <CambiarContrasenaModal
        isOpen={cambiarContrasenaOpen}
        onClose={() => setCambiarContrasenaOpen(false)}
      />
    </header>
  )
}
