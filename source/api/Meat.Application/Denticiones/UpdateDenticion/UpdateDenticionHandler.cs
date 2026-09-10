using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Denticiones.UpdateDenticion
{
    public class UpdateDenticionHandler : IRequestHandler<UpdateDenticionRequest, UpdateDenticionResponse>
    {
        private readonly MeatContext context;

        public UpdateDenticionHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<UpdateDenticionResponse> Handle(UpdateDenticionRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.Denticiones
                .FirstOrDefaultAsync(x => x.Codigo == request.Codigo, cancellationToken);

            if (entity == null)
                throw new ValidationException("La denticion no existe.");

            if (!await this.context.Especies.AnyAsync(e => e.Codigo == request.EspecieId, cancellationToken))
                throw new ValidationException("La especie indicada no existe.");

            entity.Nombre = (request.Nombre ?? string.Empty).Trim();
            entity.EspecieId = request.EspecieId;
            entity.Orden = request.Orden;
            entity.Activo = request.Activo;

            await this.context.SaveChangesAsync(cancellationToken);

            return new UpdateDenticionResponse();
        }
    }
}
