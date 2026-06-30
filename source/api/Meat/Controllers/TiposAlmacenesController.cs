using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Meat.Application.TiposAlmacenes.GetTiposAlmacenes;
using System.Threading.Tasks;

namespace Meat.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class TiposAlmacenesController : MeatBaseController
    {
        public TiposAlmacenesController(IMediator mediator) : base(mediator) { }

        [HttpGet]
        public async Task<IActionResult> GetAsync() =>
            await this.Handle(new GetTiposAlmacenesRequest());
    }
}
