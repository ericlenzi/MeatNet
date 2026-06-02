using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
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
            var existe = await this.context.Sucursales.AnyAsync(s => s.CodigoSucursal == request.CodigoSucursal, cancellationToken);
            if (existe)
                throw new ValidationException("Ya existe una sucursal con ese codigo.");

            var empresa = await this.context.Empresas.FirstOrDefaultAsync(e => e.CodigoEmpresa == request.CodigoEmpresa, cancellationToken);
            if (empresa == null)
                throw new ValidationException("La empresa activa no es valida.");

            var sucursal = Domain.Sucursales.SucursalFactory.Create();
            this.mapper.Map(request, sucursal);
            sucursal.EmpresaId = empresa.Id;

            this.context.Sucursales.Add(sucursal);

            await this.context.SaveChangesAsync();

            return new CreateSucursalResponse()
            {
                Id = sucursal.Id,
            };
        }
    }
}
