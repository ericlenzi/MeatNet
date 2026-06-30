using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Meat.Application.Provincias.GetProvincias;
using System.Threading.Tasks;

namespace Meat.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class ProvinciasController : MeatBaseController
    {
        public ProvinciasController(IMediator mediator) : base(mediator) { }

        [HttpGet]
        public async Task<IActionResult> GetAsync() =>
            await this.Handle(new GetProvinciasRequest());
    }
}
