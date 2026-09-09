import { useRef, useState } from 'react'

interface ImageUploadProps {
  label?: string
  /** Imagen como data URI base64, o cadena vacia si no hay. */
  value: string
  onChange: (dataUri: string) => void
  /** Tope del archivo original, en KB. Debe acompanar al limite del backend. */
  maxKB?: number
  error?: string
}

const TIPOS_ACEPTADOS = ['image/png', 'image/jpeg', 'image/gif', 'image/webp']

/**
 * Carga una imagen chica y la entrega como data URI. No sube el archivo a ningun lado:
 * el valor viaja en el mismo PUT/POST que el resto del formulario y se guarda en la base.
 */
export default function ImageUpload({ label, value, onChange, maxKB = 200, error }: ImageUploadProps) {
  const inputRef = useRef<HTMLInputElement>(null)
  const [errorLocal, setErrorLocal] = useState('')

  const seleccionar = (archivo: File | undefined) => {
    setErrorLocal('')
    if (!archivo) return

    if (!TIPOS_ACEPTADOS.includes(archivo.type)) {
      setErrorLocal('El logo debe ser PNG, JPG, GIF o WEBP.')
      return
    }
    if (archivo.size > maxKB * 1024) {
      setErrorLocal(`El logo no puede superar los ${maxKB} KB.`)
      return
    }

    const lector = new FileReader()
    lector.onload = () => onChange(String(lector.result))
    lector.onerror = () => setErrorLocal('No se pudo leer el archivo.')
    lector.readAsDataURL(archivo)
  }

  const mensaje = error || errorLocal

  return (
    <div className="w-full">
      {label && <label className="mb-1 block text-sm font-medium text-text">{label}</label>}

      <div className="flex items-center gap-3">
        <div className="flex h-16 w-16 shrink-0 items-center justify-center overflow-hidden rounded-lg border border-border bg-white">
          {value ? (
            <img src={value} alt="Logo de la empresa" className="max-h-full max-w-full object-contain" />
          ) : (
            <svg className="h-6 w-6 text-text-light" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5}
                d="M3 16l5-5 4 4 3-3 6 6M4 4h16a1 1 0 011 1v14a1 1 0 01-1 1H4a1 1 0 01-1-1V5a1 1 0 011-1zm11 5h.01" />
            </svg>
          )}
        </div>

        <div className="flex flex-col gap-1">
          <div className="flex gap-2">
            <button
              type="button"
              onClick={() => inputRef.current?.click()}
              className="rounded-lg border border-border bg-white px-3 py-1.5 text-sm text-text transition-colors hover:bg-gray-50"
            >
              {value ? 'Cambiar' : 'Elegir imagen'}
            </button>
            {value && (
              <button
                type="button"
                onClick={() => { onChange(''); setErrorLocal('') }}
                className="px-2 py-1.5 text-sm text-text-light transition-colors hover:text-danger"
              >
                Quitar
              </button>
            )}
          </div>
          <span className="text-xs text-text-light">PNG, JPG, GIF o WEBP. Hasta {maxKB} KB.</span>
        </div>
      </div>

      <input
        ref={inputRef}
        type="file"
        accept={TIPOS_ACEPTADOS.join(',')}
        className="hidden"
        onChange={(e) => { seleccionar(e.target.files?.[0]); e.target.value = '' }}
      />

      {mensaje && <p className="mt-1 text-sm text-danger">{mensaje}</p>}
    </div>
  )
}
