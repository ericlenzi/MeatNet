interface BadgeProps {
  variant: 'success' | 'danger' | 'neutral' | 'info'
  children: React.ReactNode
}

const variantClasses = {
  success: 'bg-green-100 text-green-800',
  danger: 'bg-red-100 text-red-800',
  neutral: 'bg-gray-100 text-gray-800',
  info: 'bg-blue-100 text-blue-800',
}

export default function Badge({ variant, children }: BadgeProps) {
  return (
    <span
      className={`inline-flex items-center rounded-full px-2.5 py-0.5 text-xs font-medium ${variantClasses[variant]}`}
    >
      {children}
    </span>
  )
}
