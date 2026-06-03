import { useState } from 'react'
import type { ReactNode } from 'react'
import { NavLink, Link } from 'react-router'
import { useAuth } from '@/contexts/AuthContext'
import { useApp } from '@/contexts/AppContext'

interface SidebarProps {
  isOpen: boolean
  onClose: () => void
  isCollapsed: boolean
  onToggleCollapse: () => void
}

interface NavItem {
  label: string
  path: string
  icon: ReactNode
}

interface NavGroup {
  title: string
  icon?: ReactNode
  items?: NavItem[]
  children?: NavGroup[]
  adminOnly?: boolean
}

// --- Icons ---
const icons = {
  chevronDoubleRight: (
    <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5} d="M13 5l7 7l-7 7M5 5l7 7l-7 7" />
    </svg>
  ),
  truck: (
    <svg className="h-4 w-4.5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5} d="M13 16V6a1 1 0 00-1-1H4a1 1 0 00-1 1v10a1 1 0 001 1h1m8-1a1 1 0 01-1 1H9m4-1V8a1 1 0 011-1h2.586a1 1 0 01.707.293l3.414 3.414a1 1 0 01.293.707V16a1 1 0 01-1 1h-1m-6-1a1 1 0 001 1h1M5 17a2 2 0 104 0m-4 0a2 2 0 114 0m6 0a2 2 0 104 0m-4 0a2 2 0 114 0" />
    </svg>
  ),
  clipboardCheck: (
    <svg className="h-4 w-4.5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5} d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2m-6 9l2 2 4-4" />
    </svg>
  ),
  chartBar: (
    <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5} d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
    </svg>
  ),
  calendar: (
    <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5} d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
    </svg>
  ),
  desktopComputer: (
    <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5} d="M9.75 17L9 20l-1 1h8l-1-1l-.75-3M3 13h18M5 17h14a2 2 0 0 0 2-2V5a2 2 0 0 0-2-2H5a2 2 0 0 0-2 2v10a2 2 0 0 0 2 2z" />
    </svg>
  ),
  documentReport: (
    <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5} d="M9 17v-2m3 2v-4m3 4v-6m2 10H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
    </svg>
  ),
  cog: (
    <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5} d="M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.066 2.573c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.573 1.066c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.066-2.573c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z" />
      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5} d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
    </svg>
  ),
  database: (
    <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5} d="M4 7v10c0 2.21 3.582 4 8 4s8-1.79 8-4V7M4 7c0 2.21 3.582 4 8 4s8-1.79 8-4M4 7c0-2.21 3.582-4 8-4s8 1.79 8 4m0 5c0 2.21-3.582 4-8 4s-8-1.79-8-4" />
    </svg>
  ),
  officeBuilding: (
    <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5} d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4" />
    </svg>
  ),
  locationMarker: (
    <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5} d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z" />
      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5} d="M15 11a3 3 0 11-6 0 3 3 0 016 0z" />
    </svg>
  ),
  library: (
    <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5} d="M8 14v3m4-3v3m4-3v3M3 21h18M3 10h18M3 7l9-4 9 4M4 10h16v11H4V10z" />
    </svg>
  ),
  users: (
    <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5} d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197M13 7a4 4 0 11-8 0 4 4 0 018 0z" />
    </svg>
  ),
  adjustments: (
    <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5} d="M12 6V4m0 2a2 2 0 100 4m0-4a2 2 0 110 4m-6 8a2 2 0 100-4m0 4a2 2 0 110-4m0 4v2m0-6V4m6 6v10m6-2a2 2 0 100-4m0 4a2 2 0 110-4m0 4v2m0-6V4" />
    </svg>
  ),
  tag: (
    <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5} d="M7 7h.01M7 3h5c.512 0 1.024.195 1.414.586l7 7a2 2 0 010 2.828l-7 7a2 2 0 01-2.828 0l-7-7A1.994 1.994 0 013 12V7a4 4 0 014-4z" />
    </svg>
  ),
  logout: (
    <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5} d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" />
    </svg>
  ),
  collapseLeft: (
    <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5} d="M11 19l-7-7 7-7M19 19l-7-7 7-7" />
    </svg>
  ),
  expandRight: (
    <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5} d="M13 5l7 7-7 7M5 5l7 7-7 7" />
    </svg>
  ),
}

