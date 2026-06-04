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
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (usuario == null)
            {
                throw new ValidationException("El usuario no existe");
            }

            usuario.Activo = false;

            this.context.Usuarios.Update(usuario);

            await this.context.SaveChangesAsync();

            return new DeleteUsuarioResponse();
        }
    }
}
