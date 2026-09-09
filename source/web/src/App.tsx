import { BrowserRouter, Routes, Route } from 'react-router'
import { AuthProvider, useAuth } from '@/contexts/AuthContext'
import { AppProvider } from '@/contexts/AppContext'
import { ToastProvider } from '@/components/ui/Toast'
import ProtectedRoute from '@/components/ProtectedRoute'
import MainLayout from '@/layouts/MainLayout'
import CambiarContrasenaModal from '@/components/layout/CambiarContrasenaModal'
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
import ParametrosListPage from '@/pages/parametros/ParametrosListPage'
import ParametroFormPage from '@/pages/parametros/ParametroFormPage'
import RolesListPage from '@/pages/roles/RolesListPage'
import RolFormPage from '@/pages/roles/RolFormPage'
import EspeciesListPage from '@/pages/especies/EspeciesListPage'
import EspecieFormPage from '@/pages/especies/EspecieFormPage'
import ClientesListPage from '@/pages/clientes/ClientesListPage'
import ClienteFormPage from '@/pages/clientes/ClienteFormPage'
import TiposEspeciesListPage from '@/pages/tiposEspecies/TiposEspeciesListPage'
import TipoEspecieFormPage from '@/pages/tiposEspecies/TipoEspecieFormPage'
import CategoriasHaciendaListPage from '@/pages/categoriasHacienda/CategoriasHaciendaListPage'
import CategoriaHaciendaFormPage from '@/pages/categoriasHacienda/CategoriaHaciendaFormPage'
import EjeTipificacionListPage from '@/pages/ejesTipificacion/EjeTipificacionListPage'
import EjeTipificacionFormPage from '@/pages/ejesTipificacion/EjeTipificacionFormPage'
import UnidadesFaenasListPage from '@/pages/unidadesFaenas/UnidadesFaenasListPage'
import UnidadFaenaFormPage from '@/pages/unidadesFaenas/UnidadFaenaFormPage'
import TipificacionesListPage from '@/pages/tipificaciones/TipificacionesListPage'
import TipificacionFormPage from '@/pages/tipificaciones/TipificacionFormPage'
import MaterialesListPage from '@/pages/materiales/MaterialesListPage'
import MaterialFormPage from '@/pages/materiales/MaterialFormPage'
import DespiecesMaterialesListPage from '@/pages/despiecesMateriales/DespiecesMaterialesListPage'
import DespieceMaterialFormPage from '@/pages/despiecesMateriales/DespieceMaterialFormPage'
import NumeradoresListPage from '@/pages/numeradores/NumeradoresListPage'
import NumeradorFormPage from '@/pages/numeradores/NumeradorFormPage'
import NumeradoresTropasListPage from '@/pages/numeradoresTropas/NumeradoresTropasListPage'
import NumeradorTropaFormPage from '@/pages/numeradoresTropas/NumeradorTropaFormPage'
import AlmacenesListPage from '@/pages/almacenes/AlmacenesListPage'
import AlmacenFormPage from '@/pages/almacenes/AlmacenFormPage'
import IngresosHaciendaListPage from '@/pages/ingresosHacienda/IngresosHaciendaListPage'
import IngresoHaciendaFormPage from '@/pages/ingresosHacienda/IngresoHaciendaFormPage'
import AprobacionHaciendaListPage from '@/pages/aprobacionHacienda/AprobacionHaciendaListPage'
import ExistenciaHaciendaPage from '@/pages/existenciaHacienda/ExistenciaHaciendaPage'
import ExistenciaCamaraPage from '@/pages/existenciaCamara/ExistenciaCamaraPage'
import EvaluacionFaenaHubPage from '@/pages/evaluacionFaena/EvaluacionFaenaHubPage'
import EvaluacionFaenaPage from '@/pages/evaluacionFaena/EvaluacionFaenaPage'
import AnalisisFaenaPage from '@/pages/analisisFaena/AnalisisFaenaPage'
import TrazabilidadTropasPage from '@/pages/trazabilidadTropas/TrazabilidadTropasPage'
import PlanificacionFaenaListPage from '@/pages/planificacionFaena/PlanificacionFaenaListPage'
import ListaMatanzaFormPage from '@/pages/planificacionFaena/ListaMatanzaFormPage'
import ListaMatanzaDetailPage from '@/pages/planificacionFaena/ListaMatanzaDetailPage'
import TipificadorPage from '@/pages/ejecucionFaena/TipificadorPage'
import MonitorFaenaPage from '@/pages/ejecucionFaena/MonitorFaenaPage'
import EjecucionFaenaHubPage from '@/pages/ejecucionFaena/EjecucionFaenaHubPage'
import NotFoundPage from '@/pages/shared/NotFoundPage'

