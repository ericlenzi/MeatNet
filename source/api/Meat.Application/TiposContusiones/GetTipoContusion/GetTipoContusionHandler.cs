using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.TiposContusiones.GetTipoContusion
{
    public class GetTipoContusionHandler : IRequestHandler<GetTipoContusionRequest, GetTipoContusionResponse>
    {
        private readonly MeatContext context;

        public GetTipoContusionHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetTipoContusionResponse> Handle(GetTipoContusionRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.TiposContusiones
                .FirstOrDefaultAsync(x => x.Codigo == request.Codigo, cancellationToken);

            if (entity == null)
                return null;

            return new GetTipoContusionResponse
            {
                Codigo = entity.Codigo,
                Nombre = entity.Nombre,
                EspecieId = entity.EspecieId,
                Orden = entity.Orden,
                Activo = entity.Activo,
            };
        }
    }
}
