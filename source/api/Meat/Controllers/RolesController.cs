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
            return await this.Handle(new GetRolesRequest());
        }

        [HttpGet("{codigo}")]
        public async Task<IActionResult> GetRolAsync([FromRoute] string codigo) =>
            await this.Handle(new GetRolRequest { Codigo = codigo });

        [HttpPost]
        public async Task<IActionResult> CreateRolAsync([FromBody] CreateRolRequest request)
        {
            return await this.Handle(request);
        }

        [HttpPut("{codigo}")]
        public async Task<IActionResult> UpdateRolAsync([FromRoute] string codigo, [FromBody] UpdateRolRequestFromBody body) =>
            await this.Handle(new UpdateRolRequest
            {
                Codigo = codigo,
                Nombre = body.Nombre,
                Activo = body.Activo
            });

        [HttpDelete("{codigo}")]
        public async Task<IActionResult> DeleteRolAsync([FromRoute] string codigo) =>
            await this.Handle(new DeleteRolRequest { Codigo = codigo });
    }
}
