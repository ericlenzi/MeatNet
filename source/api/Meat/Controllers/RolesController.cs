using MediatR;
using Meat.Application.Roles.CreateRol;
using Meat.Application.Roles.DeleteRol;
using Meat.Application.Roles.GetRol;
using Meat.Application.Roles.GetRoles;
using Meat.Application.Roles.UpdateRol;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Meat.Controllers
{
    /// <summary>
    /// Rol es un catalogo comun a todas las empresas: no lleva EmpresaId, y ademas define
    /// que puede hacer cada quien. Lo mantiene el SUPERADMIN desde la empresa administrativa;
    /// un ADMIN no deberia poder crear roles ni redefinir el suyo.
    ///
    /// La lectura queda abierta a cualquier usuario autenticado porque el ADMIN necesita la
    /// lista para asignarle un rol a cada usuario de su empresa.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Authorize()]
    public class RolesController : MeatBaseController
    {
        public RolesController(IMediator mediator)
            : base(mediator)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetRolesAsync()
        {
            return await this.Handle(new GetRolesRequest());
        }

        [HttpGet("{codigo}")]
        public async Task<IActionResult> GetRolAsync([FromRoute] string codigo) =>
            await this.Handle(new GetRolRequest { Codigo = codigo });

        [HttpPost]
        [Authorize(Roles = "SUPERADMIN")]
        public async Task<IActionResult> CreateRolAsync([FromBody] CreateRolRequest request)
        {
            return await this.Handle(request);
        }

        [HttpPut("{codigo}")]
        [Authorize(Roles = "SUPERADMIN")]
        public async Task<IActionResult> UpdateRolAsync([FromRoute] string codigo, [FromBody] UpdateRolRequestFromBody body) =>
            await this.Handle(new UpdateRolRequest
            {
                Codigo = codigo,
                Nombre = body.Nombre,
                Activo = body.Activo
            });

        [HttpDelete("{codigo}")]
        [Authorize(Roles = "SUPERADMIN")]
        public async Task<IActionResult> DeleteRolAsync([FromRoute] string codigo) =>
            await this.Handle(new DeleteRolRequest { Codigo = codigo });
    }
}
