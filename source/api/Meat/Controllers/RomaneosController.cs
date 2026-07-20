using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Meat.Application.Romaneos.AnularRomaneo;
using Meat.Application.Romaneos.CrearRomaneo;
using Meat.Application.Romaneos.GetMonitorFaena;
using Meat.Application.Romaneos.GetRenglonesEjecucion;
using Meat.Application.Romaneos.GetRomaneosJornada;
using Meat.Application.Romaneos.SugerirTipificacion;
using System;
using System.Threading.Tasks;

namespace Meat.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "ABAST,ABASTADMIN,ADMIN")]
    public class RomaneosController : MeatBaseController
    {
        public RomaneosController(IMediator mediator) : base(mediator) { }

        [HttpGet("renglones")]
        public async Task<IActionResult> GetRenglonesAsync([FromQuery] GetRenglonesEjecucionRequest request)
        {
            request.CodigoEmpresa = base.CurrentUser.CodigoEmpresa;
            return await this.Handle(request);
        }

        [HttpGet("sugerir-tipificacion")]
        public async Task<IActionResult> SugerirTipificacionAsync([FromQuery] SugerirTipificacionRequest request)
        {
            request.CodigoEmpresa = base.CurrentUser.CodigoEmpresa;
            return await this.Handle(request);
        }

        [HttpGet("jornada")]
        public async Task<IActionResult> GetJornadaAsync([FromQuery] GetRomaneosJornadaRequest request)
        {
            request.CodigoEmpresa = base.CurrentUser.CodigoEmpresa;
            return await this.Handle(request);
        }

        [HttpGet("monitor")]
        public async Task<IActionResult> GetMonitorAsync([FromQuery] GetMonitorFaenaRequest request)
        {
            request.CodigoEmpresa = base.CurrentUser.CodigoEmpresa;
            return await this.Handle(request);
        }

        [HttpPost]
        public async Task<IActionResult> CrearAsync([FromBody] CrearRomaneoRequest request)
        {
            request.CodigoEmpresa = base.CurrentUser.CodigoEmpresa;
            request.UsuarioId = base.CurrentUser.Id;
            return await this.Handle(request);
        }

        [HttpPost("{id}/anular")]
        public async Task<IActionResult> AnularAsync([FromRoute] Guid id, [FromBody] AnularRomaneoRequest request)
        {
            request.Id = id;
            request.CodigoEmpresa = base.CurrentUser.CodigoEmpresa;
            request.UsuarioId = base.CurrentUser.Id;
            return await this.Handle(request);
        }
    }
}
