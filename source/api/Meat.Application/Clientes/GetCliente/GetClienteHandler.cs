using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Clientes.GetCliente
{
    public class GetClienteHandler : IRequestHandler<GetClienteRequest, GetClienteResponse>
    {
        private readonly MeatContext context;
        private readonly IMapper mapper;

        public GetClienteHandler(MeatContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<GetClienteResponse> Handle(GetClienteRequest request, CancellationToken cancellationToken)
        {
            var entity = await this.context.Clientes
                .Include(x => x.EmpresaPadre)
                .Include(x => x.TipoCliente)
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.EmpresaPadre.CodigoEmpresa == request.CodigoEmpresa, cancellationToken);

            return this.mapper.Map<GetClienteResponse>(entity);
        }
    }
}
