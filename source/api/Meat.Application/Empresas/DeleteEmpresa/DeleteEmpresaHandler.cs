using MediatR;
using Meat.Application.Shared;
using Meat.Domain.Shared;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Empresas.DeleteEmpresa
{
    public class DeleteEmpresaHandler : IRequestHandler<DeleteEmpresaRequest, DeleteEmpresaResponse>
    {
        private readonly MeatContext context;

        public DeleteEmpresaHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<DeleteEmpresaResponse> Handle(DeleteEmpresaRequest request, CancellationToken cancellationToken)
        {
            var empresa = await this.context.Empresas
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (empresa == null)
                throw new ValidationException("La empresa no existe");

            // Se valida que la empresa este vacia en vez de arrastrar su operacion entera:
            // borrar una empresa con datos es casi siempre un error, no una intencion.
            await this.NoDebeTenerAsync<Domain.Sucursales.Sucursal>(
                request.Id, "sucursales asignadas", cancellationToken);
            await this.NoDebeTenerAsync<Domain.Establecimientos.Establecimiento>(
                request.Id, "establecimientos asignados", cancellationToken);
            await this.NoDebeTenerAsync<Domain.Usuarios.Usuario>(
                request.Id, "usuarios asignados", cancellationToken);
            await this.NoDebeTenerAsync<Domain.Clientes.Cliente>(
                request.Id, "clientes cargados", cancellationToken);
            await this.NoDebeTenerAsync<Domain.Materiales.Material>(
                request.Id, "materiales cargados", cancellationToken);

            this.context.Empresas.Remove(empresa);
            await this.context.SaveChangesAsync(cancellationToken);

            return new DeleteEmpresaResponse();
        }

        /// <summary>
        /// Cuenta filas de otra empresa, asi que tiene que saltear el query filter: de lo
        /// contrario un SUPERADMIN parado en la empresa A veria vacia a la empresa B y la
        /// borraria con datos adentro. Como en EF Core 8 IgnoreQueryFilters apaga todos los
        /// filtros de una vez, hay que reponer a mano el de soft delete.
        /// </summary>
        private async Task NoDebeTenerAsync<TEntity>(string empresaId, string queCosa, CancellationToken cancellationToken)
            where TEntity : class, ITenantScoped
        {
            var tiene = await this.context.Set<TEntity>()
                .IgnoreQueryFilters()
                .AnyAsync(x => x.EmpresaId == empresaId
                    && EF.Property<DateTime?>(x, "FechaBaja") == null, cancellationToken);

            if (tiene)
                throw new ValidationException($"No se puede eliminar la empresa porque tiene {queCosa}.");
        }
    }
}
