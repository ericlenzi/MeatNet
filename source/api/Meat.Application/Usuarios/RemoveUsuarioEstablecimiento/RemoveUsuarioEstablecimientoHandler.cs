using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Repositories;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Usuarios.RemoveUsuarioEstablecimiento
{
    public class RemoveUsuarioEstablecimientoHandler : IRequestHandler<RemoveUsuarioEstablecimientoRequest, RemoveUsuarioEstablecimientoResponse>
    {
        private readonly MeatContext context;

        public RemoveUsuarioEstablecimientoHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<RemoveUsuarioEstablecimientoResponse> Handle(RemoveUsuarioEstablecimientoRequest request, CancellationToken cancellationToken)
        {
            var usuarioEstablecimiento = await this.context.UsuariosEstablecimientos
                .FirstOrDefaultAsync(ue => ue.Id == request.UsuarioEstablecimientoId, cancellationToken);

            if (usuarioEstablecimiento == null)
                throw new ValidationException("La asignacion no existe.");

            if (usuarioEstablecimiento.EsMain)
            {
                var otrosEstablecimientos = await this.context.UsuariosEstablecimientos
                    .AnyAsync(ue => ue.UsuarioId == usuarioEstablecimiento.UsuarioId && ue.Id != usuarioEstablecimiento.Id, cancellationToken);

                if (otrosEstablecimientos)
                    throw new ValidationException("No se puede quitar el establecimiento principal. Primero establece otro como principal.");
            }

            this.context.UsuariosEstablecimientos.Remove(usuarioEstablecimiento);
            await this.context.SaveChangesAsync(cancellationToken);

            return new RemoveUsuarioEstablecimientoResponse();
        }
    }
}
