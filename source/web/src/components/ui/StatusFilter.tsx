interface StatusOption {
  label: string
  value: string
}

interface StatusFilterProps {
  options?: StatusOption[]
  value: string
  onChange: (value: string) => void
}

const defaultOptions: StatusOption[] = [
  { label: 'Activos', value: 'active' },
  { label: 'Inactivos', value: 'inactive' },
  { label: 'Todos', value: 'all' },
]

export default function StatusFilter({
  options = defaultOptions,
  value,
  onChange,
}: StatusFilterProps) {
  return (
    <div className="inline-flex rounded-lg border border-border bg-gray-50 p-0.5">
      {options.map((opt) => (
        <button
          key={opt.value}
          type="button"
          onClick={() => onChange(opt.value)}
          className={`rounded-md px-3.5 py-1.5 text-sm font-medium transition-colors ${
            value === opt.value
              ? 'bg-white text-primary-700 shadow-sm'
              : 'text-text-light hover:text-text'
          }`}
        >
          {opt.label}
        </button>
      ))}
    </div>
  )
}
