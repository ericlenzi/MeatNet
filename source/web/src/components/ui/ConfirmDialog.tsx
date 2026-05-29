import Modal from './Modal'
import Button from './Button'

interface ConfirmDialogProps {
  isOpen: boolean
  onConfirm: () => void
  onCancel: () => void
  title: string
  message: string
  isLoading?: boolean
}

export default function ConfirmDialog({
  isOpen,
  onConfirm,
  onCancel,
  title,
  message,
  isLoading = false,
}: ConfirmDialogProps) {
  return (
    <Modal isOpen={isOpen} onClose={onCancel} title={title} size="sm">
      <p className="mb-6 text-sm text-text-light">{message}</p>
      <div className="flex justify-end gap-3">
        <Button variant="secondary" onClick={onCancel} disabled={isLoading}>
          Cancelar
        </Button>
        <Button variant="danger" onClick={onConfirm} loading={isLoading}>
          Eliminar
        </Button>
      </div>
    </Modal>
  )
}
