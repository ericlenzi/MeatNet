using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.TiposContusiones.CreateTipoContusion
{
    public class CreateTipoContusionHandler : IRequestHandler<CreateTipoContusionRequest, CreateTipoContusionResponse>
    {
        private readonly MeatContext context;

        public CreateTipoContusionHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<CreateTipoContusionResponse> Handle(CreateTipoContusionRequest request, CancellationToken cancellationToken)
        {
            var codigo = (request.Codigo ?? string.Empty).Trim();

            if (string.IsNullOrEmpty(codigo))
                throw new ValidationException("El codigo es requerido.");

            if (await this.context.TiposContusiones.AnyAsync(x => x.Codigo == codigo, cancellationToken))
                throw new ValidationException("Ya existe el tipo de contusion con ese codigo.");

            if (!await this.context.Especies.AnyAsync(e => e.Codigo == request.EspecieId, cancellationToken))
                throw new ValidationException("La especie indicada no existe.");

            var entity = new Meat.Domain.TiposContusiones.TipoContusion
            {
                Codigo = codigo,
                Nombre = (request.Nombre ?? string.Empty).Trim(),
                EspecieId = request.EspecieId,
                Orden = request.Orden,
                Activo = true,
            };

            this.context.TiposContusiones.Add(entity);
            await this.context.SaveChangesAsync(cancellationToken);

            return new CreateTipoContusionResponse { Codigo = entity.Codigo };
        }
    }
}
