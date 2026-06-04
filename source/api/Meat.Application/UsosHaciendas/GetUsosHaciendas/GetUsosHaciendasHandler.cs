using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Repositories;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.UsosHaciendas.GetUsosHaciendas
{
    public class GetUsosHaciendasHandler : IRequestHandler<GetUsosHaciendasRequest, GetUsosHaciendasResponse>
    {
        private readonly MeatContext context;

        public GetUsosHaciendasHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetUsosHaciendasResponse> Handle(GetUsosHaciendasRequest request, CancellationToken cancellationToken)
        {
            var data = await this.context.UsosHaciendas
                .Where(x => x.Activo)
                .OrderBy(x => x.Nombre)
                .ToListAsync(cancellationToken);

            return new GetUsosHaciendasResponse { Data = data };
        }
    }
}
