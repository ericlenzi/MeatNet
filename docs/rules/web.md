# Reglas — source/web (React + Next.js)

## Arquitectura Next.js
- Usar **App Router** si el proyecto lo tiene (carpeta `app/`), sino Pages Router
- Componentes de servidor por defecto; `"use client"` solo cuando sea necesario
- Separar: `app/` (rutas) → `components/` (UI) → `lib/` o `services/` (lógica/API calls)

## Convenciones React / TypeScript
- Componentes en **PascalCase** (ej: `ProductCard`, `StockTable`)
- Hooks personalizados con prefijo `use` (ej: `useProducts`, `useSaleOrder`)
- Props con interfaz TypeScript explícita — nunca usar `any`
- Preferir **functional components** con hooks

## Estilos
- Usar lo que ya esté configurado en el proyecto (Tailwind / CSS Modules / styled-components)
- Si no hay preferencia definida: **Tailwind CSS**

## Llamadas a la API
- Centralizar fetch en `lib/api/` o `services/`
- Usar variables de entorno para la URL base: `process.env.NEXT_PUBLIC_API_URL`
- Manejar siempre estados: loading, error, data

## Dominio Cárnico (contexto)
- Módulos probables: Productos/Cortes, Stock, Proveedores, Ventas, Clientes
- Mostrar pesos en **kg** con 3 decimales (ej: `1.250 kg`)
- Mostrar precios en **ARS** con separador de miles

## Comandos frecuentes
```bash
npm run dev      # Levantar entorno de desarrollo
npm run build    # Build de producción
npm run lint     # Correr ESLint
npm test         # Correr tests
```
