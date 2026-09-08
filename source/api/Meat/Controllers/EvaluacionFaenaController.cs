using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Meat.Application.EvaluacionFaena.ActualizarPieza;
using Meat.Application.EvaluacionFaena.GetRomaneosEvaluacion;
using Meat.Application.EvaluacionFaena.LiberarJornada;
using Meat.Application.EvaluacionFaena.PrevisualizarLiberacion;
using System;
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

        [HttpGet("romaneos")]
        public async Task<IActionResult> GetRomaneosAsync([FromQuery] GetRomaneosEvaluacionRequest request)
        {
            request.EmpresaId = base.CurrentUser.EmpresaId;
            return await this.Handle(request);
        }

        [HttpPut("pieza/{id}")]
        [Authorize(Roles = "ABASTADMIN,ADMIN")]
        public async Task<IActionResult> ActualizarPiezaAsync([FromRoute] Guid id, [FromBody] ActualizarPiezaBody body) =>
            await this.Handle(new ActualizarPiezaRequest
            {
                Id = id,
                EmpresaId = base.CurrentUser.EmpresaId,
                Peso = body.Peso,
                TipificacionId = body.TipificacionId,
                AlmacenDestinoId = body.AlmacenDestinoId,
                ForzarFueraRango = body.ForzarFueraRango
            });

        [HttpGet("previsualizar")]
        public async Task<IActionResult> PrevisualizarAsync([FromQuery] PrevisualizarLiberacionRequest request)
        {
            request.EmpresaId = base.CurrentUser.EmpresaId;
            return await this.Handle(request);
        }

        [HttpPost("liberar")]
        public async Task<IActionResult> LiberarAsync([FromBody] LiberarJornadaRequest request)
        {
            request.EmpresaId = base.CurrentUser.EmpresaId;
            request.UsuarioId = base.CurrentUser.Id;
            return await this.Handle(request);
        }
    }
}
