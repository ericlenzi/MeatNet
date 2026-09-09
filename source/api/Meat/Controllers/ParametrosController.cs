using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Meat.Application.Parametros.GetParametros;
using Meat.Application.Parametros.GetParametro;
using Meat.Application.Parametros.CreateParametro;
using Meat.Application.Parametros.UpdateParametro;
using Meat.Application.Parametros.DeleteParametro;
using System.Threading.Tasks;

namespace Meat.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize()]
    public class ParametrosController : MeatBaseController
    {
        public ParametrosController(IMediator mediator)
            : base(mediator)
        {
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetParametrosAsync([FromQuery] GetParametrosRequest request)
        {
            request.EmpresaId = base.CurrentUser.EmpresaId;
            return await this.Handle(request);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetParametroByIdAsync([FromRoute] Guid id) => await this.Handle(
            new GetParametroRequest
            {
                Id = id,
                EmpresaId = base.CurrentUser.EmpresaId
            }
        );

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> CreateParametroAsync([FromBody] CreateParametroRequest request)
        {
            request.EmpresaId = base.CurrentUser.EmpresaId;
            return await Handle(request);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> UpdateParametroAsync([FromRoute] Guid id, [FromBody] UpdateParametroRequestFromBody body) => await this.Handle(
            new UpdateParametroRequest()
            {
                Id = id,
                EmpresaId = base.CurrentUser.EmpresaId,
                Nombre = body.Nombre,
                Valor = body.Valor,
                Activo = body.Activo
            }
        );

        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeleteParametroByIdAsync([FromRoute] Guid id) => await this.Handle(
            new DeleteParametroRequest
            {
                Id = id,
                EmpresaId = base.CurrentUser.EmpresaId
            }
        );
    }
}
