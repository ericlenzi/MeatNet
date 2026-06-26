using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Meat.Application.NumeradoresTropas.CreateNumeradorTropa;
using Meat.Application.NumeradoresTropas.DeleteNumeradorTropa;
using Meat.Application.NumeradoresTropas.GetNumeradoresTropas;
using Meat.Application.NumeradoresTropas.GetNumeradorTropa;
using Meat.Application.NumeradoresTropas.UpdateNumeradorTropa;
using System;
using System.Threading.Tasks;

namespace Meat.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "ADMIN")]
    public class NumeradoresTropasController : MeatBaseController
    {
        public NumeradoresTropasController(IMediator mediator) : base(mediator) { }

        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery] GetNumeradoresTropasRequest request) =>
            await this.Handle(request);

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id) =>
            await this.Handle(new GetNumeradorTropaRequest { Id = id });

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateNumeradorTropaRequest request) =>
            await this.Handle(request);

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] UpdateNumeradorTropaRequestFromBody body) =>
            await this.Handle(new UpdateNumeradorTropaRequest { Id = id, UltimoNumeroTropa = body.UltimoNumeroTropa });

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid id) =>
            await this.Handle(new DeleteNumeradorTropaRequest { Id = id });
    }
}
