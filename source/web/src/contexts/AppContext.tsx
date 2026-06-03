import { createContext, useContext, useState, useCallback, useEffect, useMemo } from 'react'
import type { ReactNode } from 'react'
import { getUsuarioSucursales, getUsuarioEstablecimientos } from '@/services/usuarios.service'
import type { UsuarioSucursalItem, UsuarioEstablecimientoItem } from '@/services/usuarios.service'
import { useAuth } from './AuthContext'

interface SucursalOption {
  id: string
  sucursalId: string
  codigoSucursal: string
  nombre: string
  color: string
  esMain: boolean
}

interface EstablecimientoOption {
  id: string           // establecimientoId
  codigoEstablecimiento: string
  nombre: string
  sucursalId: string
}

interface AppContextType {
  currentSucursal: SucursalOption | null
  sucursales: SucursalOption[]
  isLoadingSucursales: boolean
  selectSucursal: (sucursal: SucursalOption) => void
  loadSucursales: () => Promise<SucursalOption[] | undefined>
  currentEstablecimiento: EstablecimientoOption | null
  // Establecimientos de la sucursal activa (para el selector del header y hasEstablecimientos)
  establecimientos: EstablecimientoOption[]
  isLoadingEstablecimientos: boolean
  selectEstablecimiento: (establecimiento: EstablecimientoOption) => void
  // true solo si la sucursal activa tiene establecimientos asignados al usuario
  hasEstablecimientos: boolean
}

const AppContext = createContext<AppContextType | null>(null)

