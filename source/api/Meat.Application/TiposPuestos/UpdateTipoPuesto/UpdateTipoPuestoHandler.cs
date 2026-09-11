using MediatR;
using Meat.Application.Shared;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.TiposPuestos.UpdateTipoPuesto
{
    public class UpdateTipoPuestoHandler : IRequestHandler<UpdateTipoPuestoRequest, UpdateTipoPuestoResponse>
    {
        private readonly MeatContext context;

        public UpdateTipoPuestoHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<UpdateTipoPuestoResponse> Handle(UpdateTipoPuestoRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.TiposPuestos
                .FirstOrDefaultAsync(x => x.Codigo == request.Codigo, cancellationToken);

            if (entity == null)
                throw new ValidationException("El tipo de puesto no existe.");

            entity.Nombre = (request.Nombre ?? string.Empty).Trim();
            entity.Activo = request.Activo;

            await this.context.SaveChangesAsync(cancellationToken);

            return new UpdateTipoPuestoResponse();
        }
    }
}
