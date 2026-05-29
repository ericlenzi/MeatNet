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

namespace Meat.Application.Usuarios.CambiarContraseñaUsuario
{
    public class CambiarContraseñaUsuarioHandler : IRequestHandler<CambiarContraseñaUsuarioRequest, CambiarContraseñaUsuarioResponse>
    {
        private readonly MeatContext context;
        private readonly IMediator mediator;

        public CambiarContraseñaUsuarioHandler(MeatContext context, IMediator mediator)
        {
            this.context = context;
            this.mediator = mediator;
        }

        public async Task<CambiarContraseñaUsuarioResponse> Handle(CambiarContraseñaUsuarioRequest request, CancellationToken cancellationToken)
        {
            string passwordHash;

            using (SHA1 sha1Hash = SHA1.Create())
            {
                var newPassword = request.ContraseñaActual;
                byte[] sourceBytes = Encoding.UTF8.GetBytes(newPassword);
                byte[] hashBytes = sha1Hash.ComputeHash(sourceBytes);
                passwordHash = BitConverter.ToString(hashBytes).Replace("-", string.Empty);
            }

            var user = await this.context.Usuarios.FirstOrDefaultAsync(
                p => p.Id == request.UsuarioId && p.PasswordHash == passwordHash
            );

            if (user == null)
                throw new ArgumentException("Contraseña incorrecta.");

            if (!user.Activo)
                throw new ArgumentException("Usuario inactivo.");

            var generatePasswordResponse = this.mediator.Send(new GeneratePasswordRequest()
            {
                Contraseña = request.ContraseñaNueva
            }).Result;

            user.PasswordHash = generatePasswordResponse.PasswordHash;

            this.context.Usuarios.Update(user);

            await this.context.SaveChangesAsync();

            return new CambiarContraseñaUsuarioResponse();
        }
    }
}