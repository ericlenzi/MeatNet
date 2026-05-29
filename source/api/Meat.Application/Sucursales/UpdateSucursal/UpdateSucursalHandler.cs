using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Repositories;
using System;

namespace Meat.Application.Sucursales.UpdateSucursal
{

    public class UpdateSucursalHandler : IRequestHandler<UpdateSucursalRequest, UpdateSucursalResponse>
    {
        private readonly MeatContext context;
        private readonly IMapper mapper;

        public UpdateSucursalHandler(MeatContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<UpdateSucursalResponse> Handle(UpdateSucursalRequest request, CancellationToken cancellationToken)
        {
            var sucursal = await this.context.Sucursales.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (sucursal == null)
            {
                throw new ValidationException("La sucursal no existe");
            }

            this.mapper.Map(request, sucursal);
            sucursal.FechaActualizacion = DateTime.Now;

            await this.context.SaveChangesAsync();

            return new UpdateSucursalResponse();
        }
    }
}
