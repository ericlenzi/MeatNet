using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Meat.Application.Tropas.GetTropasDisponibles;
using Meat.Application.Tropas.GetTrazabilidadTropa;
using System.Threading.Tasks;

namespace Meat.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "ABAST,ABASTADMIN,ADMIN")]
    public class TropasController : MeatBaseController
    {
        public TropasController(IMediator mediator) : base(mediator) { }

        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery] GetTropasDisponiblesRequest request)
        {
            request.CodigoEmpresa = base.CurrentUser.CodigoEmpresa;
            return await this.Handle(request);
        }

        [HttpGet("trazabilidad")]
        public async Task<IActionResult> GetTrazabilidadAsync([FromQuery] GetTrazabilidadTropaRequest request)
        {
            request.CodigoEmpresa = base.CurrentUser.CodigoEmpresa;
            return await this.Handle(request);
        }
    }
}
