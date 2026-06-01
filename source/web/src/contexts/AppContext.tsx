import { createContext, useContext, useState, useCallback, useEffect } from 'react'
import type { ReactNode } from 'react'
import { getUsuarioSucursales } from '@/services/usuarios.service'
import type { UsuarioSucursalItem } from '@/services/usuarios.service'
import { useAuth } from './AuthContext'

interface SucursalOption {
  id: string
  sucursalId: string
  codigoSucursal: string
  nombre: string
  color: string
  esMain: boolean
}

interface AppContextType {
  currentSucursal: SucursalOption | null
  sucursales: SucursalOption[]
  isLoadingSucursales: boolean
  selectSucursal: (sucursal: SucursalOption) => void
  loadSucursales: () => Promise<void>
}

const AppContext = createContext<AppContextType | null>(null)

export function AppProvider({ children }: { children: ReactNode }) {
  const { isAuthenticated, user } = useAuth()
  const [sucursales, setSucursales] = useState<SucursalOption[]>([])
  const [currentSucursal, setCurrentSucursal] = useState<SucursalOption | null>(null)
  const [isLoadingSucursales, setIsLoadingSucursales] = useState(false)

  const loadSucursales = useCallback(async () => {
    if (!user?.id) return
    setIsLoadingSucursales(true)
    try {
      const items: UsuarioSucursalItem[] = await getUsuarioSucursales(user.id)
      const options: SucursalOption[] = items.map((item) => ({
        id: item.id,
        sucursalId: item.sucursalId,
        codigoSucursal: item.codigoSucursal,
        nombre: item.nombre,
        color: item.color || '#DAE4F0',
        esMain: item.esMain,
      }))
      setSucursales(options)

      // Try to restore from localStorage
      const storedSucursal = localStorage.getItem('currentSucursal')
      if (storedSucursal) {
        try {
          const parsed = JSON.parse(storedSucursal) as SucursalOption
          const found = options.find((s) => s.sucursalId === parsed.sucursalId)
          if (found) {
            setCurrentSucursal(found)
            return
          }
        } catch {
          // ignore parse error
        }
      }

      // Default to main sucursal or first one
      const mainSucursal = options.find((s) => s.esMain)
      if (mainSucursal) {
        setCurrentSucursal(mainSucursal)
        localStorage.setItem('currentSucursal', JSON.stringify(mainSucursal))
      } else if (options.length > 0) {
        setCurrentSucursal(options[0]!)
        localStorage.setItem('currentSucursal', JSON.stringify(options[0]))
      }
    } catch {
      // silently fail
    } finally {
      setIsLoadingSucursales(false)
    }
  }, [user?.id])

  useEffect(() => {
    if (isAuthenticated && user?.id) {
      void loadSucursales()
    } else {
      setSucursales([])
      setCurrentSucursal(null)
    }
  }, [isAuthenticated, user?.id, loadSucursales])

  const selectSucursal = useCallback((sucursal: SucursalOption) => {
    setCurrentSucursal(sucursal)
    localStorage.setItem('currentSucursal', JSON.stringify(sucursal))
  }, [])

  return (
    <AppContext.Provider
      value={{ currentSucursal, sucursales, isLoadingSucursales, selectSucursal, loadSucursales }}
    >
      {children}
    </AppContext.Provider>
  )
}

export function useApp(): AppContextType {
  const context = useContext(AppContext)
  if (!context) {
    throw new Error('useApp must be used within an AppProvider')
  }
  return context
}
