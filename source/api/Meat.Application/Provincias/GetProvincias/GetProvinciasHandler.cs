using MediatR;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Provincias.GetProvincias
{
    public class GetProvinciasHandler : IRequestHandler<GetProvinciasRequest, GetProvinciasResponse>
    {
        private readonly MeatContext context;

        public GetProvinciasHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetProvinciasResponse> Handle(GetProvinciasRequest request, CancellationToken cancellationToken)
        {
            var data = await this.context.Provincias
                .Where(p => p.Activo)
                .OrderBy(p => p.Nombre)
                .Select(p => new ProvinciaItem { Id = p.Id, Nombre = p.Nombre })
                .ToListAsync(cancellationToken);

            return new GetProvinciasResponse { Data = data };
        }
    }
}
