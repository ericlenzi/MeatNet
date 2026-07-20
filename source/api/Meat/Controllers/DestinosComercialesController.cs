using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Meat.Application.DestinosComerciales.GetDestinosComerciales;
using System.Threading.Tasks;

namespace Meat.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class DestinosComercialesController : MeatBaseController
    {
        public DestinosComercialesController(IMediator mediator) : base(mediator) { }

        [HttpGet]
        public async Task<IActionResult> GetAsync() =>
            await this.Handle(new GetDestinosComercialesRequest());
    }
}
