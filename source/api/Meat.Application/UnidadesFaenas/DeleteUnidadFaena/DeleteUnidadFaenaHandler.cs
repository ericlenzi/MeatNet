using MediatR;
using Meat.Application.Shared;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.UnidadesFaenas.DeleteUnidadFaena
{
    public class DeleteUnidadFaenaHandler : IRequestHandler<DeleteUnidadFaenaRequest, DeleteUnidadFaenaResponse>
    {
        private readonly MeatContext context;

        public DeleteUnidadFaenaHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<DeleteUnidadFaenaResponse> Handle(DeleteUnidadFaenaRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.UnidadesFaenas
                .FirstOrDefaultAsync(u => u.Codigo == request.Codigo, cancellationToken);
            if (entity == null)
                throw new ValidationException("La unidad de faena no existe.");

            var enUsoTipificacion = await this.context.Tipificaciones
                .AnyAsync(t => t.UnidadFaenaId == request.Codigo, cancellationToken);
            if (enUsoTipificacion)
                throw new ValidationException("No se puede eliminar la unidad de faena porque esta en uso en tipificaciones.");

            this.context.Remove(entity);
            await this.context.SaveChangesAsync(cancellationToken);

            return new DeleteUnidadFaenaResponse();
        }
    }
}
