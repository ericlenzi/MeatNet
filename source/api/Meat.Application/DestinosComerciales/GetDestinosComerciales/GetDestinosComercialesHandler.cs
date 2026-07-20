using MediatR;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.DestinosComerciales.GetDestinosComerciales
{
    public class GetDestinosComercialesHandler : IRequestHandler<GetDestinosComercialesRequest, GetDestinosComercialesResponse>
    {
        private readonly MeatContext context;

        public GetDestinosComercialesHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetDestinosComercialesResponse> Handle(GetDestinosComercialesRequest request, CancellationToken cancellationToken)
        {
            var data = await this.context.DestinosComerciales
                .Where(d => d.Activo)
                .OrderBy(d => d.Nombre)
                .Select(d => new DestinoComercialItem { Codigo = d.Codigo, Nombre = d.Nombre })
                .ToListAsync(cancellationToken);

            return new GetDestinosComercialesResponse { Data = data };
        }
    }
}
