using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Sucursales.GetSucursal
{

    public class GetSucursalHandler : IRequestHandler<GetSucursalRequest, GetSucursalResponse>
    {
        private readonly MeatContext context;
        private readonly IMapper mapper;

        public GetSucursalHandler(MeatContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<GetSucursalResponse> Handle(GetSucursalRequest request, CancellationToken cancellationToken)
        {
            var sucursal = await this.context.Sucursales.FirstOrDefaultAsync(p => p.Id == request.Id);

            return this.mapper.Map<GetSucursalResponse>(sucursal);
        }
    }
}
