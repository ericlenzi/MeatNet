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
using Meat.Domain.UsuariosSucursales;
using System.Collections.Generic;

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
            string sucursalJwt = string.Empty;
            string empresaJwt = string.Empty;
            string passwordHash;

            using (SHA1 sha1Hash = SHA1.Create())
            {
                var newPassword = request.Contraseña;
                byte[] sourceBytes = Encoding.UTF8.GetBytes(newPassword);
                byte[] hashBytes = sha1Hash.ComputeHash(sourceBytes);
                passwordHash = BitConverter.ToString(hashBytes).Replace("-", string.Empty);
            }

            var user = await this.context.Usuarios
                .Include(x => x.Empresa)
                .FirstOrDefaultAsync(p => p.UserName == request.Usuario && p.PasswordHash == passwordHash
            );

            if (user == null)
                throw new ArgumentException("Usuario o contraseña incorrecto");

            if (!user.Activo)
                throw new ArgumentException("Usuario inactivo.");

            empresaJwt = user.Empresa.CodigoEmpresa ?? string.Empty;
            sucursalJwt = this.getMainSucursalActiva(user.Id) ?? string.Empty;

            if (string.IsNullOrEmpty(empresaJwt) || string.IsNullOrEmpty(sucursalJwt))
                throw new ArgumentException("Usuario sin configuración empresa-sucursal");

            return new LoginResponse()
            {
                Token = !string.IsNullOrEmpty(empresaJwt) ? this.GenerateJwt(user, empresaJwt, sucursalJwt ) : string.Empty,
                CurrentUser = new CurrentUser
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    NombreCompleto = $"{user.Nombre} {user.Apellido}",
                    RolId = user.RolId,
                    CodigoEmpresa = empresaJwt,
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

        private string getMainSucursalActiva(Guid? userid)
        {
            string sucursal = string.Empty;
            List<UsuarioSucursal> usersucs = this.context.UsuariosSucursales.Where(x => x.UsuarioId == userid).ToList();
            foreach (UsuarioSucursal item in usersucs)
            {
                if (item.esMain)
                {
                    sucursal = this.context.Sucursales.FirstOrDefault(x => x.Id == item.SucursalId).CodigoSucursal;
                }

            }
            return sucursal;
        }
    }
}