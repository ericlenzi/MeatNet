import { useAuth } from '@/contexts/AuthContext'
import SucursalSelector from './SucursalSelector'

interface HeaderProps {
  onMenuToggle: () => void
}

export default function Header({ onMenuToggle }: HeaderProps) {
  const { user, logout } = useAuth()

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
          <span className="hidden text-sm text-text-light sm:block">
            <span className="font-medium text-text">Empresa:</span>{' '}
            {user.nombreEmpresa} {/* ({user.codigoEmpresa}) */}
          </span>
        )}
        {' | '}
        <span className="hidden text-sm font-medium text-text sm:block">Sucursal:</span>
        <SucursalSelector />
      </div>

      {/* Right: user + logout */}
      <div className="flex items-center gap-4">
        <div className="hidden items-center gap-2 sm:flex">
          <div className="flex h-8 w-8 items-center justify-center rounded-full bg-primary-100 text-xs font-semibold text-primary-700">
            {user?.nombreCompleto?.split(' ').map((n) => n[0]).join('').slice(0, 2).toUpperCase() || 'U'}
          </div>
          <span className="text-sm font-medium text-text">
            {user?.nombreCompleto || user?.userName}
          </span>
        </div>

        <button
          onClick={logout}
          className="rounded-lg p-2 text-text-light hover:bg-red-50 hover:text-danger transition-colors"
          title="Cerrar sesion"
        >
          <svg className="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" />
          </svg>
        </button>
      </div>
    </header>
  )
}
