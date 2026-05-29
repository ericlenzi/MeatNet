using Meat.Application.Parametros.CreateParametro;
using Meat.Application.Parametros.DeleteParametro;
using Meat.Application.Parametros.GetParametro;
using Meat.Application.Parametros.GetParametros;
using Meat.Application.Parametros.UpdateParametro;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Meat.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "ADMIN, ABAST")]
    public class ParametrosController : MeatBaseController
    {
        public ParametrosController(IMediator mediator)
            : base(mediator)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetParametrosAsync([FromQuery] GetParametrosRequest request)
        {
            return await this.Handle(request);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetParametroByIdAsync([FromRoute] Guid id) => await this.Handle(
            new GetParametroRequest
            {
                Id = id,
            }
        );

        [HttpPost]
        public async Task<IActionResult> CreateParametroAsync([FromBody] CreateParametroRequest request) => await Handle(request);

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateParametroAsync([FromRoute] Guid id, [FromBody] UpdateParametroRequestFromBody body) => await this.Handle(
            new UpdateParametroRequest()
            {
                Id = id,
                Codigo = body.Codigo,
                Valor = body.Valor
            }
        );

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteParametroByIdAsync([FromRoute] Guid id) => await this.Handle(
            new DeleteParametroRequest
            {
                Id = id,
            }
        );
    }
}
