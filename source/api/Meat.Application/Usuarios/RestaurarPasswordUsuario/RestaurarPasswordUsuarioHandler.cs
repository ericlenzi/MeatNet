using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared;
using Meat.Application.Shared.GeneratePassword;
using Meat.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Usuarios.RestaurarPasswordUsuario
{
    public class RestaurarPasswordUsuarioHandler : IRequestHandler<RestaurarPasswordUsuarioRequest, RestaurarPasswordUsuarioResponse>
    {
        private readonly MeatContext context;
        private readonly IMediator mediator;

        public RestaurarPasswordUsuarioHandler(MeatContext context, IMediator mediator)
        {
            this.context = context;
            this.mediator = mediator;
        }

        public async Task<RestaurarPasswordUsuarioResponse> Handle(RestaurarPasswordUsuarioRequest request, CancellationToken cancellationToken)
        {
            var parametro = await this.context.Parametros
                .FirstOrDefaultAsync(p => p.Codigo == "PASSWORD_INICIAL"
                    && p.Empresa.CodigoEmpresa == request.CodigoEmpresa, cancellationToken);

            if (parametro == null || string.IsNullOrWhiteSpace(parametro.Valor))
                throw new ValidationException("No se encontro el parametro PASSWORD_INICIAL para esta empresa.");

            var usuario = await this.context.Usuarios
                .FirstOrDefaultAsync(u => u.Id == request.UsuarioId, cancellationToken);

            if (usuario == null)
                throw new ValidationException("El usuario no existe.");

            if (!usuario.Activo)
                throw new ValidationException("No se puede restaurar la contraseña de un usuario inactivo.");

            var generatePasswordResponse = await this.mediator.Send(
                new GeneratePasswordRequest { Contraseña = parametro.Valor }, cancellationToken);

            usuario.PasswordHash = generatePasswordResponse.PasswordHash;
            this.context.Usuarios.Update(usuario);
            await this.context.SaveChangesAsync(cancellationToken);

            return new RestaurarPasswordUsuarioResponse();
        }
    }
}
