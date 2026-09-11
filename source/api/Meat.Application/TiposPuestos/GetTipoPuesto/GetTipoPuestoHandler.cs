using MediatR;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.TiposPuestos.GetTipoPuesto
{
    public class GetTipoPuestoHandler : IRequestHandler<GetTipoPuestoRequest, GetTipoPuestoResponse>
    {
        private readonly MeatContext context;

        public GetTipoPuestoHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetTipoPuestoResponse> Handle(GetTipoPuestoRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.TiposPuestos
                .FirstOrDefaultAsync(x => x.Codigo == request.Codigo, cancellationToken);

            if (entity == null)
                return null;

            return new GetTipoPuestoResponse
            {
                Codigo = entity.Codigo,
                Nombre = entity.Nombre,
                Activo = entity.Activo,
            };
        }
    }
}
