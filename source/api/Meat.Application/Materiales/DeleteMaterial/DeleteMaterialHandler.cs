using MediatR;
using Meat.Application.Shared;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Materiales.DeleteMaterial
{
    public class DeleteMaterialHandler : IRequestHandler<DeleteMaterialRequest, DeleteMaterialResponse>
    {
        private readonly MeatContext context;

        public DeleteMaterialHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<DeleteMaterialResponse> Handle(DeleteMaterialRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.Materiales
                .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);
            if (entity == null)
                throw new ValidationException("El material no existe.");

            this.context.Remove(entity);
            await this.context.SaveChangesAsync(cancellationToken);

            return new DeleteMaterialResponse();
        }
    }
}
