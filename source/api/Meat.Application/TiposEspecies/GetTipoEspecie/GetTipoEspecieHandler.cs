using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.TiposEspecies.GetTipoEspecie
{
    public class GetTipoEspecieHandler : IRequestHandler<GetTipoEspecieRequest, GetTipoEspecieResponse>
    {
        private readonly MeatContext context;
        private readonly IMapper mapper;

        public GetTipoEspecieHandler(MeatContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<GetTipoEspecieResponse> Handle(GetTipoEspecieRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.TiposEspecies
                .Include(x => x.Especie)
                .Include(x => x.TipoSexo)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            return this.mapper.Map<GetTipoEspecieResponse>(entity);
        }
    }
}
