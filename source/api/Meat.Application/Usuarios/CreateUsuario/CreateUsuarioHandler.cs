using MediatR;
using Meat.Repositories;
using System.Threading;
using System.Threading.Tasks;
using Meat.Application.Shared.GeneratePassword;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Meat.Application.Usuarios.CreateUsuario
{
    public class CreateUsuarioHandler : IRequestHandler<CreateUsuarioRequest, CreateUsuarioResponse>
    {
        private readonly MeatContext context;
        private readonly IMediator mediator;

        public CreateUsuarioHandler(MeatContext context, IMediator mediator)
        {
            this.context = context;
            this.mediator = mediator;
        }

        public async Task<CreateUsuarioResponse> Handle(CreateUsuarioRequest request, CancellationToken cancellationToken)
        {
            if (this.context.Usuarios.FirstOrDefault(x => x.UserName == request.UserName) != null)
            {
                throw new ValidationException("Ya existe un usuario con el nombre de usuario ingresado.");
            }

            var empresa = await this.context.Empresas.FirstOrDefaultAsync(e => e.CodigoEmpresa == request.CodigoEmpresa);
            if (empresa == null)
                throw new ValidationException("La empresa activa no es valida.");

            var generatePasswordResponse = this.mediator.Send(new GeneratePasswordRequest()
            {
                Contraseña = request.Apellido,
            }).Result;

            var usuario = Domain.Usuarios.UsuarioFactory.Create(
                request.UserName,
                generatePasswordResponse.PasswordHash,
                request.Nombre,
                request.Apellido,
                request.Email,
                request.Legajo,
                request.RolId,
                empresa.Id,
                request.Activo
            );

            this.context.Usuarios.Add(usuario);

            await this.context.SaveChangesAsync();

            return new CreateUsuarioResponse()
            {
                Id = usuario.Id,
            };
        }
    }
}