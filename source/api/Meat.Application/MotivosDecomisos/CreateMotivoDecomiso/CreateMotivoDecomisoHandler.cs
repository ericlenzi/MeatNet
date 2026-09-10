using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.MotivosDecomisos.CreateMotivoDecomiso
{
    public class CreateMotivoDecomisoHandler : IRequestHandler<CreateMotivoDecomisoRequest, CreateMotivoDecomisoResponse>
    {
        private readonly MeatContext context;

        public CreateMotivoDecomisoHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<CreateMotivoDecomisoResponse> Handle(CreateMotivoDecomisoRequest request, CancellationToken cancellationToken)
        {
            var codigo = (request.Codigo ?? string.Empty).Trim();

            if (string.IsNullOrEmpty(codigo))
                throw new ValidationException("El codigo es requerido.");

            if (await this.context.MotivosDecomisos.AnyAsync(x => x.Codigo == codigo, cancellationToken))
                throw new ValidationException("Ya existe el motivo de decomiso con ese codigo.");

            if (!await this.context.Especies.AnyAsync(e => e.Codigo == request.EspecieId, cancellationToken))
                throw new ValidationException("La especie indicada no existe.");

            var entity = new Meat.Domain.MotivosDecomisos.MotivoDecomiso
            {
                Codigo = codigo,
                Nombre = (request.Nombre ?? string.Empty).Trim(),
                EspecieId = request.EspecieId,
                Orden = request.Orden,
                ExigeContusion = request.ExigeContusion,
                Activo = true,
            };

            this.context.MotivosDecomisos.Add(entity);
            await this.context.SaveChangesAsync(cancellationToken);

            return new CreateMotivoDecomisoResponse { Codigo = entity.Codigo };
        }
    }
}
