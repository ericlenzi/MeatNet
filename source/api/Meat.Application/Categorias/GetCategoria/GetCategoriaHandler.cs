using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Categorias.GetCategoria
{
    public class GetCategoriaHandler : IRequestHandler<GetCategoriaRequest, GetCategoriaResponse>
    {
        private readonly MeatContext context;
        private readonly IMapper mapper;

        public GetCategoriaHandler(MeatContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<GetCategoriaResponse> Handle(GetCategoriaRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.Categorias
                .Include(x => x.Especie)
                .Include(x => x.TipoSexo)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            return this.mapper.Map<GetCategoriaResponse>(entity);
        }
    }
}
