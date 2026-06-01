using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Usuarios.AddUsuarioSucursal
{
    public class AddUsuarioSucursalHandler : IRequestHandler<AddUsuarioSucursalRequest, AddUsuarioSucursalResponse>
    {
        private readonly MeatContext context;

        public AddUsuarioSucursalHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<AddUsuarioSucursalResponse> Handle(AddUsuarioSucursalRequest request, CancellationToken cancellationToken)
        {
            var exists = await this.context.UsuariosSucursales
                .AnyAsync(us => us.UsuarioId == request.UsuarioId && us.SucursalId == request.SucursalId, cancellationToken);

            if (exists)
                throw new ValidationException("La sucursal ya esta asignada al usuario.");

            var usuarioSucursal = new Domain.UsuariosSucursales.UsuarioSucursal
            {
                Id = Guid.NewGuid(),
                UsuarioId = request.UsuarioId,
                SucursalId = request.SucursalId,
                esMain = request.EsMain,
                FechaActualizacion = DateTime.Now
            };

            this.context.UsuariosSucursales.Add(usuarioSucursal);
            await this.context.SaveChangesAsync(cancellationToken);

            return new AddUsuarioSucursalResponse { Id = usuarioSucursal.Id };
        }
    }
}
