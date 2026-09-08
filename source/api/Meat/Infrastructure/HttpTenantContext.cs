using Meat.Domain.Shared;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Security.Claims;

namespace Meat.Infrastructure
{
    /// <summary>
    /// Resuelve la empresa activa desde el claim del JWT de la request en curso.
    /// Devuelve null si no hay usuario autenticado (login, o el contexto usado fuera de
    /// una request, como las migraciones al arrancar): en ese caso el MeatContext no filtra
    /// por empresa.
    /// </summary>
    public class HttpTenantContext : ITenantContext
    {
        /// <summary>Claim que transporta la empresa activa en el JWT.</summary>
        public const string EmpresaClaimType = "empresa_id";

        private readonly IHttpContextAccessor httpContextAccessor;

        public HttpTenantContext(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public string EmpresaId
        {
            get
            {
                var identity = this.httpContextAccessor.HttpContext?.User?.Identity as ClaimsIdentity;

                if (identity == null || !identity.IsAuthenticated)
                {
                    return null;
                }

                return identity.Claims
                    .Where(c => c.Type == EmpresaClaimType)
                    .Select(c => c.Value)
                    .FirstOrDefault();
            }
        }
    }
}
