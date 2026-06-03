using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Repositories;
using System.Linq;
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

            var otrasAsignaciones = await this.context.UsuariosSucursales
                .Where(us => us.UsuarioId == usuarioSucursal.UsuarioId && us.Id != usuarioSucursal.Id)
                .ToListAsync(cancellationToken);

            if (!otrasAsignaciones.Any())
                throw new ValidationException("El usuario debe tener al menos una sucursal asignada.");

            var tieneEstablecimientos = await this.context.UsuariosEstablecimientos
                .Join(this.context.Establecimientos,
                    ue => ue.EstablecimientoId,
                    e => e.Id,
                    (ue, e) => new { ue.UsuarioId, e.SucursalId })
                .AnyAsync(x => x.UsuarioId == usuarioSucursal.UsuarioId && x.SucursalId == usuarioSucursal.SucursalId,
                    cancellationToken);

            if (tieneEstablecimientos)
                throw new ValidationException("No se puede quitar la sucursal porque el usuario tiene establecimientos asignados de esa sucursal.");

            if (usuarioSucursal.esMain)
                throw new ValidationException("No se puede quitar la sucursal principal. Primero establecé otra como principal.");

            this.context.UsuariosSucursales.Remove(usuarioSucursal);
            await this.context.SaveChangesAsync(cancellationToken);

            return new RemoveUsuarioSucursalResponse();
        }
    }
}