const navGroups: NavGroup[] = [
  {
    title: 'Operaciones Ciclo I',
    icon: icons.chevronDoubleRight,
    items: [
      { label: 'Ingreso de Hacienda', path: '/operaciones/ingreso-hacienda', icon: icons.truck },
      { label: 'Aprobación de Ingreso', path: '/operaciones/aprobacion-ingreso', icon: icons.clipboardCheck },
      { label: 'Existencias en Corrales', path: '/operaciones/existencias-corrales', icon: icons.chartBar },
      { label: 'Planificación de Faena', path: '/operaciones/planificacion-faena', icon: icons.calendar },
      { label: 'Monitor de Faena', path: '/operaciones/monitor-faena', icon: icons.desktopComputer },
      { label: 'Evaluación de Faena', path: '/operaciones/evaluacion-faena', icon: icons.documentReport },
    ],
  },
  {
    title: 'Operaciones Ciclo II',
    adminOnly: true,
    icon: icons.chevronDoubleRight,
    items: [],
  },
  {
    title: 'Administración',
    adminOnly: true,
    icon: icons.chevronDoubleRight,
    children: [
      {
        title: 'Datos Maestros',
        icon: icons.database,
        items: [
          { label: 'Empresas', path: '/empresas', icon: icons.officeBuilding },
          { label: 'Sucursales', path: '/sucursales', icon: icons.locationMarker },
          { label: 'Establecimientos', path: '/establecimientos', icon: icons.library },
          { label: 'Especies', path: '/especies', icon: icons.tag },
          { label: 'Clientes', path: '/clientes', icon: icons.users },
        ],
      },
      {
        title: 'Seguridad',
        icon: icons.adjustments,
        items: [
          { label: 'Usuarios', path: '/usuarios', icon: icons.users },
          { label: 'Roles', path: '/roles', icon: icons.tag },
        ],
      },
      {
        title: 'Configuración',
        icon: icons.cog,
        items: [
          { label: 'Parámetros Generales', path: '/parametros', icon: icons.adjustments },
        ],
      },
    ],
  },
]

// --- Components ---

function ChevronIcon({ expanded }: { expanded: boolean }) {
  return (
    <svg
      className={`h-3.5 w-3.5 transition-transform ${expanded ? 'rotate-180' : ''}`}
      fill="none"
      viewBox="0 0 24 24"
      stroke="currentColor"
    >
      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 9l-7 7-7-7" />
    </svg>
  )
}

function NavGroupItems({ items, onClose }: { items: NavItem[]; onClose: () => void }) {
  return (
    <div className="mt-1 space-y-0.5">
      {items.map((item) => (
        <NavLink
          key={item.path}
          to={item.path}
          onClick={onClose}
          className={({ isActive }) =>
            `flex items-center gap-2.5 rounded-lg px-3 py-2 text-sm transition-colors ${
              isActive
                ? 'bg-sidebar-active text-white font-medium'
                : 'text-blue-300 hover:bg-sidebar-hover hover:text-white'
            }`
          }
        >
          <span className="shrink-0 opacity-70">{item.icon}</span>
          {item.label}
        </NavLink>
      ))}
    </div>
  )
}

function NavSubGroup({
  group,
  expanded,
  onToggle,
  onClose,
}: {
  group: NavGroup
  expanded: Record<string, boolean>
  onToggle: (title: string) => void
  onClose: () => void
}) {
  const isExpanded = !!expanded[group.title]

  return (
    <div className="ml-2 mt-1">
      <button
        onClick={() => onToggle(group.title)}
        className="flex w-full items-center justify-between rounded-lg px-3 py-1.5 text-xs font-medium text-white/60 hover:text-white/90 transition-colors"
      >
        <span className="flex items-center gap-2">
          {group.icon && <span className="shrink-0 opacity-60">{group.icon}</span>}
          {group.title}
        </span>
        <ChevronIcon expanded={isExpanded} />
      </button>
      {isExpanded && group.items && (
        <div className="ml-2">
          <NavGroupItems items={group.items} onClose={onClose} />
        </div>
      )}
    </div>
  )
}

function CollapsedNavItem({ item, onClose }: { item: NavItem; onClose: () => void }) {
  return (
    <NavLink
      to={item.path}
      onClick={onClose}
      title={item.label}
      className={({ isActive }) =>
        `flex items-center justify-center rounded-lg p-2.5 transition-colors ${
          isActive
            ? 'bg-sidebar-active text-white'
            : 'text-blue-300 hover:bg-sidebar-hover hover:text-white'
        }`
      }
    >
      <span className="h-5 w-5 [&>svg]:h-5 [&>svg]:w-5">{item.icon}</span>
    </NavLink>
  )
}

const roleLabels: Record<string, string> = {
  ADMIN: 'Administrador',
}

