using Meat.Application.UsosHaciendas.GetUsosHaciendas;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Meat.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize()]
    public class UsosHaciendasController : MeatBaseController
    {
        public UsosHaciendasController(IMediator mediator)
            : base(mediator)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetUsosHaciendasAsync() => await this.Handle(new GetUsosHaciendasRequest());
    }
}
