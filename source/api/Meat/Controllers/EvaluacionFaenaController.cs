using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Meat.Application.EvaluacionFaena.LiberarJornada;
using Meat.Application.EvaluacionFaena.PrevisualizarLiberacion;
using System.Threading.Tasks;

namespace Meat.Controllers
{
    /// <summary>
    /// Evaluacion de Faena (Ciclo I paso 4): cierre de la jornada y nacimiento de la existencia
    /// de camara. Ver docs/manuales/EvaluacionFaena.md.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "ABAST,ABASTADMIN,ADMIN")]
    public class EvaluacionFaenaController : MeatBaseController
    {
        public EvaluacionFaenaController(IMediator mediator) : base(mediator) { }

        [HttpGet("previsualizar")]
        public async Task<IActionResult> PrevisualizarAsync([FromQuery] PrevisualizarLiberacionRequest request)
        {
            request.CodigoEmpresa = base.CurrentUser.CodigoEmpresa;
            return await this.Handle(request);
        }

        [HttpPost("liberar")]
        public async Task<IActionResult> LiberarAsync([FromBody] LiberarJornadaRequest request)
        {
            request.CodigoEmpresa = base.CurrentUser.CodigoEmpresa;
            request.UsuarioId = base.CurrentUser.Id;
            return await this.Handle(request);
        }
    }
}