export default function Sidebar({ isOpen, onClose, isCollapsed, onToggleCollapse }: SidebarProps) {
  const { isAdmin, user, logout } = useAuth()
  const { hasEstablecimientos } = useApp()
  const [expandedGroups, setExpandedGroups] = useState<Record<string, boolean>>({
    'Operaciones Ciclo I': false,
    'Operaciones Ciclo II': false,
    'Administración': false,
    'Datos Maestros': false,
    'Seguridad': false,
  })

  const toggleGroup = (title: string) => {
    setExpandedGroups((prev) => ({ ...prev, [title]: !prev[title] }))
  }

  const filteredGroups = navGroups.filter((group) => {
    if (group.title === 'Operaciones Ciclo I' || group.title === 'Operaciones Ciclo II') {
      return hasEstablecimientos
    }
    return !group.adminOnly || isAdmin
  })

  const allItems = filteredGroups.flatMap((group) => [
    ...(group.items ?? []),
    ...(group.children ?? []).flatMap((child) => child.items ?? []),
  ])

  const userInitials =
    user?.nombreCompleto
      ?.split(' ')
      .map((n) => n[0])
      .join('')
      .slice(0, 2)
      .toUpperCase() ?? 'U'

  const roleLabel = user?.rolId ? (roleLabels[user.rolId] ?? user.rolId) : ''

  const sidebarContent = (
    <div className="flex h-full flex-col bg-sidebar text-white">
      {/* Logo */}
      <Link
        to="/"
        onClick={onClose}
        className={`flex h-16 shrink-0 items-center border-b border-white/10 hover:bg-sidebar-hover transition-colors ${
          isCollapsed ? 'justify-center px-2' : 'gap-3 px-5'
        }`}
      >
        <div className="flex h-9 w-9 shrink-0 items-center justify-center rounded-lg bg-primary-600 font-bold text-sm">
          MN
        </div>
        {!isCollapsed && (
          <span className="text-lg font-semibold tracking-tight">MeatNet</span>
        )}
      </Link>

      {/* Navigation */}
      <nav className="flex-1 overflow-y-auto px-2 py-3">
        {isCollapsed ? (
          <div className="space-y-1">
            {allItems.map((item) => (
              <CollapsedNavItem key={item.path} item={item} onClose={onClose} />
            ))}
          </div>
        ) : (
          filteredGroups.map((group) => (
            <div key={group.title} className="mb-2">
              <button
                onClick={() => toggleGroup(group.title)}
                className="flex w-full items-center justify-between rounded-lg px-3 py-2 text-xs font-semibold uppercase tracking-wider text-white/50 hover:text-white/80 transition-colors"
              >
                <span className="flex items-center gap-2">
                  {group.icon && <span className="opacity-60">{group.icon}</span>}
                  {group.title}
                </span>
                <ChevronIcon expanded={!!expandedGroups[group.title]} />
              </button>

              {expandedGroups[group.title] && (
                <>
                  {group.items && group.items.length > 0 && (
                    <NavGroupItems items={group.items} onClose={onClose} />
                  )}
                  {group.children && group.children.map((child) => (
                    <NavSubGroup
                      key={child.title}
                      group={child}
                      expanded={expandedGroups}
                      onToggle={toggleGroup}
                      onClose={onClose}
                    />
                  ))}
                </>
              )}
            </div>
          ))
        )}
      </nav>

      {/* Collapse toggle */}
      <div className="hidden lg:flex shrink-0 justify-end border-t border-white/10 px-2 py-1.5">
        <button
          onClick={onToggleCollapse}
          title={isCollapsed ? 'Expandir sidebar' : 'Colapsar sidebar'}
          className="flex items-center justify-center rounded-lg p-1.5 text-white/40 hover:bg-sidebar-hover hover:text-white transition-colors"
        >
          {isCollapsed ? icons.expandRight : icons.collapseLeft}
        </button>
      </div>

      {/* User footer */}
      <div className={`shrink-0 border-t border-white/10 p-3 ${isCollapsed ? 'flex flex-col items-center gap-2' : ''}`}>
        {isCollapsed ? (
          <>
            <div
              title={user?.nombreCompleto}
              className="flex h-9 w-9 items-center justify-center rounded-full bg-primary-600/40 text-xs font-semibold text-white"
            >
              {userInitials}
            </div>
            <button
              onClick={logout}
              title="Salir del Sistema"
              className="flex items-center justify-center rounded-lg p-1.5 text-blue-300 hover:bg-sidebar-hover hover:text-white transition-colors"
            >
              {icons.logout}
            </button>
          </>
        ) : (
          <div className="flex items-center gap-3">
            <div className="flex h-9 w-9 shrink-0 items-center justify-center rounded-full bg-primary-600/40 text-xs font-semibold text-white">
              {userInitials}
            </div>
            <div className="flex-1 min-w-0">
              <p className="truncate text-sm font-medium text-white">{user?.nombreCompleto}</p>
              <p className="truncate text-xs text-blue-300">{roleLabel}</p>
            </div>
            <button
              onClick={logout}
              title="Salir del Sistema"
              className="shrink-0 rounded-lg p-1.5 text-blue-300 hover:bg-sidebar-hover hover:text-white transition-colors"
            >
              {icons.logout}
            </button>
          </div>
        )}
      </div>
    </div>
  )

  return (
    <>
      {/* Desktop sidebar */}
      <aside
        className={`hidden lg:fixed lg:inset-y-0 lg:flex lg:flex-col transition-all duration-300 ${
          isCollapsed ? 'lg:w-16' : 'lg:w-64'
        }`}
      >
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
