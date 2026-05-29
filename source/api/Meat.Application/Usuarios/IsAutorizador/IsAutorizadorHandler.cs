 using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Meat.Domain.Usuarios;
using Meat.Repositories;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace Meat.Application.Usuarios.IsAutorizador
{
    public class IsAutorizadorHandler : IRequestHandler<IsAutorizadorRequest, IsAutorizadorResponse>
    {
        private readonly MeatContext context;
        private readonly IMediator mediator;

        public IsAutorizadorHandler(MeatContext context, IMediator mediator)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.mediator = mediator;
        }

        public async Task<IsAutorizadorResponse> Handle(IsAutorizadorRequest request, CancellationToken cancellationToken)
        {
            string passwordHash;
            string[] permisos = { "Admin", "Abastecimiento" };

            using (SHA1 sha1Hash = SHA1.Create())
            {
                var newPassword = request.Contraseña;
                byte[] sourceBytes = Encoding.UTF8.GetBytes(newPassword);
                byte[] hashBytes = sha1Hash.ComputeHash(sourceBytes);
                passwordHash = BitConverter.ToString(hashBytes).Replace("-", string.Empty);
            }

            var user = await this.context.Usuarios.FirstOrDefaultAsync(
                p => p.UserName == request.Usuario && 
                     p.PasswordHash == passwordHash 
            );

            if (user == null)
                throw new ArgumentException("Usuario o contraseña incorrecto.");

            return new IsAutorizadorResponse()
            {
                IsAutorizador = permisos.Contains(user.RolId)
            };
        }

    }
}
