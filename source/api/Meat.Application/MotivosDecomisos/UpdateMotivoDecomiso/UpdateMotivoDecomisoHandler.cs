using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.MotivosDecomisos.UpdateMotivoDecomiso
{
    public class UpdateMotivoDecomisoHandler : IRequestHandler<UpdateMotivoDecomisoRequest, UpdateMotivoDecomisoResponse>
    {
        private readonly MeatContext context;

        public UpdateMotivoDecomisoHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<UpdateMotivoDecomisoResponse> Handle(UpdateMotivoDecomisoRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.MotivosDecomisos
                .FirstOrDefaultAsync(x => x.Codigo == request.Codigo, cancellationToken);

            if (entity == null)
                throw new ValidationException("El motivo de decomiso no existe.");

            if (!await this.context.Especies.AnyAsync(e => e.Codigo == request.EspecieId, cancellationToken))
                throw new ValidationException("La especie indicada no existe.");

            entity.Nombre = (request.Nombre ?? string.Empty).Trim();
            entity.EspecieId = request.EspecieId;
            entity.Orden = request.Orden;
            entity.ExigeContusion = request.ExigeContusion;
            entity.Activo = request.Activo;

            await this.context.SaveChangesAsync(cancellationToken);

            return new UpdateMotivoDecomisoResponse();
        }
    }
}
