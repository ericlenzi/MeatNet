using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Meat.Application.UnidadesMedidas.GetUnidadesMedidas;
using System.Threading.Tasks;

namespace Meat.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class UnidadesMedidasController : MeatBaseController
    {
        public UnidadesMedidasController(IMediator mediator) : base(mediator) { }

        [HttpGet]
        public async Task<IActionResult> GetAsync() =>
            await this.Handle(new GetUnidadesMedidasRequest());
    }
}
