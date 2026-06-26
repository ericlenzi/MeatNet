using MediatR;
using Meat.Application.Shared;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.NumeradoresTropas.UpdateNumeradorTropa
{
    public class UpdateNumeradorTropaHandler : IRequestHandler<UpdateNumeradorTropaRequest, UpdateNumeradorTropaResponse>
    {
        private readonly MeatContext context;

        public UpdateNumeradorTropaHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<UpdateNumeradorTropaResponse> Handle(UpdateNumeradorTropaRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.NumeradoresTropas
                .FirstOrDefaultAsync(nt => nt.Id == request.Id, cancellationToken);

            if (entity == null)
                throw new ValidationException("El numerador de tropa no existe.");

            entity.UltimoNumeroTropa = request.UltimoNumeroTropa;
            await this.context.SaveChangesAsync(cancellationToken);

            return new UpdateNumeradorTropaResponse();
        }
    }
}
