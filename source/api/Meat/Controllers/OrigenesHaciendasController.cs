using Meat.Application.OrigenesHaciendas.GetOrigenesHaciendas;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Meat.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize()]
    public class OrigenesHaciendasController : MeatBaseController
    {
        public OrigenesHaciendasController(IMediator mediator)
            : base(mediator)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetOrigenesHaciendasAsync() => await this.Handle(new GetOrigenesHaciendasRequest());
    }
}
