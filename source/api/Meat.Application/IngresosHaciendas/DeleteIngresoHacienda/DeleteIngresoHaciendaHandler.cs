using MediatR;
using Meat.Application.Shared;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.IngresosHaciendas.DeleteIngresoHacienda
{
    public class DeleteIngresoHaciendaHandler : IRequestHandler<DeleteIngresoHaciendaRequest, DeleteIngresoHaciendaResponse>
    {
        private readonly MeatContext context;

        public DeleteIngresoHaciendaHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<DeleteIngresoHaciendaResponse> Handle(DeleteIngresoHaciendaRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.IngresosHaciendas
                .Include(i => i.Establecimiento).ThenInclude(e => e.Empresa)
                .Include(i => i.Pesadas)
                .Include(i => i.Ubicaciones)
                .Include(i => i.Tropas)
                .FirstOrDefaultAsync(i => i.Id == request.Id
                    && i.Establecimiento.Empresa.CodigoEmpresa == request.CodigoEmpresa, cancellationToken);

            if (entity == null)
                throw new ValidationException("El ingreso de hacienda no existe.");

            if (entity.EstadoIngresoId != EstadosIngreso.Borrador)
                throw new ValidationException("Solo se puede eliminar un ingreso en estado Borrador.");

            // Soft-delete en cascada de las tablas relacionadas antes del Remove principal
            this.context.IngresosHaciendasPesadas.RemoveRange(entity.Pesadas);
            this.context.IngresosHaciendasUbicaciones.RemoveRange(entity.Ubicaciones);
            this.context.Tropas.RemoveRange(entity.Tropas);
            this.context.IngresosHaciendas.Remove(entity);

            await this.context.SaveChangesAsync(cancellationToken);

            return new DeleteIngresoHaciendaResponse();
        }
    }
}
