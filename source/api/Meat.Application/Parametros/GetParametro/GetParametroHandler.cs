using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Parametros.GetParametro
{
    public class GetParametroHandler : IRequestHandler<GetParametroRequest, GetParametroResponse>
    {
        private readonly MeatContext context;
        private readonly IMapper mapper;

        public GetParametroHandler(MeatContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<GetParametroResponse> Handle(GetParametroRequest request, CancellationToken cancellationToken)
        {
            var parametro = await this.context.Parametros.FirstOrDefaultAsync(p => p.Id == request.Id);

            return this.mapper.Map<GetParametroResponse>(parametro);
        }
    }
}
