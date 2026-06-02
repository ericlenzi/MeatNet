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
            var parametro = await this.context.Parametros
                .Include(p => p.Empresa)
                .FirstOrDefaultAsync(p => p.Codigo == request.Codigo && p.Empresa.CodigoEmpresa == request.CodigoEmpresa);

            return this.mapper.Map<GetParametroResponse>(parametro);
        }
    }
}
