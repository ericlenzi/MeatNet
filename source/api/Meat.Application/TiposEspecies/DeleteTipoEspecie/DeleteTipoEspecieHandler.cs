using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Repositories;
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
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (entity == null)
                throw new ValidationException("El tipo de especie no existe.");

            this.context.TiposEspecies.Remove(entity);
            await this.context.SaveChangesAsync(cancellationToken);

            return new DeleteTipoEspecieResponse();
        }
    }
}
