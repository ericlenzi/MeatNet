using MediatR;
using Meat.Application.Shared;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Tipificaciones.DeleteTipificacion
{
    public class DeleteTipificacionHandler : IRequestHandler<DeleteTipificacionRequest, DeleteTipificacionResponse>
    {
        private readonly MeatContext context;

        public DeleteTipificacionHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<DeleteTipificacionResponse> Handle(DeleteTipificacionRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.Tipificaciones
                .FirstOrDefaultAsync(t => t.Codigo == request.Codigo && t.CodigoEmpresa == request.CodigoEmpresa, cancellationToken);
            if (entity == null)
                throw new ValidationException("La tipificacion no existe.");

            this.context.Remove(entity);
            await this.context.SaveChangesAsync(cancellationToken);

            return new DeleteTipificacionResponse();
        }
    }
}
