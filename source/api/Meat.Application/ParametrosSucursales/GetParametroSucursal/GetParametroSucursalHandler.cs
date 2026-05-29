using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.ParametrosSucursales.GetParametroSucursal
{
    public class GetParametroSucursalHandler : IRequestHandler<GetParametroSucursalRequest, GetParametroSucursalResponse>
    {
        private readonly MeatContext context;
        private readonly IMapper mapper;

        public GetParametroSucursalHandler(MeatContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<GetParametroSucursalResponse> Handle(GetParametroSucursalRequest request, CancellationToken cancellationToken)
        {
            var parametro = await this.context.ParametrosSucursales.FirstOrDefaultAsync(p => p.Id == request.Id);

            return this.mapper.Map<GetParametroSucursalResponse>(parametro);
        }
    }
}
