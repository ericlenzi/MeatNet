using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Repositories;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.TiposSexos.GetTiposSexos
{
    public class GetTiposSexosHandler : IRequestHandler<GetTiposSexosRequest, GetTiposSexosResponse>
    {
        private readonly MeatContext context;

        public GetTiposSexosHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetTiposSexosResponse> Handle(GetTiposSexosRequest request, CancellationToken cancellationToken)
        {
            var data = await this.context.TiposSexos
                .Where(x => x.Activo)
                .OrderBy(x => x.Nombre)
                .ToListAsync(cancellationToken);

            return new GetTiposSexosResponse { Data = data };
        }
    }
}
