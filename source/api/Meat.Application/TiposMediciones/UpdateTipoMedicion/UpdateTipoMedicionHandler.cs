using MediatR;
using Meat.Application.Shared;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.TiposMediciones.UpdateTipoMedicion
{
    public class UpdateTipoMedicionHandler : IRequestHandler<UpdateTipoMedicionRequest, UpdateTipoMedicionResponse>
    {
        private readonly MeatContext context;

        public UpdateTipoMedicionHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<UpdateTipoMedicionResponse> Handle(UpdateTipoMedicionRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.TiposMediciones
                .FirstOrDefaultAsync(x => x.Codigo == request.Codigo, cancellationToken);

            if (entity == null)
                throw new ValidationException("El tipo de medicion no existe.");

            entity.Nombre = (request.Nombre ?? string.Empty).Trim();
            entity.Activo = request.Activo;

            await this.context.SaveChangesAsync(cancellationToken);

            return new UpdateTipoMedicionResponse();
        }
    }
}
