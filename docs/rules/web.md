# Reglas — source/web (React + Vite)

## Stack
- **React 19** + **Vite 6** + **TypeScript 5.7**
- **React Router v7** para routing
- **Tailwind CSS v4** para estilos (plugin `@tailwindcss/vite`)
- **Axios** para llamadas HTTP
- Sin librerías de estado externas (Context nativo)
- Sin librerías de formularios (componentes controlados)

## Estructura del Proyecto
```
source/web/src/
├── components/
│   ├── ui/          # Componentes reutilizables: Button, Input, Select, Modal, DataTable, Badge, Spinner, Toast, SearchInput, ConfirmDialog, PageHeader
│   └── layout/      # Sidebar, Header, SucursalSelector
├── contexts/        # AuthContext (sesion JWT), AppContext (sucursal activa)
├── hooks/           # usePagination, useDebounce
├── layouts/         # MainLayout (sidebar + header + outlet)
├── pages/
│   ├── login/       # LoginPage
│   ├── dashboard/   # DashboardPage
│   ├── empresas/    # EmpresasListPage, EmpresaFormPage
│   ├── sucursales/  # SucursalesListPage, SucursalFormPage
│   ├── usuarios/    # UsuariosListPage, UsuarioFormPage
│   └── shared/      # PlaceholderPage, NotFoundPage
├── services/        # Capa de servicios API (axios-instance, auth, empresas, sucursales, usuarios, roles)
├── types/           # Interfaces TypeScript (auth, api, empresa, sucursal, usuario)
├── App.tsx          # Router y providers
├── main.tsx         # Entry point
└── index.css        # Tailwind imports + tema corporativo
```

## Convenciones React / TypeScript
- Componentes en **PascalCase** (ej: `DataTable`, `EmpresaFormPage`)
- Hooks personalizados con prefijo `use` (ej: `usePagination`, `useDebounce`)
- Props con interfaz TypeScript explicita — evitar `any`
- **Functional components** con hooks
- Archivos de pagina: `{Entidad}ListPage.tsx`, `{Entidad}FormPage.tsx`

## Estilos
- **Tailwind CSS v4** con `@theme` en `index.css` para la paleta corporativa
- Paleta principal: tonos celeste-grisaceos (primary-50 a primary-900)
- Sidebar oscuro (slate-800), fondo general (slate-100), superficie blanca

## Llamadas a la API
- Centralizadas en `services/` — un archivo por entidad
- Instancia Axios en `services/axios-instance.ts` con interceptores para JWT y manejo de 401
- Variable de entorno: `VITE_API_BASE_URL` (default: `http://localhost:5822`)
- Los endpoints de lista usan params `Filter`, `PageIndex`, `PageSize` y devuelven `{Data, TotalRows}`

## Autenticacion
- JWT Bearer almacenado en localStorage
- `AuthContext` provee: `user`, `token`, `isAuthenticated`, `isAdmin`, `login()`, `logout()`
- `isAdmin` se deriva de `user.RolId === "Admin"`
- Rutas protegidas via `ProtectedRoute` component

## Patron CRUD
Cada entidad sigue el mismo patron:
1. **ListPage**: PageHeader + SearchInput + DataTable paginada + ConfirmDialog para delete
2. **FormPage**: detecta create/edit por `useParams().id`, formulario con validacion cliente, toast on success

## Menu (Sidebar)
- **Operaciones**: visible para todos los roles
- **Datos Maestros**: visible solo si `isAdmin === true`

## Comandos frecuentes
```bash
cd source/web
npm run dev      # Levantar en http://localhost:5173
npm run build    # Build de produccion
npm run preview  # Preview del build
```
