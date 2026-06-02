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
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "ADMIN")]
    public class RolesController : MeatBaseController
    {
        public RolesController(IMediator mediator)
            : base(mediator)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetRolesAsync()
        {
            var request = new GetRolesRequest { CodigoEmpresa = base.CurrentUser.CodigoEmpresa };
            return await this.Handle(request);
        }

        [HttpGet("{codigo}")]
        public async Task<IActionResult> GetRolAsync([FromRoute] string codigo) =>
            await this.Handle(new GetRolRequest { Codigo = codigo, CodigoEmpresa = base.CurrentUser.CodigoEmpresa });

        [HttpPost]
        public async Task<IActionResult> CreateRolAsync([FromBody] CreateRolRequest request)
        {
            request.CodigoEmpresa = base.CurrentUser.CodigoEmpresa;
            return await this.Handle(request);
        }

        [HttpPut("{codigo}")]
        public async Task<IActionResult> UpdateRolAsync([FromRoute] string codigo, [FromBody] UpdateRolRequestFromBody body) =>
            await this.Handle(new UpdateRolRequest
            {
                Codigo = codigo,
                CodigoEmpresa = base.CurrentUser.CodigoEmpresa,
                Nombre = body.Nombre,
                Activo = body.Activo
            });

        [HttpDelete("{codigo}")]
        public async Task<IActionResult> DeleteRolAsync([FromRoute] string codigo) =>
            await this.Handle(new DeleteRolRequest { Codigo = codigo, CodigoEmpresa = base.CurrentUser.CodigoEmpresa });
    }
}
