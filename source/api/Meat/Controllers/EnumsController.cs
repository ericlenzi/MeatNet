namespace Meat.Controllers
{
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using Meat.Application.Enums.GetSexos;
    using Meat.Application.TiposSexos.GetTiposSexos;
    using System.Threading.Tasks;

    [ApiController]
    [Route("[controller]")]
    public class EnumsController : MeatBaseController
    {
        public EnumsController(IMediator mediator)
            : base(mediator)
        {
        }

        [HttpGet("sexos")]
        public async Task<IActionResult> GetSexosAsync([FromRoute] GetSexosRequest request) => await this.Handle(request);

        [HttpGet("diasSemana")]
        public async Task<IActionResult> GetDiasSemanaAsync([FromRoute] GetDiasSemanaRequest request) => await this.Handle(request);

        [HttpGet("tiposSexos")]
        public async Task<IActionResult> GetTiposSexosAsync() => await this.Handle(new GetTiposSexosRequest());
    }
}