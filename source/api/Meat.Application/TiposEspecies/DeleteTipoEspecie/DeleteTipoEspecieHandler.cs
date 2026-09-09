using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.TiposEspecies.DeleteTipoEspecie
{
    public class DeleteTipoEspecieHandler : IRequestHandler<DeleteTipoEspecieRequest, DeleteTipoEspecieResponse>
    {
        private readonly MeatContext context;

        public DeleteTipoEspecieHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<DeleteTipoEspecieResponse> Handle(DeleteTipoEspecieRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.TiposEspecies
                .FirstOrDefaultAsync(x => x.Codigo == request.Codigo, cancellationToken);

            if (entity == null)
                throw new ValidationException("El tipo de especie no existe.");

            await this.NoDebeEstarUsadoAsync<Domain.EmpresasTiposEspecies.EmpresaTipoEspecie>(
                request.Codigo, "empresas que lo tienen configurado", cancellationToken);
            await this.NoDebeEstarUsadoAsync<Domain.Tipificaciones.Tipificacion>(
                request.Codigo, "tipificaciones", cancellationToken);
            await this.NoDebeEstarUsadoAsync<Domain.ListasMatanzas.ListaMatanzaDetalle>(
                request.Codigo, "renglones de listas de matanza", cancellationToken);
            await this.NoDebeEstarUsadoAsync<Domain.IngresosHaciendas.IngresoHaciendaPesada>(
                request.Codigo, "pesadas de ingresos de hacienda", cancellationToken);
            await this.NoDebeEstarUsadoAsync<Domain.IngresosHaciendas.IngresoHaciendaUbicacion>(
                request.Codigo, "ubicaciones de ingresos de hacienda", cancellationToken);
            await this.NoDebeEstarUsadoAsync<Domain.MovimientosCamaras.MovimientoCamara>(
                request.Codigo, "movimientos de camara", cancellationToken);

            this.context.TiposEspecies.Remove(entity);
            await this.context.SaveChangesAsync(cancellationToken);

            return new DeleteTipoEspecieResponse();
        }

        /// <summary>
        /// El catalogo es comun a todas las empresas, asi que la dependencia puede estar en
        /// cualquiera de ellas. Hay que saltear el query filter para verla: el SUPERADMIN borra
        /// parado en la empresa administrativa, que no tiene operacion propia y siempre daria
        /// cero. Como IgnoreQueryFilters apaga todos los filtros de una vez, se repone a mano el
        /// de soft delete (mismo criterio que DeleteEmpresaHandler).
        /// </summary>
        private async Task NoDebeEstarUsadoAsync<TEntity>(string codigo, string queCosa, CancellationToken cancellationToken)
            where TEntity : class
        {
            var usado = await this.context.Set<TEntity>()
                .IgnoreQueryFilters()
                .AnyAsync(x => EF.Property<string>(x, "TipoEspecieId") == codigo
                    && EF.Property<DateTime?>(x, "FechaBaja") == null, cancellationToken);

            if (usado)
                throw new ValidationException($"No se puede eliminar el tipo de especie porque tiene {queCosa} asociados.");
        }
    }
}
