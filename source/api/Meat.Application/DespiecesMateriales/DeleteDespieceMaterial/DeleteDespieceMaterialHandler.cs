using MediatR;
using Meat.Application.Shared;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.DespiecesMateriales.DeleteDespieceMaterial
{
    public class DeleteDespieceMaterialHandler : IRequestHandler<DeleteDespieceMaterialRequest, DeleteDespieceMaterialResponse>
    {
        private readonly MeatContext context;

        public DeleteDespieceMaterialHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<DeleteDespieceMaterialResponse> Handle(DeleteDespieceMaterialRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.DespiecesMateriales
                .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken);
            if (entity == null)
                throw new ValidationException("El despiece no existe.");

            this.context.Remove(entity);
            await this.context.SaveChangesAsync(cancellationToken);

            return new DeleteDespieceMaterialResponse();
        }
    }
}
