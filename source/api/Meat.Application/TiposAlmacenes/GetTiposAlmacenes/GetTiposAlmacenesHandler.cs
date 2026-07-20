using MediatR;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.TiposAlmacenes.GetTiposAlmacenes
{
    public class GetTiposAlmacenesHandler : IRequestHandler<GetTiposAlmacenesRequest, GetTiposAlmacenesResponse>
    {
        private readonly MeatContext context;

        public GetTiposAlmacenesHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetTiposAlmacenesResponse> Handle(GetTiposAlmacenesRequest request, CancellationToken cancellationToken)
        {
            var data = await this.context.TiposAlmacenes
                .Where(t => t.Activo)
                .OrderBy(t => t.Nombre)
                .Select(t => new TipoAlmacenItem { Codigo = t.Codigo, Nombre = t.Nombre, Familia = t.Familia })
                .ToListAsync(cancellationToken);

            return new GetTiposAlmacenesResponse { Data = data };
        }
    }
}
