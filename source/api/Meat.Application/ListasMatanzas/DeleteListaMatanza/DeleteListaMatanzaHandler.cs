using MediatR;
using Meat.Application.Shared;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.ListasMatanzas.DeleteListaMatanza
{
    public class DeleteListaMatanzaHandler : IRequestHandler<DeleteListaMatanzaRequest, DeleteListaMatanzaResponse>
    {
        private readonly MeatContext context;

        public DeleteListaMatanzaHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<DeleteListaMatanzaResponse> Handle(DeleteListaMatanzaRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.ListasMatanzas
                .Include(lm => lm.Establecimiento).ThenInclude(e => e.Empresa)
                .Include(lm => lm.Renglones)
                .Include(lm => lm.Movimientos)
                .FirstOrDefaultAsync(lm => lm.Id == request.Id
                    && lm.Establecimiento.Empresa.CodigoEmpresa == request.CodigoEmpresa, cancellationToken);

            if (entity == null)
                throw new ValidationException("La lista de matanza no existe.");

            if (entity.EstadoListaMatanzaId != EstadosListaMatanza.Borrador)
                throw new ValidationException("Solo se puede eliminar una lista en Borrador.");

            // Soft-delete en cascada de las tablas relacionadas antes del Remove principal
            this.context.ListasMatanzasDetalles.RemoveRange(entity.Renglones);
            this.context.ListasMatanzasMovimientos.RemoveRange(entity.Movimientos);
            this.context.ListasMatanzas.Remove(entity);

            await this.context.SaveChangesAsync(cancellationToken);

            return new DeleteListaMatanzaResponse();
        }
    }
}
