using MediatR;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.UnidadesMedidas.GetUnidadesMedidas
{
    public class GetUnidadesMedidasHandler : IRequestHandler<GetUnidadesMedidasRequest, GetUnidadesMedidasResponse>
    {
        private readonly MeatContext context;

        public GetUnidadesMedidasHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetUnidadesMedidasResponse> Handle(GetUnidadesMedidasRequest request, CancellationToken cancellationToken)
        {
            var data = await this.context.UnidadesMedidas
                .Where(u => u.Activo)
                .OrderBy(u => u.Nombre)
                .Select(u => new UnidadMedidaItem { Codigo = u.Codigo, Nombre = u.Nombre })
                .ToListAsync(cancellationToken);

            return new GetUnidadesMedidasResponse { Data = data };
        }
    }
}
