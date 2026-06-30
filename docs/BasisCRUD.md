# BasisCRUD - Guia para crear un CRUD de una entidad nueva

Este documento describe los patrones establecidos en el proyecto MeatNet para implementar un CRUD completo (Create, Read, Update, Delete) de una nueva entidad, tanto en el backend (API .NET 8) como en el frontend (React + Vite + TypeScript).

> **Alcance:** esta guía aplica a las **tablas de proceso/negocio** con PK `Guid Id` (el patrón completo: Entity con Factory, Handlers CQRS, Controller, migraciones, frontend).
> **No** aplica a las **tablas de catálogo** (PK `string Codigo` + `Nombre` + `Activo`, equivalentes a enums; ej: `TipoAlmacen`, `TipoEstadoIngreso`, `TipoEstadoHacienda`). Ver la sección "Patrones de Tablas" en `CLAUDE.md` para la distinción entre ambos tipos.

---

## Indice

1. [Estructura general](#1-estructura-general)
2. [Backend - API](#2-backend---api)
   - [2.1 Entity (Dominio)](#21-entity-dominio)
   - [2.2 DbContext](#22-dbcontext)
   - [2.3 Application Layer (Handlers)](#23-application-layer-handlers)
   - [2.4 Controller](#24-controller)
   - [2.5 Migracion EF Core](#25-migracion-ef-core)
3. [Frontend - Web](#3-frontend---web)
   - [3.1 Types](#31-types)
   - [3.2 Service](#32-service)
   - [3.3 List Page](#33-list-page)
   - [3.4 Form Page](#34-form-page)
   - [3.5 Rutas (App.tsx)](#35-rutas-apptsx)
   - [3.6 Sidebar](#36-sidebar)
4. [Regla de Empresa Activa](#4-regla-de-empresa-activa)
5. [Validaciones comunes](#5-validaciones-comunes)
6. [Checklist de implementacion](#6-checklist-de-implementacion)

---

## 1. Estructura general

Cada entidad genera archivos en estas ubicaciones:

```
source/api/
  Meat.Domain/{Entidad}/                    # Entity + Factory (opcional)
  Meat.Application/{Entidad}/
    Create{Entidad}/                        # Request, Response, Handler, MapperProfile
    Get{Entidad}/                           # Request, Response, Handler, MapperProfile
    Get{Entidades}/                         # Request, Response, Handler (lista paginada)
    Update{Entidad}/                        # Request, RequestFromBody, Response, Handler, MapperProfile
    Delete{Entidad}/                        # Request, Response, Handler
  Meat/Controllers/{Entidades}Controller.cs # REST endpoints
  Meat.Repositories/MeatContext.cs          # DbSet registration

source/web/src/
  types/{entidad}.ts                        # Interfaces TypeScript
  types/index.ts                            # Re-export
  services/{entidades}.service.ts           # Funciones API (axios)
  pages/{entidades}/
    {Entidades}ListPage.tsx                 # Listado con paginacion, filtros, eliminar
    {Entidad}FormPage.tsx                   # Formulario crear/editar
  App.tsx                                   # Rutas
  components/layout/Sidebar.tsx             # Menu de navegacion
```

### Convenciones de nombres

| Contexto | Ejemplo | Regla |
|---|---|---|
| Entity / Clases C# | `Establecimiento` | PascalCase, singular |
| Namespace / Carpeta | `Establecimientos` | PascalCase, plural |
| Controller | `EstablecimientosController` | Plural + Controller |
| Archivo TS tipo | `establecimiento.ts` | lowercase, singular |
| Archivo TS servicio | `establecimientos.service.ts` | lowercase, plural |
| Pagina List | `EstablecimientosListPage.tsx` | PascalCase, plural |
| Pagina Form | `EstablecimientoFormPage.tsx` | PascalCase, singular |
| Ruta URL | `/establecimientos`, `/establecimientos/:id/edit` | lowercase, plural |

---

## 2. Backend - API

### 2.1 Entity (Dominio)

Ubicacion: `Meat.Domain/{Entidad}/{Entidad}.cs`

**Con PK Guid (patron estandar):**

```csharp
using Meat.Domain.Empresas;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meat.Domain.{Entidades}
{
    public class {Entidad}
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
        
        // Propiedades del negocio
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public bool Activo { get; set; }
        
        // FK a Empresa (OBLIGATORIO para filtro de empresa activa)
        public Guid EmpresaId { get; set; }
        public virtual Empresa Empresa { get; set; }
    }
}
```

**Con PK string (como Parametro, Rol):**

```csharp
[Key]
[DatabaseGenerated(DatabaseGeneratedOption.None)]
public string Codigo { get; set; }  // PK manual, no auto-generated
```

**Factory (opcional, para entidades con Guid PK):**

Ubicacion: `Meat.Domain/{Entidad}/{Entidad}Factory.cs`

```csharp
public static class {Entidad}Factory
{
    public static {Entidad} Create()
    {
        return new {Entidad}
        {
            Id = Guid.NewGuid(),
            Activo = true
        };
    }
}
```

> **Nota:** Si la entidad no tiene Guid como PK (ej: string Codigo), no se usa Factory. Se crea el objeto directamente en el Handler.

### 2.2 DbContext

Ubicacion: `Meat.Repositories/MeatContext.cs`

Agregar el DbSet:

```csharp
public virtual DbSet<Domain.{Entidades}.{Entidad}> {Entidades} { get; set; }
```

> El `MeatContext` aplica automaticamente:
> - **Soft delete** via shadow property `FechaBaja` (intercepta `EntityState.Deleted`)
> - **Query filter** global `WHERE FechaBaja IS NULL`

#### Indices unicos filtrados

Cuando la entidad tiene campos que deben ser **unicos** (un codigo de negocio, o la clave compuesta de una tabla de relacion), agregar un indice unico **filtrado** en la region `Indices Unicos` de `OnModelCreating` (`MeatContext.cs`).

```csharp
// Codigo unico (una columna)
modelBuilder.Entity<Domain.{Entidades}.{Entidad}>()
    .HasIndex(x => x.Codigo)
    .IsUnique()
    .HasFilter("[FechaBaja] IS NULL");

// Tabla de relacion (clave compuesta)
modelBuilder.Entity<Domain.{Relaciones}.{Relacion}>()
    .HasIndex(x => new { x.PadreId, x.HijoId })
    .IsUnique()
    .HasFilter("[FechaBaja] IS NULL");
```

> **CRITICO: el filtro `[FechaBaja] IS NULL` es obligatorio.** Sin el, los registros con soft-delete (que conservan su clave) bloquearian la reutilizacion del codigo al dar de alta uno nuevo. Con el filtro, la unicidad solo aplica a los registros activos.

Reglas:
- Indexar columnas `string` requiere que sean `nvarchar(450)`, no `nvarchar(max)`. EF lo ajusta solo al generar la migracion (puede avisar "possible data loss" por el cambio de tipo, es seguro).
- El indice unico **no reemplaza** la validacion en el handler: el `AnyAsync(...)` previo en Create/Add devuelve un `ValidationException` (HTTP 400) con mensaje amigable, mientras que el indice solo lanza un error SQL crudo (HTTP 500). Mantener ambos.
- Antes de aplicar la migracion en una base con datos, verificar que no existan duplicados activos (la creacion del indice unico falla si los hay).

### 2.3 Application Layer (Handlers)

Arquitectura: **CQRS con MediatR**. Cada operacion tiene su carpeta con Request, Response, Handler y opcionalmente MapperProfile.

#### GetAll (Lista paginada)

**Request** - hereda de `RequestListBase` (trae Filter, PageIndex, PageSize, CodigoEmpresa):

```csharp
public class Get{Entidades}Request : RequestListBase, IRequest<Get{Entidades}Response>
{
    public bool? Estado { get; set; }  // Filtro activo/inactivo
}
```

**Response** - hereda de `ResponseListBase<T>`:

```csharp
public class Get{Entidades}Response : ResponseListBase<IEnumerable<Domain.{Entidades}.{Entidad}>>
{
}
```

**Handler:**

```csharp
public async Task<Get{Entidades}Response> Handle(Get{Entidades}Request request, CancellationToken cancellationToken)
{
    IQueryable<{Entidad}> queryable = this.context.{Entidades}
        .Include(x => x.Empresa)
        .Where(x => x.Empresa.CodigoEmpresa == request.CodigoEmpresa)  // FILTRO EMPRESA
        .OrderBy(x => x.Nombre)
        .AsQueryable();

    if (!string.IsNullOrEmpty(request.Filter))
    {
        queryable = queryable.Where(x =>
            x.Nombre.Contains(request.Filter) ||
            x.Codigo.Contains(request.Filter));
    }

    if (request.Estado.HasValue)
    {
        queryable = queryable.Where(x => x.Activo == request.Estado.Value);
    }

    var totalRows = await queryable.CountAsync();
    var data = await queryable.Page(request.PageSize, request.PageIndex).ToListAsync();

    return new Get{Entidades}Response()
    {
        Data = data,
        TotalRows = totalRows
    };
}
```

> **`.Page()`** es un extension method de `QueryableExtensionMethods` que aplica `Skip/Take`.

#### GetById (Detalle)

**Request:**

```csharp
public class Get{Entidad}Request : IRequest<Get{Entidad}Response>
{
    public Guid Id { get; set; }              // o string Codigo
    [JsonIgnore]
    public string CodigoEmpresa { get; set; } // Inyectado por el Controller
}
```

**Response** - hereda de la entidad:

```csharp
public class Get{Entidad}Response : Domain.{Entidades}.{Entidad}
{
}
```

**MapperProfile:**

```csharp
public class Get{Entidad}MapperProfile : AutoMapper.Profile
{
    public Get{Entidad}MapperProfile()
    {
        this.CreateMap<{Entidad}, Get{Entidad}Response>();
    }
}
```

**Handler:**

```csharp
public async Task<Get{Entidad}Response> Handle(Get{Entidad}Request request, CancellationToken cancellationToken)
{
    var entity = await this.context.{Entidades}
        .Include(x => x.Empresa)
        .FirstOrDefaultAsync(x => x.Id == request.Id 
            && x.Empresa.CodigoEmpresa == request.CodigoEmpresa);  // FILTRO EMPRESA

    return this.mapper.Map<Get{Entidad}Response>(entity);
}
```

#### Create

**Request:**

```csharp
public class Create{Entidad}Request : IRequest<Create{Entidad}Response>
{
    [Required]
    public string Nombre { get; set; }
    // ... otras propiedades
    
    [JsonIgnore]
    public string CodigoEmpresa { get; set; }  // Inyectado por el Controller
}
```

**Response:**

```csharp
public class Create{Entidad}Response
{
    public Guid Id { get; set; }    // o string Codigo
}
```

**MapperProfile:**

```csharp
this.CreateMap<Create{Entidad}Request, {Entidad}>()
    .ForMember(dest => dest.EmpresaId, opt => opt.Ignore())
    .ForMember(dest => dest.Empresa, opt => opt.Ignore());
```

**Handler:**

```csharp
public async Task<Create{Entidad}Response> Handle(Create{Entidad}Request request, CancellationToken cancellationToken)
{
    // Resolver empresa activa
    var empresa = await this.context.Empresas
        .FirstOrDefaultAsync(e => e.CodigoEmpresa == request.CodigoEmpresa, cancellationToken);
    if (empresa == null)
        throw new ValidationException("La empresa activa no es valida.");

    var entity = {Entidad}Factory.Create();  // o new {Entidad}() si no hay Factory
    this.mapper.Map(request, entity);
    entity.EmpresaId = empresa.Id;           // ASIGNAR EMPRESA

    this.context.{Entidades}.Add(entity);
    await this.context.SaveChangesAsync();

    return new Create{Entidad}Response() { Id = entity.Id };
}
```

#### Update

**Request:**

```csharp
public class Update{Entidad}Request : IRequest<Update{Entidad}Response>
{
    public Guid Id { get; set; }
    [JsonIgnore]
    public string CodigoEmpresa { get; set; }
    
    public string Nombre { get; set; }
    // ... propiedades editables
}
```

**RequestFromBody** (lo que viene del body HTTP, sin Id ni CodigoEmpresa):

```csharp
public class Update{Entidad}RequestFromBody
{
    [Required]
    public string Nombre { get; set; }
    // ... propiedades editables
    public bool Activo { get; set; }
}
```

**MapperProfile:**

```csharp
this.CreateMap<Update{Entidad}Request, {Entidad}>()
    .ForMember(dest => dest.Id, opt => opt.Ignore())        // No sobreescribir PK
    .ForMember(dest => dest.EmpresaId, opt => opt.Ignore()) // No cambiar empresa
    .ForMember(dest => dest.Empresa, opt => opt.Ignore());
```

**Handler:**

```csharp
public async Task<Update{Entidad}Response> Handle(Update{Entidad}Request request, CancellationToken cancellationToken)
{
    var entity = await this.context.{Entidades}
        .Include(x => x.Empresa)
        .FirstOrDefaultAsync(x => x.Id == request.Id 
            && x.Empresa.CodigoEmpresa == request.CodigoEmpresa);  // FILTRO EMPRESA

    if (entity == null)
        throw new ValidationException("El/La {entidad} no existe");

    this.mapper.Map(request, entity);
    await this.context.SaveChangesAsync();

    return new Update{Entidad}Response();
}
```

#### Delete

**Request:**

```csharp
public class Delete{Entidad}Request : IRequest<Delete{Entidad}Response>
{
    public Guid Id { get; set; }
    [JsonIgnore]
    public string CodigoEmpresa { get; set; }
}
```

**Handler:**

```csharp
public async Task<Delete{Entidad}Response> Handle(Delete{Entidad}Request request, CancellationToken cancellationToken)
{
    var entity = await this.context.{Entidades}
        .Include(x => x.Empresa)
        .FirstOrDefaultAsync(x => x.Id == request.Id 
            && x.Empresa.CodigoEmpresa == request.CodigoEmpresa);  // FILTRO EMPRESA

    if (entity == null)
        throw new ValidationException("El/La {entidad} no existe");

    // Validar dependencias antes de eliminar (ej: tiene hijos asignados)
    // var tieneHijos = await this.context.{Hijos}.AnyAsync(h => h.{Entidad}Id == request.Id);
    // if (tieneHijos)
    //     throw new ValidationException("No se puede eliminar porque tiene {hijos} asignados.");

    this.context.Remove(entity);  // Soft delete via MeatContext
    await this.context.SaveChangesAsync();

    return new Delete{Entidad}Response();
}
```

### 2.4 Controller

Ubicacion: `Meat/Controllers/{Entidades}Controller.cs`

```csharp
[ApiController]
[Route("[controller]")]
[Authorize()]
public class {Entidades}Controller : MeatBaseController
{
    public {Entidades}Controller(IMediator mediator) : base(mediator) { }

    [HttpGet]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Get{Entidades}Async([FromQuery] Get{Entidades}Request request)
    {
        request.CodigoEmpresa = base.CurrentUser.CodigoEmpresa;
        return await this.Handle(request);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Get{Entidad}Async([FromRoute] Guid id) => await this.Handle(
        new Get{Entidad}Request
        {
            Id = id,
            CodigoEmpresa = base.CurrentUser.CodigoEmpresa
        }
    );

    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Create{Entidad}Async([FromBody] Create{Entidad}Request request)
    {
        request.CodigoEmpresa = base.CurrentUser.CodigoEmpresa;
        return await Handle(request);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Update{Entidad}Async(
        [FromRoute] Guid id, 
        [FromBody] Update{Entidad}RequestFromBody body) => await this.Handle(
        new Update{Entidad}Request()
        {
            Id = id,
            CodigoEmpresa = base.CurrentUser.CodigoEmpresa,
            Nombre = body.Nombre,
            // ... mapear propiedades del body
            Activo = body.Activo
        }
    );

    [HttpDelete("{id}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Delete{Entidad}ByIdAsync([FromRoute] Guid id) => await this.Handle(
        new Delete{Entidad}Request
        {
            Id = id,
            CodigoEmpresa = base.CurrentUser.CodigoEmpresa
        }
    );
}
```

> **CRITICO:** Todos los endpoints deben inyectar `base.CurrentUser.CodigoEmpresa` en el request. Nunca confiar en datos del body del cliente para filtrar por empresa.

### 2.5 Migracion EF Core

Despues de crear la entidad y registrarla en MeatContext:

```bash
dotnet ef migrations add Add{Entidad} --project Meat.Repositories --startup-project Meat
```

Si la entidad tiene `EmpresaId NOT NULL` y ya existen registros en la tabla, la migracion debe:

1. Agregar la columna como **nullable**
2. Actualizar registros existentes con SQL
3. Cambiar a **not null**
4. Crear FK e indice

```csharp
// En el metodo Up() de la migracion generada:
migrationBuilder.AddColumn<Guid>(
    name: "EmpresaId", table: "{Entidades}",
    type: "uniqueidentifier", nullable: true);

migrationBuilder.Sql(
    "UPDATE {Entidades} SET EmpresaId = (SELECT TOP 1 Id FROM Empresas WHERE TipoEmpresaId = 'PRP') WHERE EmpresaId IS NULL");

migrationBuilder.AlterColumn<Guid>(
    name: "EmpresaId", table: "{Entidades}",
    type: "uniqueidentifier", nullable: false,
    defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
```

---

## 3. Frontend - Web

### 3.1 Types

Ubicacion: `source/web/src/types/{entidad}.ts`

```typescript
export interface {Entidad} {
  id: string                  // Guid del backend llega como string
  codigo: string              // camelCase (JSON serialization)
  nombre: string
  activo: boolean
}

export interface Create{Entidad}Request {
  Codigo: string              // PascalCase (matchea el backend request)
  Nombre: string
}

export interface Update{Entidad}Request {
  Nombre: string
  Activo: boolean
}
```

Registrar en `types/index.ts`:

```typescript
export type { {Entidad}, Create{Entidad}Request, Update{Entidad}Request } from './{entidad}'
```

> **Nota:** Las propiedades de la interfaz de respuesta son **camelCase** (como devuelve JSON). Las de los request son **PascalCase** (como espera el backend).

### 3.2 Service

Ubicacion: `source/web/src/services/{entidades}.service.ts`

```typescript
import api from './axios-instance'
import type {
  PaginatedResponse,
  PaginatedRequest,
  {Entidad},
  Create{Entidad}Request,
  Update{Entidad}Request,
} from '@/types'

interface Get{Entidades}Params extends PaginatedRequest {
  Estado?: boolean
}

export async function get{Entidades}(
  params?: Get{Entidades}Params,
): Promise<PaginatedResponse<{Entidad}>> {
  const response = await api.get<PaginatedResponse<{Entidad}>>('/{Entidades}', { params })
  return response.data
}

export async function get{Entidad}(id: string): Promise<{Entidad}> {
  const response = await api.get<{Entidad}>(`/{Entidades}/${id}`)
  return response.data
}

export async function create{Entidad}(
  data: Create{Entidad}Request,
): Promise<{ id: string }> {
  const response = await api.post<{ id: string }>('/{Entidades}', data)
  return response.data
}

export async function update{Entidad}(
  id: string,
  data: Update{Entidad}Request,
): Promise<void> {
  await api.put(`/{Entidades}/${id}`, data)
}

export async function delete{Entidad}(id: string): Promise<void> {
  await api.delete(`/{Entidades}/${id}`)
}
```

> La URL del endpoint matchea el nombre del Controller (ej: `/Establecimientos`).

### 3.3 List Page

Ubicacion: `source/web/src/pages/{entidades}/{Entidades}ListPage.tsx`

Componentes usados:
- `PageHeader` - titulo + boton "Nuevo"
- `SearchInput` - busqueda con debounce
- `StatusFilter` - filtro Activo/Inactivo/Todos
- `DataTable` - tabla con columnas, paginacion, sort
- `Badge` - para mostrar estado activo/inactivo
- `ConfirmDialog` - confirmacion de eliminacion
- `usePagination` - hook para manejar paginacion
- `useDebounce` - hook para debounce del filtro
- `useToast` - notificaciones

**Estructura estandar:**

```typescript
export default function {Entidades}ListPage() {
  const navigate = useNavigate()
  const { toast } = useToast()
  const pagination = usePagination()
  const debouncedFilter = useDebounce(pagination.filter)
  const [statusFilter, setStatusFilter] = useState('active')
  const [data, setData] = useState<{Entidad}[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [sort, setSort] = useState<SortState | null>(null)
  const [deleteTarget, setDeleteTarget] = useState<{Entidad} | null>(null)
  const [isDeleting, setIsDeleting] = useState(false)

  // fetchData con useCallback + dependencias del filtro/paginacion
  // handleStatusChange resetea a pagina 0
  // handleDelete llama al service y refresca

  const columns: Column<{Entidad}>[] = [
    { key: 'codigo', header: 'Codigo', width: '150px', sortable: true },
    { key: 'nombre', header: 'Nombre', sortable: true },
    {
      key: 'activo', header: 'Estado', width: '100px', sortable: true,
      render: (value) => <Badge variant={value ? 'success' : 'danger'}>{value ? 'Activo' : 'Inactivo'}</Badge>,
    },
    {
      key: '_actions', header: '', width: '100px',
      render: (_, row) => (/* botones editar + eliminar */),
    },
  ]

  return (
    <>
      <PageHeader title="{Entidades}">
        <Button onClick={() => navigate('/{entidades}/create')}>Nuevo/a {Entidad}</Button>
      </PageHeader>
      <SearchInput ... />
      <StatusFilter ... />
      <DataTable ... />
      <ConfirmDialog ... />
    </>
  )
}
```

### 3.4 Form Page

Ubicacion: `source/web/src/pages/{entidades}/{Entidad}FormPage.tsx`

**Estructura estandar:**

```typescript
export default function {Entidad}FormPage() {
  const { id } = useParams()          // o { codigo } si PK es string
  const navigate = useNavigate()
  const { toast } = useToast()
  const { user } = useAuth()           // Para empresa activa
  const isEdit = !!id

  const [loading, setLoading] = useState(false)
  const [fetching, setFetching] = useState(true)
  const [form, setForm] = useState({
    Codigo: '',
    Nombre: '',
    Activo: true,
  })
  const [errors, setErrors] = useState<Record<string, string>>({})

  // useEffect: carga datos (y datos relacionados si los hay)
  //   - En modo crear: no carga nada (o carga combos)
  //   - En modo editar: carga la entidad por id/codigo

  // validate(): retorna boolean, setea errores
  // handleSubmit(): llama create o update segun isEdit
  // updateField(): actualiza campo y limpia error

  return (
    <>
      <PageHeader title={isEdit ? 'Editar {Entidad}' : 'Nuevo/a {Entidad}'} />
      <div className="mx-auto max-w-2xl">
        <form className="rounded-lg border border-border bg-surface p-6 shadow-sm">
          <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
            <Input label="Codigo" disabled={isEdit} ... />
            <Input label="Nombre" ... />
            {/* Select para relaciones (ej: Sucursal, Empresa) */}
          </div>

          {isEdit && (
            <div className="mt-4">
              {/* Checkbox Activo */}
            </div>
          )}

          <div className="mt-6 flex justify-end gap-3">
            <Button variant="secondary" onClick={() => navigate('/{entidades}')}>Cancelar</Button>
            <Button type="submit" loading={loading}>
              {isEdit ? 'Guardar Cambios' : 'Crear {Entidad}'}
            </Button>
          </div>
        </form>
      </div>
    </>
  )
}
```

**Reglas del formulario:**
- El campo **Codigo** / campo clave se deshabilita en modo editar (`disabled={isEdit}`)
- El campo **Empresa** siempre muestra la empresa activa y esta **deshabilitado** (`disabled`)
- El checkbox **Activo** solo se muestra en modo editar
- Validacion client-side antes de submit
- Toast de exito/error despues de la operacion
- Navegacion automatica al listado despues de guardar

### 3.5 Rutas (App.tsx)

Agregar dentro del `<Route element={<MainLayout />}>`:

```tsx
<Route path="{entidades}" element={<{Entidades}ListPage />} />
<Route path="{entidades}/create" element={<{Entidad}FormPage />} />
<Route path="{entidades}/:id/edit" element={<{Entidad}FormPage />} />
```

> Si la PK es string (como `codigo`), usar `/:codigo/edit` en vez de `/:id/edit`, y en el FormPage usar `useParams().codigo`.

### 3.6 Sidebar

Ubicacion: `source/web/src/components/layout/Sidebar.tsx`

Agregar el item al grupo correspondiente en `navGroups`:

```typescript
{ label: '{Entidad}', path: '/{entidades}', icon: icons.{icono} }
```

Iconos disponibles: `officeBuilding`, `locationMarker`, `library`, `users`, `tag`, `adjustments`, `cog`, `database`, `truck`, `calendar`, `chartBar`, `clipboardCheck`, `documentReport`, `desktopComputer`.

---

## 4. Regla de Empresa Activa

**Toda entidad DEBE estar aislada por empresa activa.** Esto significa:

### Backend
- **Cada Request** que necesita filtrar tiene `CodigoEmpresa` con `[JsonIgnore]` (nunca viene del cliente)
- **Cada Controller** inyecta `base.CurrentUser.CodigoEmpresa` antes de enviar al handler
- **Cada Handler** filtra por `x.Empresa.CodigoEmpresa == request.CodigoEmpresa`

### Filtro por tipo de relacion con Empresa

| Relacion | Filtro en Handler |
|---|---|
| FK directa (`EmpresaId`) | `.Where(x => x.Empresa.CodigoEmpresa == request.CodigoEmpresa)` |
| Via Sucursal | `.Where(x => x.Sucursal.Empresa.CodigoEmpresa == request.CodigoEmpresa)` |
| Auto-referencial (Empresa) | `.Where(x => x.CodigoEmpresa == req.CodigoEmpresa \|\| x.EmpresaPadre.CodigoEmpresa == req.CodigoEmpresa)` |

### Las 5 operaciones deben estar protegidas

| Operacion | Patron |
|---|---|
| **GetAll** | Filtro en WHERE del queryable |
| **GetById** | Condicion adicional en FirstOrDefaultAsync |
| **Create** | Resolver empresa y asignar EmpresaId |
| **Update** | Condicion adicional en FirstOrDefaultAsync (si no la encuentra, no la modifica) |
| **Delete** | Condicion adicional en FirstOrDefaultAsync (si no la encuentra, no la elimina) |

### Frontend
- El campo **Empresa** en formularios siempre esta `disabled`
- Se pre-selecciona con la empresa activa del `useAuth().user.codigoEmpresa`
- En modo editar, se fuerza la empresa activa (no se toma la del registro)

---

## 5. Validaciones comunes

| Validacion | Donde | Ejemplo |
|---|---|---|
| Codigo duplicado | Create Handler | `AnyAsync(x => x.Codigo == request.Codigo)` |
| Entidad no existe | Update/Delete Handler | `if (entity == null) throw ValidationException(...)` |
| Dependencias al eliminar | Delete Handler | `AnyAsync(hijo => hijo.{Entidad}Id == request.Id)` |
| Empresa activa invalida | Create Handler | `if (empresa == null) throw ValidationException(...)` |
| No eliminar empresa activa | Delete Handler | `if (entity.CodigoEmpresa == request.CodigoEmpresaActiva)` |
| Campos requeridos (backend) | Request | `[Required]` en propiedades |
| Campos requeridos (frontend) | FormPage validate() | `if (!form.Campo.trim()) newErrors['Campo'] = 'Requerido'` |

Excepciones personalizadas:
- `ValidationException(message)` -> HTTP 400
- `ResourceNotFoundException` -> HTTP 404
- Entity no encontrada en GetById -> retorna `null` que `MeatBaseController.Handle()` convierte en HTTP 404

---

## 6. Checklist de implementacion

### Backend

- [ ] Crear entity en `Meat.Domain/{Entidad}/{Entidad}.cs` con `EmpresaId` + navegacion `Empresa`
- [ ] (Opcional) Crear factory en `Meat.Domain/{Entidad}/{Entidad}Factory.cs`
- [ ] Registrar DbSet en `MeatContext.cs`
- [ ] (Si hay campos unicos) Agregar indice unico filtrado (`HasFilter("[FechaBaja] IS NULL")`) en la region `Indices Unicos` de `OnModelCreating`
- [ ] Crear carpeta `Meat.Application/{Entidades}/`
- [ ] Crear `Get{Entidades}/` - Request (hereda RequestListBase), Response, Handler (con filtro empresa)
- [ ] Crear `Get{Entidad}/` - Request, Response, MapperProfile, Handler (con filtro empresa)
- [ ] Crear `Create{Entidad}/` - Request (con CodigoEmpresa JsonIgnore), Response, MapperProfile, Handler
- [ ] Crear `Update{Entidad}/` - Request, RequestFromBody, Response, MapperProfile, Handler (con filtro empresa)
- [ ] Crear `Delete{Entidad}/` - Request, Response, Handler (con filtro empresa + validacion dependencias)
- [ ] Crear `{Entidades}Controller.cs` inyectando `CodigoEmpresa` en TODOS los endpoints
- [ ] Generar migracion EF Core
- [ ] Verificar que compila: `dotnet build`

### Frontend

- [ ] Crear types en `types/{entidad}.ts`
- [ ] Registrar exports en `types/index.ts`
- [ ] Crear service en `services/{entidades}.service.ts`
- [ ] Crear `pages/{entidades}/{Entidades}ListPage.tsx`
- [ ] Crear `pages/{entidades}/{Entidad}FormPage.tsx`
- [ ] Agregar rutas en `App.tsx`
- [ ] Agregar link en `Sidebar.tsx`
- [ ] Verificar que compila: `npx tsc --noEmit`
