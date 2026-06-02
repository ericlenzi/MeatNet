import { useAuth } from '@/contexts/AuthContext'
import PageHeader from '@/components/ui/PageHeader'

export default function PerfilPage() {
  const { user } = useAuth()

  return (
    <>
      <PageHeader title="Datos Personales" />

      <div className="mx-auto max-w-2xl">
        <div className="rounded-lg border border-border bg-surface p-6 shadow-sm">
          <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
            <div>
              <p className="text-xs font-medium uppercase text-text-light">Usuario</p>
              <p className="mt-1 text-sm text-text">{user?.userName}</p>
            </div>
            <div>
              <p className="text-xs font-medium uppercase text-text-light">Nombre Completo</p>
              <p className="mt-1 text-sm text-text">{user?.nombreCompleto}</p>
            </div>
            <div>
              <p className="text-xs font-medium uppercase text-text-light">Rol</p>
              <p className="mt-1 text-sm text-text">{user?.rolId}</p>
            </div>
            <div>
              <p className="text-xs font-medium uppercase text-text-light">Empresa</p>
              <p className="mt-1 text-sm text-text">{user?.nombreEmpresa} ({user?.codigoEmpresa})</p>
            </div>
          </div>
        </div>
      </div>
    </>
  )
}
