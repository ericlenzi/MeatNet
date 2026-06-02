using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Especies.GetEspecie
{
    public class GetEspecieHandler : IRequestHandler<GetEspecieRequest, GetEspecieResponse>
    {
        private readonly MeatContext context;
        private readonly IMapper mapper;

        public GetEspecieHandler(MeatContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<GetEspecieResponse> Handle(GetEspecieRequest request, CancellationToken cancellationToken)
        {
            var especie = await this.context.Especies
                .FirstOrDefaultAsync(x => x.Codigo == request.Codigo);

            return this.mapper.Map<GetEspecieResponse>(especie);
        }
    }
}
