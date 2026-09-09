using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace Meat.Infrastructure
{
    public static class IdentityConfiguration
    {
        /// <summary>Quien emite el token y para quien: deben coincidir con los del LoginHandler.</summary>
        public const string IssuerPorDefecto = "meatnet-api";
        public const string AudiencePorDefecto = "meatnet-web";

        /// <summary>
        /// Claves que estan escritas en los appsettings del repositorio, o sea publicas para
        /// cualquiera que tenga acceso al codigo. Fuera de desarrollo no se aceptan.
        /// Las de juleriaque son restos de otro proyecto, nunca se cambiaron.
        /// </summary>
        private static readonly string[] ClavesVersionadas =
        {
            "key-meatnet-development-2021-mc-sha256",
            "key-juleriaque-production-2022-mc",
            "key-juleriaque-testing-2022-mc-32",
            "key-juleriaque-integration-2021-mc",
        };

        /// <summary>HMAC-SHA256 pide una clave de al menos 256 bits.</summary>
        private const int BytesMinimosDeClave = 32;

        public static IServiceCollection AddIdentityServices(
            this IServiceCollection services, IConfiguration config, bool esDesarrollo)
        {
            var clave = ObtenerClaveDeFirma(config, esDesarrollo);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = !esDesarrollo;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(clave)),

                    // Antes no se validaban: un token firmado con la misma clave por cualquier
                    // otro sistema era aceptado aca.
                    ValidateIssuer = true,
                    ValidIssuer = config.GetValue<string>("JwtOptions:Issuer") ?? IssuerPorDefecto,
                    ValidateAudience = true,
                    ValidAudience = config.GetValue<string>("JwtOptions:Audience") ?? AudiencePorDefecto,

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(1),
                };
            });

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                builder.AllowAnyOrigin()
             .AllowAnyMethod()
             .AllowAnyHeader()
             );
            });

            return services;
        }

        /// <summary>
        /// La clave sale de la configuracion, que en produccion se sobreescribe con la variable
        /// de entorno JwtOptions__SigninKey. Se valida al arrancar: una clave debil o la de
        /// desarrollo fuera de dev hacen fallar el arranque en vez de dejar la API abierta.
        /// </summary>
        private static string ObtenerClaveDeFirma(IConfiguration config, bool esDesarrollo)
        {
            var clave = config.GetValue<string>("JwtOptions:SigninKey");

            if (string.IsNullOrWhiteSpace(clave))
                throw new InvalidOperationException(
                    "Falta JwtOptions:SigninKey. En produccion se configura con la variable de "
                    + "entorno JwtOptions__SigninKey.");

            if (Encoding.UTF8.GetByteCount(clave) < BytesMinimosDeClave)
                throw new InvalidOperationException(
                    $"JwtOptions:SigninKey debe tener al menos {BytesMinimosDeClave} bytes para firmar con HMAC-SHA256.");

            if (!esDesarrollo && Array.IndexOf(ClavesVersionadas, clave) >= 0)
                throw new InvalidOperationException(
                    "JwtOptions:SigninKey es una de las claves escritas en los appsettings del "
                    + "repositorio, o sea publica. Configure una propia con la variable de entorno "
                    + "JwtOptions__SigninKey.");

            return clave;
        }
    }
}
