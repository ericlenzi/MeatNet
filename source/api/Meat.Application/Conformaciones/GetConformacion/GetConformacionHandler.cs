using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Conformaciones.GetConformacion
{
    public class GetConformacionHandler : IRequestHandler<GetConformacionRequest, GetConformacionResponse>
    {
        private readonly MeatContext context;

        public GetConformacionHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetConformacionResponse> Handle(GetConformacionRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.Conformaciones
                .FirstOrDefaultAsync(x => x.Codigo == request.Codigo, cancellationToken);

            if (entity == null)
                return null;

            return new GetConformacionResponse
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
