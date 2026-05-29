import type { ReactNode } from 'react'

interface PageHeaderProps {
  title: string
  children?: ReactNode
}

export default function PageHeader({ title, children }: PageHeaderProps) {
  return (
    <div className="mb-6 flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
      <h1 className="text-2xl font-bold text-text">{title}</h1>
      {children && <div className="flex items-center gap-3">{children}</div>}
    </div>
  )
}
