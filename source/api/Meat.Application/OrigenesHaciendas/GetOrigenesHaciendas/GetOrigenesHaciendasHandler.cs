using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Repositories;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.OrigenesHaciendas.GetOrigenesHaciendas
{
    public class GetOrigenesHaciendasHandler : IRequestHandler<GetOrigenesHaciendasRequest, GetOrigenesHaciendasResponse>
    {
        private readonly MeatContext context;

        public GetOrigenesHaciendasHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetOrigenesHaciendasResponse> Handle(GetOrigenesHaciendasRequest request, CancellationToken cancellationToken)
        {
            var data = await this.context.OrigenesHaciendas
                .Where(x => x.Activo)
                .OrderBy(x => x.Nombre)
                .ToListAsync(cancellationToken);

            return new GetOrigenesHaciendasResponse { Data = data };
        }
    }
}
