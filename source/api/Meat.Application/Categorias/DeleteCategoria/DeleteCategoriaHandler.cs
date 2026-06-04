using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Categorias.DeleteCategoria
{
    public class DeleteCategoriaHandler : IRequestHandler<DeleteCategoriaRequest, DeleteCategoriaResponse>
    {
        private readonly MeatContext context;

        public DeleteCategoriaHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<DeleteCategoriaResponse> Handle(DeleteCategoriaRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.Categorias
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (entity == null)
                throw new ValidationException("La categoria no existe.");

            this.context.Categorias.Remove(entity);
            await this.context.SaveChangesAsync(cancellationToken);

            return new DeleteCategoriaResponse();
        }
    }
}
