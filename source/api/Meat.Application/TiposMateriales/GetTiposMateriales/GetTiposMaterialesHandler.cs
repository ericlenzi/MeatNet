using MediatR;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.TiposMateriales.GetTiposMateriales
{
    public class GetTiposMaterialesHandler : IRequestHandler<GetTiposMaterialesRequest, GetTiposMaterialesResponse>
    {
        private readonly MeatContext context;

        public GetTiposMaterialesHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetTiposMaterialesResponse> Handle(GetTiposMaterialesRequest request, CancellationToken cancellationToken)
        {
            var data = await this.context.TiposMateriales
                .Where(t => t.Activo)
                .OrderBy(t => t.Nombre)
                .Select(t => new TipoMaterialItem { Codigo = t.Codigo, Nombre = t.Nombre })
                .ToListAsync(cancellationToken);

            return new GetTiposMaterialesResponse { Data = data };
        }
    }
}
