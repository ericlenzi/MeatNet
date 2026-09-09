 using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Meat.Application.Shared;
using Meat.Domain.Usuarios;
using Meat.Repositories;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
            string[] permisos = { "Admin", "Abastecimiento" };

            var user = await this.context.Usuarios.FirstOrDefaultAsync(
                p => p.UserName == request.Usuario
            );

            // El hash lleva salt, asi que la comparacion va en memoria y no en la consulta.
            if (user == null || !PasswordHash.Verificar(request.Contraseña, user.PasswordHash).EsValida)
                throw new ArgumentException("Usuario o contraseña incorrecto.");

            return new IsAutorizadorResponse()
            {
                IsAutorizador = permisos.Contains(user.RolId)
            };
        }

    }
}
