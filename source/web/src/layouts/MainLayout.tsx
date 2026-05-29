import { useState } from 'react'
import { Outlet } from 'react-router'
import Sidebar from '@/components/layout/Sidebar'
import Header from '@/components/layout/Header'

export default function MainLayout() {
  const [sidebarOpen, setSidebarOpen] = useState(false)

  return (
    <div className="min-h-screen bg-background">
      <Sidebar isOpen={sidebarOpen} onClose={() => setSidebarOpen(false)} />

      <div className="lg:pl-64">
        <Header onMenuToggle={() => setSidebarOpen(true)} />
        <main className="p-4 lg:p-6">
          <Outlet />
        </main>
      </div>
    </div>
  )
}
