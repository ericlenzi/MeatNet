import { createContext, useContext, useState, useCallback } from 'react'
import type { ReactNode } from 'react'

type ToastType = 'success' | 'error' | 'info' | 'warning'

interface Toast {
  id: number
  type: ToastType
  message: string
}

interface ToastContextType {
  toast: (type: ToastType, message: string) => void
}

const ToastContext = createContext<ToastContextType | null>(null)

let nextId = 0

const typeStyles: Record<ToastType, string> = {
  success: 'bg-green-600',
  error: 'bg-red-600',
  info: 'bg-blue-600',
  warning: 'bg-amber-600',
}

const typeIcons: Record<ToastType, string> = {
  success: '✓',
  error: '✗',
  info: 'ℹ',
  warning: '⚠',
}

export function ToastProvider({ children }: { children: ReactNode }) {
  const [toasts, setToasts] = useState<Toast[]>([])

  const addToast = useCallback((type: ToastType, message: string) => {
    const id = nextId++
    setToasts((prev) => [...prev, { id, type, message }])
    setTimeout(() => {
      setToasts((prev) => prev.filter((t) => t.id !== id))
    }, 4000)
  }, [])

  const removeToast = useCallback((id: number) => {
    setToasts((prev) => prev.filter((t) => t.id !== id))
  }, [])

  return (
    <ToastContext.Provider value={{ toast: addToast }}>
      {children}
      <div className="fixed top-4 right-4 z-[100] flex flex-col gap-2">
        {toasts.map((t) => (
          <div
            key={t.id}
            className={`flex items-center gap-3 rounded-lg px-4 py-3 text-sm text-white shadow-lg transition-all ${typeStyles[t.type]}`}
          >
            <span className="text-base font-bold">{typeIcons[t.type]}</span>
            <span className="flex-1">{t.message}</span>
            <button
              onClick={() => removeToast(t.id)}
              className="ml-2 text-white/80 hover:text-white"
            >
              &times;
            </button>
          </div>
        ))}
      </div>
    </ToastContext.Provider>
  )
}

export function useToast(): ToastContextType {
  const context = useContext(ToastContext)
  if (!context) {
    throw new Error('useToast must be used within a ToastProvider')
  }
  return context
}