function AppRoutes() {
  const { debeCambiarContrasena, onContrasenaChanged } = useAuth()
  return (
    <>
      <Routes>
              <Route path="/login" element={<LoginPage />} />
              <Route element={<ProtectedRoute />}>
                <Route element={<MainLayout />}>
                  <Route index element={<DashboardPage />} />
                  {/* Operaciones */}
                  <Route path="operaciones/ingreso-hacienda" element={<IngresosHaciendaListPage />} />
                  <Route path="operaciones/ingreso-hacienda/create" element={<IngresoHaciendaFormPage />} />
                  <Route path="operaciones/ingreso-hacienda/:id/edit" element={<IngresoHaciendaFormPage />} />
                  <Route path="operaciones/aprobacion-ingreso" element={<AprobacionHaciendaListPage />} />
                  <Route path="operaciones/existencias-corrales" element={<ExistenciaHaciendaPage />} />
                  <Route path="operaciones/planificacion-faena" element={<PlanificacionFaenaListPage />} />
                  <Route path="operaciones/planificacion-faena/create" element={<ListaMatanzaFormPage />} />
                  <Route path="operaciones/planificacion-faena/:id/edit" element={<ListaMatanzaFormPage />} />
                  <Route path="operaciones/planificacion-faena/:id" element={<ListaMatanzaDetailPage />} />
                  <Route path="operaciones/trazabilidad-tropas" element={<TrazabilidadTropasPage />} />
                  <Route path="operaciones/ejecucion-faena" element={<EjecucionFaenaHubPage target="tipificador" />} />
                  <Route path="operaciones/ejecucion-faena/:listaMatanzaId/tipificador" element={<TipificadorPage />} />
                  <Route path="operaciones/monitor-faena" element={<EjecucionFaenaHubPage target="monitor" />} />
                  <Route path="operaciones/monitor-faena/:listaMatanzaId" element={<MonitorFaenaPage />} />
                  <Route path="operaciones/evaluacion-faena" element={<EvaluacionFaenaHubPage />} />
                  <Route path="operaciones/evaluacion-faena/:listaMatanzaId" element={<EvaluacionFaenaPage />} />
                  <Route path="operaciones/analisis-faena" element={<EvaluacionFaenaHubPage target="analisis" />} />
                  <Route path="operaciones/analisis-faena/:listaMatanzaId" element={<AnalisisFaenaPage />} />
                  <Route path="operaciones/existencia-camara" element={<ExistenciaCamaraPage />} />
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
                  <Route path="parametros" element={<ParametrosListPage />} />
                  <Route path="parametros/create" element={<ParametroFormPage />} />
                  <Route path="parametros/:id/edit" element={<ParametroFormPage />} />
                  <Route path="roles" element={<RolesListPage />} />
                  <Route path="roles/create" element={<RolFormPage />} />
                  <Route path="roles/:codigo/edit" element={<RolFormPage />} />
                  <Route path="especies" element={<EspeciesListPage />} />
                  <Route path="especies/create" element={<EspecieFormPage />} />
                  <Route path="especies/:codigo/edit" element={<EspecieFormPage />} />
                  <Route path="clientes" element={<ClientesListPage />} />
                  <Route path="clientes/create" element={<ClienteFormPage />} />
                  <Route path="clientes/:id/edit" element={<ClienteFormPage />} />
                  <Route path="numeradores-tropas" element={<NumeradoresTropasListPage />} />
                  <Route path="numeradores-tropas/create" element={<NumeradorTropaFormPage />} />
                  <Route path="numeradores-tropas/:id/edit" element={<NumeradorTropaFormPage />} />
                  <Route path="almacenes" element={<AlmacenesListPage />} />
                  <Route path="almacenes/create" element={<AlmacenFormPage />} />
                  <Route path="almacenes/:id/edit" element={<AlmacenFormPage />} />
                  <Route path="tipos-especies" element={<TiposEspeciesListPage />} />
                  <Route path="tipos-especies/create" element={<TipoEspecieFormPage />} />
                  <Route path="tipos-especies/:codigo/edit" element={<TipoEspecieFormPage />} />
                  <Route path="categorias-hacienda" element={<CategoriasHaciendaListPage />} />
                  <Route path="categorias-hacienda/create" element={<CategoriaHaciendaFormPage />} />
                  <Route path="categorias-hacienda/:id/edit" element={<CategoriaHaciendaFormPage />} />
                  <Route path="conformaciones" element={<EjeTipificacionListPage eje="conformaciones" />} />
                  <Route path="conformaciones/create" element={<EjeTipificacionFormPage eje="conformaciones" />} />
                  <Route path="conformaciones/:codigo/edit" element={<EjeTipificacionFormPage eje="conformaciones" />} />
                  <Route path="grados-engrasamiento" element={<EjeTipificacionListPage eje="grados-engrasamiento" />} />
                  <Route path="grados-engrasamiento/create" element={<EjeTipificacionFormPage eje="grados-engrasamiento" />} />
                  <Route path="grados-engrasamiento/:codigo/edit" element={<EjeTipificacionFormPage eje="grados-engrasamiento" />} />
                  <Route path="unidades-faenas" element={<UnidadesFaenasListPage />} />
                  <Route path="unidades-faenas/create" element={<UnidadFaenaFormPage />} />
                  <Route path="unidades-faenas/:id/edit" element={<UnidadFaenaFormPage />} />
                  <Route path="tipificaciones" element={<TipificacionesListPage />} />
                  <Route path="tipificaciones/create" element={<TipificacionFormPage />} />
                  <Route path="tipificaciones/:id/edit" element={<TipificacionFormPage />} />
                  <Route path="materiales" element={<MaterialesListPage />} />
                  <Route path="materiales/create" element={<MaterialFormPage />} />
                  <Route path="materiales/:id/edit" element={<MaterialFormPage />} />
                  <Route path="despieces-materiales" element={<DespiecesMaterialesListPage />} />
                  <Route path="despieces-materiales/create" element={<DespieceMaterialFormPage />} />
                  <Route path="despieces-materiales/:id/edit" element={<DespieceMaterialFormPage />} />
                  <Route path="numeradores" element={<NumeradoresListPage />} />
                  <Route path="numeradores/create" element={<NumeradorFormPage />} />
                  <Route path="numeradores/:id/edit" element={<NumeradorFormPage />} />
                </Route>
              </Route>
              <Route path="*" element={<NotFoundPage />} />
            </Routes>
      <CambiarContrasenaModal
        isOpen={debeCambiarContrasena}
        onClose={onContrasenaChanged}
        forzado
      />
    </>
  )
}

function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <AppProvider>
          <ToastProvider>
            <AppRoutes />
          </ToastProvider>
        </AppProvider>
      </AuthProvider>
    </BrowserRouter>
  )
}

export default App
