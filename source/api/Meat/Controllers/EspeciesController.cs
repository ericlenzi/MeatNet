using Meat.Application.Especies.GetEspecies;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Meat.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize()]
    public class EspeciesController : MeatBaseController
    {
        public EspeciesController(IMediator mediator)
            : base(mediator)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetEspeciesAsync() => await this.Handle(new GetEspeciesRequest());
    }
}
