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
        private readonly string issuer;
        private readonly string audience;

        public LoginHandler(MeatContext context, IConfiguration configuration)
        {
            if (configuration == null) { throw new ArgumentNullException(nameof(configuration)); }
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.jwtSigningKey = configuration.GetValue<string>("JwtOptions:SigninKey");
            this.validFor = configuration.GetValue<int>("JwtOptions:ValidFor");
            this.issuer = configuration.GetValue<string>("JwtOptions:Issuer") ?? "meatnet-api";
            this.audience = configuration.GetValue<string>("JwtOptions:Audience") ?? "meatnet-web";
        }

        public async Task<LoginResponse> Handle(LoginRequest request, CancellationToken cancellationToken)
        {
            // El hash lleva salt propio, asi que ya no se puede comparar dentro de la consulta:
            // se busca por usuario y se verifica en memoria.
            var user = await this.context.Usuarios
                .FirstOrDefaultAsync(p => p.UserName == request.Usuario, cancellationToken);

            var verificacion = user == null
                ? default
                : PasswordHash.Verificar(request.Contraseña, user.PasswordHash);

            if (user == null || !verificacion.EsValida)
                throw new ArgumentException("Usuario o contraseña incorrecto");

            if (!user.Activo)
                throw new ArgumentException("Usuario inactivo.");

            // La empresa del usuario es la fuente de verdad del tenant; la sucursal solo aporta
            // el contexto operativo. Ojo: aca todavia no hay empresa activa resuelta, asi que el
            // query filter del contexto no aplica y estas consultas ven todas las empresas.
            var empresaJwt = user.EmpresaId;
            if (string.IsNullOrEmpty(empresaJwt))
                throw new ArgumentException("El usuario no tiene una empresa asignada.");

            var empresa = await this.context.Empresas
                .FirstOrDefaultAsync(x => x.Id == empresaJwt, cancellationToken);

            if (empresa == null)
                throw new ArgumentException("La empresa asignada al usuario no existe.");

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

            var parametroPasswordInicial = await this.context.Parametros
                .FirstOrDefaultAsync(p => p.Codigo == "PASSWORD_INICIAL" && p.EmpresaId == empresaJwt, cancellationToken);

            // La contrasena ya se verifico, asi que alcanza con comparar lo que el usuario
            // escribio contra el parametro, que guarda la contrasena inicial en claro.
            bool debeCambiarContrasena = parametroPasswordInicial != null
                && !string.IsNullOrWhiteSpace(parametroPasswordInicial.Valor)
                && string.Equals(request.Contraseña, parametroPasswordInicial.Valor, StringComparison.Ordinal);

            // Migracion perezosa: es el unico momento en que se tiene la contrasena en claro
            // junto con un hash viejo. El usuario no se entera.
            if (verificacion.NecesitaRehash)
            {
                user.PasswordHash = PasswordHash.Calcular(request.Contraseña);
                await this.context.SaveChangesAsync(cancellationToken);
            }

            return new LoginResponse()
            {
                Token = this.GenerateJwt(user, empresaJwt, sucursalJwt),
                DebeCambiarContrasena = debeCambiarContrasena,
                CurrentUser = new CurrentUser
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    NombreCompleto = $"{user.Nombre} {user.Apellido}",
                    RolId = user.RolId,
                    EmpresaId = empresaJwt,
                    NombreEmpresa = empresa.Nombre ?? string.Empty,
                    ColorEmpresa = empresa.Color,
                    LogoEmpresa = empresa.Logo,
                    CodigoSucursal = sucursalJwt
                }
            };
        }


        /// <summary>Claim que transporta la empresa activa. Debe coincidir con HttpTenantContext.</summary>
        public const string EmpresaClaimType = "empresa_id";

        private string GenerateJwt(Usuario user, string codigoEmpresa, string codigoSucursal)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            // UTF8 y no ASCII: con ASCII cualquier caracter no ingles se convierte en '?'
            // y la clave efectiva termina siendo mas debil que la configurada.
            var key = Encoding.UTF8.GetBytes(this.jwtSigningKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString(), ClaimValueTypes.String),
                    new Claim(ClaimTypes.Name, user.UserName, ClaimValueTypes.String),
                    new Claim(ClaimTypes.Role, user.RolId ?? string.Empty, ClaimValueTypes.String),
                    new Claim(EmpresaClaimType, codigoEmpresa, ClaimValueTypes.String),
                    new Claim(ClaimTypes.PrimaryGroupSid, codigoSucursal, ClaimValueTypes.String)
                }),
                Issuer = this.issuer,
                Audience = this.audience,
                Expires = DateTime.UtcNow.AddMinutes(this.validFor),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}