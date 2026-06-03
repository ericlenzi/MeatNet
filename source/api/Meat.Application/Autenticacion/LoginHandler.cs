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

namespace Meat.Application.Autenticacion
{
    public class LoginHandler : IRequestHandler<LoginRequest, LoginResponse>
    {
        private readonly MeatContext context;
        private readonly string jwtSigningKey;
        private readonly int validFor;

        public LoginHandler(MeatContext context, IConfiguration configuration)
        {
            if (configuration == null) { throw new ArgumentNullException(nameof(configuration)); }
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.jwtSigningKey = configuration.GetValue<string>("JwtOptions:SigninKey");
            this.validFor = configuration.GetValue<int>("JwtOptions:ValidFor");
        }

        public async Task<LoginResponse> Handle(LoginRequest request, CancellationToken cancellationToken)
        {
            string passwordHash;

            using (SHA1 sha1Hash = SHA1.Create())
            {
                byte[] sourceBytes = Encoding.UTF8.GetBytes(request.Contraseña);
                byte[] hashBytes = sha1Hash.ComputeHash(sourceBytes);
                passwordHash = BitConverter.ToString(hashBytes).Replace("-", string.Empty);
            }

            var user = await this.context.Usuarios
                .Include(x => x.Empresa)
                .FirstOrDefaultAsync(p => p.UserName == request.Usuario && p.PasswordHash == passwordHash, cancellationToken);

            if (user == null)
                throw new ArgumentException("Usuario o contraseña incorrecto");

            if (!user.Activo)
                throw new ArgumentException("Usuario inactivo.");

            var empresaJwt = user.Empresa?.CodigoEmpresa;
            if (string.IsNullOrEmpty(empresaJwt))
                throw new ArgumentException("El usuario no tiene una empresa asignada.");

            var sucursalesUsuario = await this.context.UsuariosSucursales
                .Where(x => x.UsuarioId == user.Id)
                .ToListAsync(cancellationToken);

            if (!sucursalesUsuario.Any())
                throw new ArgumentException("El usuario no tiene sucursales asignadas. Debe tener al menos una sucursal para iniciar sesión.");

            var mainSucursal = sucursalesUsuario.FirstOrDefault(x => x.EsMain);
            if (mainSucursal == null)
                //throw new ArgumentException("El usuario no tiene una sucursal principal asignada (esMain).");
                mainSucursal = sucursalesUsuario.FirstOrDefault();

            var sucursal = await this.context.Sucursales
                .FirstOrDefaultAsync(x => x.Id == mainSucursal.SucursalId, cancellationToken);

            if (sucursal == null)
                throw new ArgumentException("La sucursal principal asignada al usuario no existe.");

            var sucursalJwt = sucursal.CodigoSucursal;

            return new LoginResponse()
            {
                Token = this.GenerateJwt(user, empresaJwt, sucursalJwt),
                CurrentUser = new CurrentUser
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    NombreCompleto = $"{user.Nombre} {user.Apellido}",
                    RolId = user.RolId,
                    CodigoEmpresa = empresaJwt,
                    NombreEmpresa = user.Empresa?.Nombre ?? string.Empty,
                    CodigoSucursal = sucursalJwt
                }
            };
        }

        private string GenerateJwt(Usuario user, string codigoEmpresa, string codigoSucursal)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(this.jwtSigningKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString(), ClaimValueTypes.String),
                    new Claim(ClaimTypes.Name, user.UserName, ClaimValueTypes.String),
                    new Claim(ClaimTypes.Role, user.RolId ?? string.Empty, ClaimValueTypes.String),
                    new Claim(ClaimTypes.PrimarySid, codigoEmpresa, ClaimValueTypes.String),
                    new Claim(ClaimTypes.PrimaryGroupSid, codigoSucursal, ClaimValueTypes.String)
                }),
                Expires = DateTime.UtcNow.AddMinutes(this.validFor),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}