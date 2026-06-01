import { useState } from 'react'
import type { FormEvent } from 'react'
import { Navigate } from 'react-router'
import { useAuth } from '@/contexts/AuthContext'
import Input from '@/components/ui/Input'
import Button from '@/components/ui/Button'

export default function LoginPage() {
  const { isAuthenticated, login } = useAuth()
  const [usuario, setUsuario] = useState('')
  const [contraseña, setContraseña] = useState('')
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)

  if (isAuthenticated) {
    return <Navigate to="/" replace />
  }

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault()
    setError('')

    if (!usuario.trim() || !contraseña.trim()) {
      setError('Ingrese usuario y contraseña')
      return
    }

    setLoading(true)
    try {
      await login(usuario, contraseña)
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Error al iniciar sesion')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="flex min-h-screen items-center justify-center bg-gradient-to-br from-primary-800 via-primary-700 to-primary-900 px-4">
      <div className="w-full max-w-sm">
        <div className="rounded-2xl bg-surface p-8 shadow-2xl">
          {/* Logo */}
          <div className="mb-8 text-center">
            <div className="mx-auto mb-3 flex h-14 w-14 items-center justify-center rounded-xl bg-primary-600 text-xl font-bold text-white">
              MN
            </div>
            <h1 className="text-2xl font-bold text-text">MeatNet</h1>
            <p className="mt-1 text-sm text-text-light">
              Sistema de Gestión para Frigoríficos
            </p>
          </div>

          {/* Form */}
          <form onSubmit={handleSubmit} className="space-y-4">
            <Input
              label="Usuario"
              value={usuario}
              onChange={(e) => setUsuario(e.target.value)}
              placeholder="Ingrese su usuario"
              autoComplete="username"
              autoFocus
            />

            <Input
              label="Contraseña"
              type="password"
              value={contraseña}
              onChange={(e) => setContraseña(e.target.value)}
              placeholder="Ingrese su contraseña"
              autoComplete="current-password"
            />

            {error && (
              <div className="rounded-lg bg-red-50 px-4 py-3 text-sm text-danger">
                {error}
              </div>
            )}

            <Button
              type="submit"
              loading={loading}
              className="w-full"
              size="lg"
            >
              Iniciar Sesión
            </Button>
          </form>
        </div>
      </div>
    </div>
  )
}