export function AppProvider({ children }: { children: ReactNode }) {
  const { isAuthenticated, user } = useAuth()

  const [sucursales, setSucursales] = useState<SucursalOption[]>([])
  const [currentSucursal, setCurrentSucursal] = useState<SucursalOption | null>(null)
  const [isLoadingSucursales, setIsLoadingSucursales] = useState(false)

  // Lista completa de establecimientos asignados al usuario (todas las sucursales)
  const [allEstablecimientos, setAllEstablecimientos] = useState<EstablecimientoOption[]>([])
  const [currentEstablecimiento, setCurrentEstablecimiento] = useState<EstablecimientoOption | null>(null)
  const [isLoadingEstablecimientos, setIsLoadingEstablecimientos] = useState(false)

  // Establecimientos de la sucursal activa (derivado, no es estado)
  const establecimientos = useMemo(
    () => allEstablecimientos.filter((e) => e.sucursalId === currentSucursal?.sucursalId),
    [allEstablecimientos, currentSucursal?.sucursalId],
  )

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
      return options
    } catch {
      return []
    } finally {
      setIsLoadingSucursales(false)
    }
  }, [user?.id])

  const loadEstablecimientos = useCallback(async () => {
    if (!user?.id) return null
    setIsLoadingEstablecimientos(true)
    try {
      const items: UsuarioEstablecimientoItem[] = await getUsuarioEstablecimientos(user.id)
      const options: EstablecimientoOption[] = items.map((item) => ({
        id: item.establecimientoId,
        codigoEstablecimiento: item.codigoEstablecimiento,
        nombre: item.nombre,
        sucursalId: item.sucursalId,
      }))
      setAllEstablecimientos(options)

      if (options.length === 0) {
        setCurrentEstablecimiento(null)
        localStorage.removeItem('currentEstablecimiento')
        return { options, mainSucursalId: null as string | null }
      }

      // Intentar restaurar desde localStorage
      const stored = localStorage.getItem('currentEstablecimiento')
      if (stored) {
        try {
          const parsed = JSON.parse(stored) as EstablecimientoOption
          const found = options.find((e) => e.id === parsed.id)
          if (found) {
            setCurrentEstablecimiento(found)
            return { options, mainSucursalId: found.sucursalId }
          }
        } catch {
          // ignore parse error
        }
      }

      // Default: EsMain o primero
      const mainItem = items.find((e) => e.esMain)
      const defaultOption = mainItem
        ? options.find((e) => e.id === mainItem.establecimientoId)
        : options[0]

      if (defaultOption) {
        setCurrentEstablecimiento(defaultOption)
        localStorage.setItem('currentEstablecimiento', JSON.stringify(defaultOption))
        return { options, mainSucursalId: defaultOption.sucursalId }
      }

      return { options, mainSucursalId: null as string | null }
    } catch {
      setAllEstablecimientos([])
      setCurrentEstablecimiento(null)
      return { options: [] as EstablecimientoOption[], mainSucursalId: null as string | null }
    } finally {
      setIsLoadingEstablecimientos(false)
    }
  }, [user?.id])

  useEffect(() => {
    if (!isAuthenticated || !user?.id) {
      setSucursales([])
      setCurrentSucursal(null)
      setAllEstablecimientos([])
      setCurrentEstablecimiento(null)
      localStorage.removeItem('currentSucursal')
      localStorage.removeItem('currentEstablecimiento')
      return
    }

    const initialize = async () => {
      const [sucursalOptions, estResult] = await Promise.all([
        loadSucursales(),
        loadEstablecimientos(),
      ])

      const options = sucursalOptions ?? []
      const mainSucursalId = estResult?.mainSucursalId ?? null

      if (options.length === 0) return

      // Intentar restaurar sucursal desde localStorage
      const stored = localStorage.getItem('currentSucursal')
      if (stored) {
        try {
          const parsed = JSON.parse(stored) as SucursalOption
          const found = options.find((s) => s.sucursalId === parsed.sucursalId)
          if (found) {
            setCurrentSucursal(found)
            return
          }
        } catch {
          // ignore
        }
      }

      // Default: sucursal del establecimiento EsMain → sucursal EsMain → primera
      const fromEstablecimiento = mainSucursalId
        ? options.find((s) => s.sucursalId === mainSucursalId)
        : undefined
      const esMainSucursal = options.find((s) => s.esMain)
      const defaultSucursal = fromEstablecimiento ?? esMainSucursal ?? options[0]

      if (defaultSucursal) {
        setCurrentSucursal(defaultSucursal)
        localStorage.setItem('currentSucursal', JSON.stringify(defaultSucursal))
      }
    }

    void initialize()
  }, [isAuthenticated, user?.id])   // eslint-disable-line react-hooks/exhaustive-deps

  const selectSucursal = useCallback((sucursal: SucursalOption) => {
    setCurrentSucursal(sucursal)
    localStorage.setItem('currentSucursal', JSON.stringify(sucursal))

    // Si el establecimiento activo no pertenece a la nueva sucursal, cambiarlo
    setCurrentEstablecimiento((prev) => {
      if (prev?.sucursalId === sucursal.sucursalId) return prev

      const ofSucursal = allEstablecimientos.filter((e) => e.sucursalId === sucursal.sucursalId)
      const next = ofSucursal[0] ?? null
      if (next) {
        localStorage.setItem('currentEstablecimiento', JSON.stringify(next))
      } else {
        localStorage.removeItem('currentEstablecimiento')
      }
      return next
    })
  }, [allEstablecimientos])

  const selectEstablecimiento = useCallback((establecimiento: EstablecimientoOption) => {
    setCurrentEstablecimiento(establecimiento)
    localStorage.setItem('currentEstablecimiento', JSON.stringify(establecimiento))

    // Actualizar la sucursal activa si el establecimiento pertenece a otra
    const matchingSucursal = sucursales.find((s) => s.sucursalId === establecimiento.sucursalId)
    if (matchingSucursal) {
      setCurrentSucursal((prev) => {
        if (prev?.sucursalId === matchingSucursal.sucursalId) return prev
        localStorage.setItem('currentSucursal', JSON.stringify(matchingSucursal))
        return matchingSucursal
      })
    }
  }, [sucursales])

  return (
    <AppContext.Provider
      value={{
        currentSucursal, sucursales, isLoadingSucursales, selectSucursal, loadSucursales,
        currentEstablecimiento, establecimientos, isLoadingEstablecimientos, selectEstablecimiento,
        hasEstablecimientos: establecimientos.length > 0,
      }}
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
