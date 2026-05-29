using AutoMapper;
using MediatR;
using Meat.Repositories;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Meat.Application.Shared;

namespace Meat.Application.Sucursales.CreateSucursal
{
    public class CreateSucursalHandler : IRequestHandler<CreateSucursalRequest, CreateSucursalResponse>
    {
        private readonly MeatContext context;
        private readonly IMapper mapper;

        public CreateSucursalHandler(MeatContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<CreateSucursalResponse> Handle(CreateSucursalRequest request, CancellationToken cancellationToken)
        {
            var sucursal = Domain.Sucursales.SucursalFactory.Create();
            this.mapper.Map(request, sucursal);

            this.context.Sucursales.Add(sucursal);

            await this.context.SaveChangesAsync();

            return new CreateSucursalResponse()
            {
                Id = sucursal.Id,
            };
        }
    }
}
