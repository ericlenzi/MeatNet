using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Application.Shared.GeneratePassword;
using Meat.Domain.Enums;
using Meat.Repositories;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Usuarios.ResetearPasswordUsuario
{
    public class ResetearPasswordUsuarioHandler : IRequestHandler<ResetearPasswordUsuarioRequest, ResetearPasswordUsuarioResponse>
    {
        private readonly MeatContext context;
        private readonly IMediator mediator;

        public ResetearPasswordUsuarioHandler(MeatContext context, IMediator mediator)
        {
            this.context = context;
            this.mediator = mediator;
        }

        public async Task<ResetearPasswordUsuarioResponse> Handle(ResetearPasswordUsuarioRequest request, CancellationToken cancellationToken)
        {

            var user = await this.context.Usuarios.FirstOrDefaultAsync(
                p => p.Id == request.UsuarioId
            );

            if (user == null)
                throw new ArgumentException("Usuario no encontrado.");

            if (!user.Activo)
                throw new ArgumentException("Usuario inactivo.");

            var generatePasswordResponse = this.mediator.Send(new GeneratePasswordRequest()
            {
                Contraseña = request.ContraseñaNueva
            }).Result;

            user.PasswordHash = generatePasswordResponse.PasswordHash;

            this.context.Usuarios.Update(user);

            await this.context.SaveChangesAsync();

            return new ResetearPasswordUsuarioResponse();
        }
    }
}