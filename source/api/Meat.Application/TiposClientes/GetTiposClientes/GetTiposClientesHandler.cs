using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Repositories;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.TiposClientes.GetTiposClientes
{
    public class GetTiposClientesHandler : IRequestHandler<GetTiposClientesRequest, GetTiposClientesResponse>
    {
        private readonly MeatContext context;

        public GetTiposClientesHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetTiposClientesResponse> Handle(GetTiposClientesRequest request, CancellationToken cancellationToken)
        {
            var data = await this.context.TiposClientes
                .Where(x => x.Activo)
                .OrderBy(x => x.Nombre)
                .ToListAsync(cancellationToken);

            return new GetTiposClientesResponse { Data = data };
        }
    }
}
