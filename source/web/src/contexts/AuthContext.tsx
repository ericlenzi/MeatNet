import { createContext, useContext, useState, useCallback, useEffect } from 'react'
import type { ReactNode } from 'react'
import type { CurrentUser } from '@/types'
import { login as loginService } from '@/services/auth.service'

interface AuthContextType {
  user: CurrentUser | null
  token: string | null
  isAuthenticated: boolean
  isAdmin: boolean
  isLoading: boolean
  login: (usuario: string, contraseña: string) => Promise<void>
  logout: () => void
}

const AuthContext = createContext<AuthContextType | null>(null)

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<CurrentUser | null>(null)
  const [token, setToken] = useState<string | null>(null)
  const [isLoading, setIsLoading] = useState(true)

  useEffect(() => {
    const storedToken = localStorage.getItem('token')
    const storedUser = localStorage.getItem('user')
    if (storedToken && storedUser) {
      try {
        setToken(storedToken)
        setUser(JSON.parse(storedUser) as CurrentUser)
      } catch {
        localStorage.removeItem('token')
        localStorage.removeItem('user')
      }
    }
    setIsLoading(false)
  }, [])

  const login = useCallback(async (usuario: string, contraseña: string) => {
    const response = await loginService({ Usuario: usuario, Contraseña: contraseña })
    localStorage.setItem('token', response.token)
    localStorage.setItem('user', JSON.stringify(response.currentUser))
    setToken(response.token)
    setUser(response.currentUser)
  }, [])

  const logout = useCallback(() => {
    localStorage.removeItem('token')
    localStorage.removeItem('user')
    localStorage.removeItem('currentSucursal')
    setToken(null)
    setUser(null)
  }, [])

  const isAuthenticated = !!token && !!user
  const isAdmin = user?.rolId === 'ADMIN'

  return (
    <AuthContext.Provider
      value={{ user, token, isAuthenticated, isAdmin, isLoading, login, logout }}
    >
      {children}
    </AuthContext.Provider>
  )
}

export function useAuth(): AuthContextType {
  const context = useContext(AuthContext)
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider')
  }
  return context
}
