using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Meat.Application.ExistenciaCamara.GetExistenciaCamara;
using Meat.Application.ExistenciaCamara.GetMovimientosCamara;
using System.Threading.Tasks;

namespace Meat.Controllers
{
    /// <summary>
    /// Existencia de camara: el stock de producto que dejo la Liberacion (Ciclo I paso 4) y que
    /// va a consumir el Ciclo II. El saldo se deriva del log de movimientos.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "ABAST,ABASTADMIN,ADMIN")]
    public class ExistenciaCamaraController : MeatBaseController
    {
        public ExistenciaCamaraController(IMediator mediator) : base(mediator) { }

        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery] GetExistenciaCamaraRequest request)
        {
            request.EmpresaId = base.CurrentUser.EmpresaId;
            return await this.Handle(request);
        }

        [HttpGet("movimientos")]
        public async Task<IActionResult> GetMovimientosAsync([FromQuery] GetMovimientosCamaraRequest request)
        {
            request.EmpresaId = base.CurrentUser.EmpresaId;
            return await this.Handle(request);
        }
    }
}
