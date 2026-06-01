import { Link } from 'react-router'
import { useAuth } from '@/contexts/AuthContext'
import { useApp } from '@/contexts/AppContext'

const operacionesTiles = [
  { title: 'Ingreso de Hacienda', path: '/operaciones/ingreso-hacienda', description: 'Registrar ingreso de animales' },
  { title: 'Aprobacion de Ingreso', path: '/operaciones/aprobacion-ingreso', description: 'Aprobar ingresos pendientes' },
  { title: 'Existencias en Corrales', path: '/operaciones/existencias-corrales', description: 'Consultar existencias actuales' },
  { title: 'Planificacion de Faena', path: '/operaciones/planificacion-faena', description: 'Planificar proximas faenas' },
  { title: 'Evaluacion de Faena', path: '/operaciones/evaluacion-faena', description: 'Evaluar resultados de faena' },
]

export default function DashboardPage() {
  const { user } = useAuth()
  const { currentSucursal } = useApp()

  return (
    <div>
      {/* Welcome */}
      <div className="mb-8">
        <h1 className="text-2xl font-bold text-text">
          Bienvenido, {user?.nombreCompleto || user?.userName}
        </h1>
        <p className="mt-1 text-text-light">
          Panel principal de MeatNet
        </p>
      </div>

      {/* Current sucursal card */}
      {currentSucursal && (
        <div
          className="mb-8 rounded-xl border border-border p-6"
          style={{
            background: `linear-gradient(135deg, ${currentSucursal.color || '#DAE4F0'}, ${currentSucursal.color || '#DAE4F0'}88, #ffffff)`,
          }}
        >
          <div className="flex items-center gap-4">
            <div className="flex h-12 w-12 items-center justify-center rounded-lg bg-primary-600 text-white">
              <svg className="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4" />
              </svg>
            </div>
            <div>
              <p className="text-sm font-medium text-primary-700">Sucursal activa</p>
              <p className="text-lg font-semibold text-text">{currentSucursal.nombre}</p>
              <p className="text-sm text-text-light">Codigo: {currentSucursal.codigoSucursal}</p>
            </div>
          </div>
        </div>
      )}

      {/* Operation tiles */}
      <h2 className="mb-4 text-lg font-semibold text-text">Operaciones</h2>
      <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
        {operacionesTiles.map((tile) => (
          <Link
            key={tile.path}
            to={tile.path}
            className="group rounded-xl border border-border bg-surface p-5 shadow-sm transition-all hover:border-primary-300 hover:shadow-md"
          >
            <h3 className="font-semibold text-text group-hover:text-primary-600 transition-colors">
              {tile.title}
            </h3>
            <p className="mt-1 text-sm text-text-light">{tile.description}</p>
          </Link>
        ))}
      </div>
    </div>
  )
}
