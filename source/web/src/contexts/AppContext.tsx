import { createContext, useContext, useState, useCallback, useEffect } from 'react'
import type { ReactNode } from 'react'
import type { Sucursal } from '@/types'
import { getSucursales } from '@/services/sucursales.service'
import { useAuth } from './AuthContext'

interface AppContextType {
  currentSucursal: Sucursal | null
  sucursales: Sucursal[]
  isLoadingSucursales: boolean
  selectSucursal: (sucursal: Sucursal) => void
  loadSucursales: () => Promise<void>
}

const AppContext = createContext<AppContextType | null>(null)

export function AppProvider({ children }: { children: ReactNode }) {
  const { isAuthenticated } = useAuth()
  const [sucursales, setSucursales] = useState<Sucursal[]>([])
  const [currentSucursal, setCurrentSucursal] = useState<Sucursal | null>(null)
  const [isLoadingSucursales, setIsLoadingSucursales] = useState(false)

  const loadSucursales = useCallback(async () => {
    setIsLoadingSucursales(true)
    try {
      const response = await getSucursales()
      const activeSucursales = (response.data || []).filter((s) => s.activo)
      setSucursales(activeSucursales)

      const storedSucursal = localStorage.getItem('currentSucursal')
      if (storedSucursal) {
        try {
          const parsed = JSON.parse(storedSucursal) as Sucursal
          const found = activeSucursales.find((s) => s.id === parsed.id)
          if (found) {
            setCurrentSucursal(found)
            return
          }
        } catch {
          // ignore parse error
        }
      }

      if (activeSucursales.length > 0) {
        const user = localStorage.getItem('user')
        if (user) {
          try {
            const parsed = JSON.parse(user) as { codigoSucursal: string }
            const mainSucursal = activeSucursales.find(
              (s) => s.codigoSucursal === parsed.codigoSucursal,
            )
            if (mainSucursal) {
              setCurrentSucursal(mainSucursal)
              localStorage.setItem('currentSucursal', JSON.stringify(mainSucursal))
              return
            }
          } catch {
            // ignore
          }
        }
        setCurrentSucursal(activeSucursales[0]!)
      }
    } catch {
      // silently fail — sucursales will be empty
    } finally {
      setIsLoadingSucursales(false)
    }
  }, [])

  useEffect(() => {
    if (isAuthenticated) {
      void loadSucursales()
    } else {
      setSucursales([])
      setCurrentSucursal(null)
    }
  }, [isAuthenticated, loadSucursales])

  const selectSucursal = useCallback((sucursal: Sucursal) => {
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
