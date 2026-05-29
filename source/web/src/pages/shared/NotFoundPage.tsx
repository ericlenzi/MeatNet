import { Link } from 'react-router'

export default function NotFoundPage() {
  return (
    <div className="flex flex-col items-center justify-center min-h-screen bg-background">
      <h1 className="text-6xl font-bold text-primary-700">404</h1>
      <p className="mt-4 text-xl text-text-light">Pagina no encontrada</p>
      <Link
        to="/"
        className="mt-6 rounded-lg bg-primary-600 px-6 py-2 text-white hover:bg-primary-700 transition-colors"
      >
        Volver al inicio
      </Link>
    </div>
  )
}
