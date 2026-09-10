using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.TiposContusiones.UpdateTipoContusion
{
    public class UpdateTipoContusionHandler : IRequestHandler<UpdateTipoContusionRequest, UpdateTipoContusionResponse>
    {
        private readonly MeatContext context;

        public UpdateTipoContusionHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<UpdateTipoContusionResponse> Handle(UpdateTipoContusionRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.TiposContusiones
                .FirstOrDefaultAsync(x => x.Codigo == request.Codigo, cancellationToken);

            if (entity == null)
                throw new ValidationException("El tipo de contusion no existe.");

            if (!await this.context.Especies.AnyAsync(e => e.Codigo == request.EspecieId, cancellationToken))
                throw new ValidationException("La especie indicada no existe.");

            entity.Nombre = (request.Nombre ?? string.Empty).Trim();
            entity.EspecieId = request.EspecieId;
            entity.Orden = request.Orden;
            entity.Activo = request.Activo;

            await this.context.SaveChangesAsync(cancellationToken);

            return new UpdateTipoContusionResponse();
        }
    }
}
