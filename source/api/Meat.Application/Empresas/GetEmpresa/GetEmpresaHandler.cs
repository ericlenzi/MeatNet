using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Empresas.GetEmpresa
{
    public class GetEmpresaHandler : IRequestHandler<GetEmpresaRequest, GetEmpresaResponse>
    {
        private readonly MeatContext context;
        private readonly IMapper mapper;

        public GetEmpresaHandler(MeatContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<GetEmpresaResponse> Handle(GetEmpresaRequest request, CancellationToken cancellationToken)
        {
            var empresa = await this.context.Empresas
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            return this.mapper.Map<GetEmpresaResponse>(empresa);
        }
    }
}
