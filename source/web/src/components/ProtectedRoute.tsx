import { Navigate, Outlet } from 'react-router'
import { useAuth } from '@/contexts/AuthContext'

export default function ProtectedRoute() {
  const { isAuthenticated, isLoading } = useAuth()

  if (isLoading) {
    return (
      <div className="flex items-center justify-center min-h-screen bg-background">
        <div className="text-text-light">Cargando...</div>
      </div>
    )
  }

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />
  }

  return <Outlet />
}
