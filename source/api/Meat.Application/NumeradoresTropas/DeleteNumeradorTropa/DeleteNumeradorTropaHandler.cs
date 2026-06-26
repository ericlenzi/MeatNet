using MediatR;
using Meat.Application.Shared;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.NumeradoresTropas.DeleteNumeradorTropa
{
    public class DeleteNumeradorTropaHandler : IRequestHandler<DeleteNumeradorTropaRequest, DeleteNumeradorTropaResponse>
    {
        private readonly MeatContext context;

        public DeleteNumeradorTropaHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<DeleteNumeradorTropaResponse> Handle(DeleteNumeradorTropaRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.NumeradoresTropas
                .FirstOrDefaultAsync(nt => nt.Id == request.Id, cancellationToken);

            if (entity == null)
                throw new ValidationException("El numerador de tropa no existe.");

            this.context.NumeradoresTropas.Remove(entity);
            await this.context.SaveChangesAsync(cancellationToken);

            return new DeleteNumeradorTropaResponse();
        }
    }
}
