using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Usuarios.RemoveUsuarioSucursal
{
    public class RemoveUsuarioSucursalHandler : IRequestHandler<RemoveUsuarioSucursalRequest, RemoveUsuarioSucursalResponse>
    {
        private readonly MeatContext context;

        public RemoveUsuarioSucursalHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<RemoveUsuarioSucursalResponse> Handle(RemoveUsuarioSucursalRequest request, CancellationToken cancellationToken)
        {
            var usuarioSucursal = await this.context.UsuariosSucursales
                .FirstOrDefaultAsync(us => us.Id == request.UsuarioSucursalId, cancellationToken);

            if (usuarioSucursal == null)
                throw new ValidationException("La asignacion no existe.");

            this.context.UsuariosSucursales.Remove(usuarioSucursal);
            await this.context.SaveChangesAsync(cancellationToken);

            return new RemoveUsuarioSucursalResponse();
        }
    }
}
