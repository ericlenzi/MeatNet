import { BrowserRouter, Routes, Route } from 'react-router'
import { AuthProvider } from '@/contexts/AuthContext'
import { AppProvider } from '@/contexts/AppContext'
import { ToastProvider } from '@/components/ui/Toast'
import ProtectedRoute from '@/components/ProtectedRoute'
import MainLayout from '@/layouts/MainLayout'
import LoginPage from '@/pages/login/LoginPage'
import DashboardPage from '@/pages/dashboard/DashboardPage'
import EmpresasListPage from '@/pages/empresas/EmpresasListPage'
import EmpresaFormPage from '@/pages/empresas/EmpresaFormPage'
import SucursalesListPage from '@/pages/sucursales/SucursalesListPage'
import SucursalFormPage from '@/pages/sucursales/SucursalFormPage'
import UsuariosListPage from '@/pages/usuarios/UsuariosListPage'
import UsuarioFormPage from '@/pages/usuarios/UsuarioFormPage'
import EstablecimientosListPage from '@/pages/establecimientos/EstablecimientosListPage'
import EstablecimientoFormPage from '@/pages/establecimientos/EstablecimientoFormPage'
import PlaceholderPage from '@/pages/shared/PlaceholderPage'
import NotFoundPage from '@/pages/shared/NotFoundPage'

function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <AppProvider>
          <ToastProvider>
            <Routes>
              <Route path="/login" element={<LoginPage />} />
              <Route element={<ProtectedRoute />}>
                <Route element={<MainLayout />}>
                  <Route index element={<DashboardPage />} />
                  {/* Operaciones */}
                  <Route path="operaciones/ingreso-hacienda" element={<PlaceholderPage title="Ingreso de Hacienda" />} />
                  <Route path="operaciones/aprobacion-ingreso" element={<PlaceholderPage title="Aprobacion de Ingreso de Hacienda" />} />
                  <Route path="operaciones/existencias-corrales" element={<PlaceholderPage title="Existencias en Corrales" />} />
                  <Route path="operaciones/planificacion-faena" element={<PlaceholderPage title="Planificacion de Faena" />} />
                  <Route path="operaciones/evaluacion-faena" element={<PlaceholderPage title="Evaluacion de Faena" />} />
                  {/* Datos Maestros */}
                  <Route path="empresas" element={<EmpresasListPage />} />
                  <Route path="empresas/create" element={<EmpresaFormPage />} />
                  <Route path="empresas/:id/edit" element={<EmpresaFormPage />} />
                  <Route path="sucursales" element={<SucursalesListPage />} />
                  <Route path="sucursales/create" element={<SucursalFormPage />} />
                  <Route path="sucursales/:id/edit" element={<SucursalFormPage />} />
                  <Route path="usuarios" element={<UsuariosListPage />} />
                  <Route path="usuarios/create" element={<UsuarioFormPage />} />
                  <Route path="usuarios/:id/edit" element={<UsuarioFormPage />} />
                  <Route path="establecimientos" element={<EstablecimientosListPage />} />
                  <Route path="establecimientos/create" element={<EstablecimientoFormPage />} />
                  <Route path="establecimientos/:id/edit" element={<EstablecimientoFormPage />} />
                </Route>
              </Route>
              <Route path="*" element={<NotFoundPage />} />
            </Routes>
          </ToastProvider>
        </AppProvider>
      </AuthProvider>
    </BrowserRouter>
  )
}

export default App
