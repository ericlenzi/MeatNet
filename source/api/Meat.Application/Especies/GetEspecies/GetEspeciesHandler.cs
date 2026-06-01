using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Repositories;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Especies.GetEspecies
{
    public class GetEspeciesHandler : IRequestHandler<GetEspeciesRequest, GetEspeciesResponse>
    {
        private readonly MeatContext context;

        public GetEspeciesHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetEspeciesResponse> Handle(GetEspeciesRequest request, CancellationToken cancellationToken)
        {
            var data = await this.context.Especies
                .Where(x => x.Activo)
                .OrderBy(x => x.Nombre)
                .ToListAsync(cancellationToken);

            return new GetEspeciesResponse { Data = data };
        }
    }
}
