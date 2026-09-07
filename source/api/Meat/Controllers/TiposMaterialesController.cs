using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Meat.Application.TiposMateriales.GetTiposMateriales;
using System.Threading.Tasks;

namespace Meat.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class TiposMaterialesController : MeatBaseController
    {
        public TiposMaterialesController(IMediator mediator) : base(mediator) { }

        [HttpGet]
        public async Task<IActionResult> GetAsync() =>
            await this.Handle(new GetTiposMaterialesRequest());
    }
}
