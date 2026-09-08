using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Sucursales.GetSucursalByCodigo
{
    public class GetSucursalByCodigoHandler : IRequestHandler<GetSucursalByCodigoRequest, GetSucursalByCodigoResponse>
    {
        private readonly MeatContext context;
        private readonly IMapper mapper;

        public GetSucursalByCodigoHandler(MeatContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<GetSucursalByCodigoResponse> Handle(GetSucursalByCodigoRequest request, CancellationToken cancellationToken)
        {
            var sucursal = await this.context.Sucursales
                .Include(x => x.Empresa)
                .FirstOrDefaultAsync(x => x.CodigoSucursal == request.Codigo);

            var suc = this.mapper.Map<GetSucursalByCodigoResponse>(sucursal);
            return suc;
        }
    }
}
