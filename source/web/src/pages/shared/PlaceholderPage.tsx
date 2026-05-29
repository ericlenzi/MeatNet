interface PlaceholderPageProps {
  title: string
}

export default function PlaceholderPage({ title }: PlaceholderPageProps) {
  return (
    <div className="flex flex-col items-center justify-center py-20">
      <div className="rounded-lg bg-surface border border-border p-10 text-center shadow-sm">
        <h1 className="text-2xl font-semibold text-text">{title}</h1>
        <p className="mt-3 text-text-light">Pagina en construccion</p>
      </div>
    </div>
  )
}
