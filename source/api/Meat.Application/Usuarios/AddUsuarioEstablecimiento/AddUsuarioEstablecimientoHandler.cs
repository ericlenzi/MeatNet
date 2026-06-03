using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Repositories;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Usuarios.AddUsuarioEstablecimiento
{
    public class AddUsuarioEstablecimientoHandler : IRequestHandler<AddUsuarioEstablecimientoRequest, AddUsuarioEstablecimientoResponse>
    {
        private readonly MeatContext context;

        public AddUsuarioEstablecimientoHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<AddUsuarioEstablecimientoResponse> Handle(AddUsuarioEstablecimientoRequest request, CancellationToken cancellationToken)
        {
            // Verificar que el establecimiento pertenece a una sucursal habilitada del usuario
            var establecimiento = await this.context.Establecimientos
                .FirstOrDefaultAsync(e => e.Id == request.EstablecimientoId, cancellationToken);

            if (establecimiento == null)
                throw new ValidationException("El establecimiento no existe.");

            var sucursalHabilitada = await this.context.UsuariosSucursales
                .AnyAsync(us => us.UsuarioId == request.UsuarioId && us.SucursalId == establecimiento.SucursalId,
                    cancellationToken);

            if (!sucursalHabilitada)
                throw new ValidationException("El establecimiento no pertenece a ninguna sucursal habilitada del usuario.");

            var exists = await this.context.UsuariosEstablecimientos
                .AnyAsync(ue => ue.UsuarioId == request.UsuarioId && ue.EstablecimientoId == request.EstablecimientoId, cancellationToken);

            if (exists)
                throw new ValidationException("El establecimiento ya esta asignado al usuario.");

            // Si es el primero, forzar EsMain = true
            var tieneEstablecimientos = await this.context.UsuariosEstablecimientos
                .AnyAsync(ue => ue.UsuarioId == request.UsuarioId, cancellationToken);

            var esMain = !tieneEstablecimientos || request.EsMain;

            var usuarioEstablecimiento = new Domain.UsuariosEstablecimientos.UsuarioEstablecimiento
            {
                Id = Guid.NewGuid(),
                UsuarioId = request.UsuarioId,
                EstablecimientoId = request.EstablecimientoId,
                EsMain = esMain,
                FechaActualizacion = DateTime.Now
            };

            this.context.UsuariosEstablecimientos.Add(usuarioEstablecimiento);
            await this.context.SaveChangesAsync(cancellationToken);

            return new AddUsuarioEstablecimientoResponse { Id = usuarioEstablecimiento.Id };
        }
    }
}
