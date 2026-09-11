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
│   ├── ui/          # Reutilizables: Badge, Button, ColorPicker, ConfirmDialog, DataTable,
│   │                #   EspecieSelect, ImageUpload, Input, Modal, PageHeader, SearchInput,
│   │                #   Select, Spinner, StatusFilter, Toast
│   └── layout/      # Sidebar, Header, SucursalSelector, EstablecimientoSelector,
│                    #   CambiarContrasenaModal, DatosPersonalesModal
├── contexts/        # AuthContext (sesion JWT), AppContext (sucursal y establecimiento activos)
├── hooks/           # usePagination, useDebounce
├── layouts/         # MainLayout (sidebar + header + outlet)
├── pages/           # Una carpeta por entidad o proceso (~30). Ver "Patron CRUD".
├── services/        # Un archivo por entidad + axios-instance
├── types/           # Un archivo por entidad, reexportados desde types/index.ts
├── App.tsx          # Router y providers
├── main.tsx         # Entry point
└── index.css        # Tailwind imports + tema corporativo
```

> `pages/`, `services/` y `types/` **no se enumeran acá a proposito**: crecen con cada CRUD y la
> lista se desactualiza sola. La convencion de nombres alcanza para ubicarse; el detalle esta en
> `docs/BasisCRUD.md`.

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
- **Un dato que el rol no puede leer no puede tumbar la pantalla.** Si un formulario carga datos de
  referencia que la API restringe a otro rol (el caso tipico es `/Empresas`, que es del SUPERADMIN),
  ese pedido va **fuera del `Promise.all`, con su propio try/catch** y su alternativa. Si comparte
  el `await` con el resto, el 403 corta la carga entera y el formulario queda vacio: es lo que
  pasaba en Establecimientos y Sucursales, donde un ADMIN no podia editar nada por un combo
  informativo y deshabilitado.

## Autenticacion
- JWT Bearer almacenado en localStorage
- `AuthContext` provee: `user`, `token`, `isAuthenticated`, `isAdmin`, `isSuperAdmin`,
  `isLoading`, `debeCambiarContrasena`, `onContrasenaChanged()`, `login()`, `logout()`
- `isAdmin` es `user.rolId === 'ADMIN'` y `isSuperAdmin` es `user.rolId === 'SUPERADMIN'`
  (codigos en mayusculas, como los guarda la tabla `Roles`)
- Rutas protegidas via `ProtectedRoute` component

## Patron CRUD
Cada entidad sigue el mismo patron:
1. **ListPage**: PageHeader + SearchInput + DataTable paginada + ConfirmDialog para delete
2. **FormPage**: detecta create/edit por `useParams()`, formulario con validacion cliente, toast on success

El parametro de ruta depende del tipo de tabla: las entidades propias de una empresa van por
`:id` (Guid) y los **catalogos globales por `:codigo`** (ver `especies`, `roles`, `tiposEspecies`).
Los tres tipos de tabla estan en `CLAUDE.md` y en `docs/BasisCRUD.md` §4.

## Menu (Sidebar)
- **Operaciones Ciclo I / II**: requieren que el usuario tenga establecimientos asignados
- **Datos Maestros / Seguridad / Configuracion**: por item, no por grupo. Cada `NavItem` puede
  llevar `superAdminOnly: true`; el filtro es
  `item.superAdminOnly ? isSuperAdmin : isAdmin`
- Es filtrado **de menu, no de ruta**: la ruta sigue siendo navegable y quien rechaza es la API

## Comandos frecuentes
```bash
cd source/web
npm run dev      # Levantar en http://localhost:5173
npm run build    # Build de produccion
npm run preview  # Preview del build
```
