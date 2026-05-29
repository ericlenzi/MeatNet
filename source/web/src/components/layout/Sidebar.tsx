import { useState } from 'react'
import { NavLink } from 'react-router'
import { useAuth } from '@/contexts/AuthContext'

interface SidebarProps {
  isOpen: boolean
  onClose: () => void
}

interface NavItem {
  label: string
  path: string
}

interface NavGroup {
  title: string
  items: NavItem[]
  adminOnly?: boolean
}

const navGroups: NavGroup[] = [
  {
    title: 'Operaciones',
    items: [
      { label: 'Ingreso de Hacienda', path: '/operaciones/ingreso-hacienda' },
      { label: 'Aprobacion de Ingreso', path: '/operaciones/aprobacion-ingreso' },
      { label: 'Existencias en Corrales', path: '/operaciones/existencias-corrales' },
      { label: 'Planificacion de Faena', path: '/operaciones/planificacion-faena' },
      { label: 'Evaluacion de Faena', path: '/operaciones/evaluacion-faena' },
    ],
  },
  {
    title: 'Datos Maestros',
    adminOnly: true,
    items: [
      { label: 'Empresas', path: '/empresas' },
      { label: 'Sucursales', path: '/sucursales' },
      { label: 'Usuarios', path: '/usuarios' },
      { label: 'Establecimientos', path: '/establecimientos' },
    ],
  },
]

export default function Sidebar({ isOpen, onClose }: SidebarProps) {
  const { isAdmin } = useAuth()
  const [expandedGroups, setExpandedGroups] = useState<Record<string, boolean>>({
    Operaciones: true,
    'Datos Maestros': true,
  })

  const toggleGroup = (title: string) => {
    setExpandedGroups((prev) => ({ ...prev, [title]: !prev[title] }))
  }

  const filteredGroups = navGroups.filter(
    (group) => !group.adminOnly || isAdmin,
  )

  const sidebarContent = (
    <div className="flex h-full flex-col bg-sidebar text-white">
      {/* Logo */}
      <div className="flex h-16 items-center gap-3 border-b border-white/10 px-5">
        <div className="flex h-9 w-9 items-center justify-center rounded-lg bg-primary-600 font-bold text-sm">
          MN
        </div>
        <span className="text-lg font-semibold tracking-tight">MeatNet</span>
      </div>

      {/* Navigation */}
      <nav className="flex-1 overflow-y-auto px-3 py-4">
        {filteredGroups.map((group) => (
          <div key={group.title} className="mb-2">
            <button
              onClick={() => toggleGroup(group.title)}
              className="flex w-full items-center justify-between rounded-lg px-3 py-2 text-xs font-semibold uppercase tracking-wider text-white/50 hover:text-white/80 transition-colors"
            >
              {group.title}
              <svg
                className={`h-3.5 w-3.5 transition-transform ${expandedGroups[group.title] ? 'rotate-180' : ''}`}
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
              >
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 9l-7 7-7-7" />
              </svg>
            </button>
            {expandedGroups[group.title] && (
              <div className="mt-1 space-y-0.5">
                {group.items.map((item) => (
                  <NavLink
                    key={item.path}
                    to={item.path}
                    onClick={onClose}
                    className={({ isActive }) =>
                      `flex items-center rounded-lg px-3 py-2 text-sm transition-colors ${
                        isActive
                          ? 'bg-sidebar-active text-white font-medium'
                          : 'text-white/70 hover:bg-sidebar-hover hover:text-white'
                      }`
                    }
                  >
                    {item.label}
                  </NavLink>
                ))}
              </div>
            )}
          </div>
        ))}
      </nav>
    </div>
  )

  return (
    <>
      {/* Desktop sidebar */}
      <aside className="hidden lg:fixed lg:inset-y-0 lg:flex lg:w-64 lg:flex-col">
        {sidebarContent}
      </aside>

      {/* Mobile sidebar overlay */}
      {isOpen && (
        <div className="fixed inset-0 z-40 lg:hidden">
          <div className="fixed inset-0 bg-black/50" onClick={onClose} />
          <aside className="fixed inset-y-0 left-0 z-50 w-64">
            {sidebarContent}
          </aside>
        </div>
      )}
    </>
  )
}
