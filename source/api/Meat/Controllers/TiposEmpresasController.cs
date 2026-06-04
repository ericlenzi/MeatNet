using Meat.Application.TiposEmpresas.GetTiposEmpresas;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Meat.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize()]
    public class TiposEmpresasController : MeatBaseController
    {
        public TiposEmpresasController(IMediator mediator)
            : base(mediator)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetTiposEmpresasAsync() => await this.Handle(new GetTiposEmpresasRequest());
    }
}
