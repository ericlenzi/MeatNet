using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Repositories;

namespace Meat.Application.Usuarios.DeleteUsuario
{
    public class DeleteUsuarioHandler : IRequestHandler<DeleteUsuarioRequest, DeleteUsuarioResponse>
    {
        private readonly MeatContext context;

        public DeleteUsuarioHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<DeleteUsuarioResponse> Handle(DeleteUsuarioRequest request, CancellationToken cancellationToken)
        {
            var usuario = await this.context.Usuarios
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (usuario == null)
                throw new ValidationException("El usuario no existe");

            var usuariosEstablecimientos = await this.context.UsuariosEstablecimientos
                .Where(ue => ue.UsuarioId == request.Id)
                .ToListAsync(cancellationToken);
            this.context.UsuariosEstablecimientos.RemoveRange(usuariosEstablecimientos);

            var usuariosSucursales = await this.context.UsuariosSucursales
                .Where(us => us.UsuarioId == request.Id)
                .ToListAsync(cancellationToken);
            this.context.UsuariosSucursales.RemoveRange(usuariosSucursales);

            this.context.Usuarios.Remove(usuario);
            await this.context.SaveChangesAsync(cancellationToken);

            return new DeleteUsuarioResponse();
        }
    }
}
