using Meat.Application.TiposClientes.GetTiposClientes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Meat.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize()]
    public class TiposClientesController : MeatBaseController
    {
        public TiposClientesController(IMediator mediator)
            : base(mediator)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetTiposClientesAsync() => await this.Handle(new GetTiposClientesRequest());
    }
}
