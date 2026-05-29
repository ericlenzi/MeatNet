import { forwardRef } from 'react'
import type { InputHTMLAttributes } from 'react'

interface InputProps extends InputHTMLAttributes<HTMLInputElement> {
  label?: string
  error?: string
}

const Input = forwardRef<HTMLInputElement, InputProps>(
  ({ label, error, className = '', id, ...props }, ref) => {
    const inputId = id || label?.toLowerCase().replace(/\s+/g, '-')

    return (
      <div className="w-full">
        {label && (
          <label
            htmlFor={inputId}
            className="mb-1 block text-sm font-medium text-text"
          >
            {label}
          </label>
        )}
        <input
          ref={ref}
          id={inputId}
          className={`w-full rounded-lg border px-3 py-2 text-sm text-text placeholder:text-muted transition-colors focus:border-primary-500 focus:outline-none focus:ring-1 focus:ring-primary-500 disabled:bg-gray-50 disabled:text-text-light ${
            error ? 'border-danger' : 'border-border'
          } ${className}`}
          {...props}
        />
        {error && <p className="mt-1 text-xs text-danger">{error}</p>}
      </div>
    )
  },
)

Input.displayName = 'Input'

export default Input
