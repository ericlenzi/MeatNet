using MediatR;
using Meat.Application.Shared;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.RendimientosSubproductos.DeleteRendimientoSubproducto
{
    public class DeleteRendimientoSubproductoHandler
        : IRequestHandler<DeleteRendimientoSubproductoRequest, DeleteRendimientoSubproductoResponse>
    {
        private readonly MeatContext context;

        public DeleteRendimientoSubproductoHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<DeleteRendimientoSubproductoResponse> Handle(
            DeleteRendimientoSubproductoRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.RendimientosSubproductos
                .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);
            if (entity == null)
                throw new ValidationException("El rendimiento no existe.");

            // Es un parametro de estimacion, no un hecho: no deja historia colgada. Las jornadas
            // ya analizadas se recalculan con lo que este cargado al momento de mirarlas.
            this.context.Remove(entity);
            await this.context.SaveChangesAsync(cancellationToken);

            return new DeleteRendimientoSubproductoResponse();
        }
    }
}
