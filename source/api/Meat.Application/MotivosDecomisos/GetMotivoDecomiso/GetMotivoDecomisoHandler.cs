using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.MotivosDecomisos.GetMotivoDecomiso
{
    public class GetMotivoDecomisoHandler : IRequestHandler<GetMotivoDecomisoRequest, GetMotivoDecomisoResponse>
    {
        private readonly MeatContext context;

        public GetMotivoDecomisoHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetMotivoDecomisoResponse> Handle(GetMotivoDecomisoRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.MotivosDecomisos
                .FirstOrDefaultAsync(x => x.Codigo == request.Codigo, cancellationToken);

            if (entity == null)
                return null;

            return new GetMotivoDecomisoResponse
            {
                Codigo = entity.Codigo,
                Nombre = entity.Nombre,
                EspecieId = entity.EspecieId,
                Orden = entity.Orden,
                ExigeContusion = entity.ExigeContusion,
                Activo = entity.Activo,
            };
        }
    }
}
