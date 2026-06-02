import { useAuth } from '@/contexts/AuthContext'
import Modal from '@/components/ui/Modal'

interface DatosPersonalesModalProps {
  isOpen: boolean
  onClose: () => void
}

export default function DatosPersonalesModal({ isOpen, onClose }: DatosPersonalesModalProps) {
  const { user } = useAuth()

  return (
    <Modal isOpen={isOpen} onClose={onClose} title="Datos Personales" size="md">
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
        <div>
          <p className="text-xs font-medium uppercase text-text-light">Sucursal</p>
          <p className="mt-1 text-sm text-text">{user?.codigoSucursal}</p>
        </div>
      </div>
    </Modal>
  )
}
