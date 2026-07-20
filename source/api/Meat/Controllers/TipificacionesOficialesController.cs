using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Meat.Application.TipificacionesOficiales.GetTipificacionesOficiales;
using System.Threading.Tasks;

namespace Meat.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class TipificacionesOficialesController : MeatBaseController
    {
        public TipificacionesOficialesController(IMediator mediator) : base(mediator) { }

        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery] GetTipificacionesOficialesRequest request) =>
            await this.Handle(request);
    }
}
