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
            request.CodigoEmpresa = base.CurrentUser.CodigoEmpresa;
            return await this.Handle(request);
        }

        [HttpGet("{codigo}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetParametroByCodigoAsync([FromRoute] string codigo) => await this.Handle(
            new GetParametroRequest
            {
                Codigo = codigo,
                CodigoEmpresa = base.CurrentUser.CodigoEmpresa
            }
        );

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> CreateParametroAsync([FromBody] CreateParametroRequest request)
        {
            request.CodigoEmpresa = base.CurrentUser.CodigoEmpresa;
            return await Handle(request);
        }

        [HttpPut("{codigo}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> UpdateParametroAsync([FromRoute] string codigo, [FromBody] UpdateParametroRequestFromBody body) => await this.Handle(
            new UpdateParametroRequest()
            {
                Codigo = codigo,
                CodigoEmpresa = base.CurrentUser.CodigoEmpresa,
                Nombre = body.Nombre,
                Valor = body.Valor,
                Activo = body.Activo
            }
        );

        [HttpDelete("{codigo}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeleteParametroByCodigoAsync([FromRoute] string codigo) => await this.Handle(
            new DeleteParametroRequest
            {
                Codigo = codigo,
                CodigoEmpresa = base.CurrentUser.CodigoEmpresa
            }
        );
    }
}
