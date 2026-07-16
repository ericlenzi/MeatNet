using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Meat.Application.IngresosHaciendas.AnularIngresoHacienda;
using Meat.Application.IngresosHaciendas.AprobarIngresoHacienda;
using Meat.Application.IngresosHaciendas.CreateIngresoHacienda;
using Meat.Application.IngresosHaciendas.DeleteIngresoHacienda;
using Meat.Application.IngresosHaciendas.EnviarAprobacionIngresoHacienda;
using Meat.Application.IngresosHaciendas.GetIngresoHacienda;
using Meat.Application.IngresosHaciendas.GetIngresosHaciendas;
using Meat.Application.IngresosHaciendas.RechazarIngresoHacienda;
using Meat.Application.IngresosHaciendas.UpdateIngresoHacienda;
using System;
using System.Threading.Tasks;

namespace Meat.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "ABAST,ABASTADMIN,ADMIN")]
    public class IngresosHaciendaController : MeatBaseController
    {
        public IngresosHaciendaController(IMediator mediator) : base(mediator) { }

        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery] GetIngresosHaciendasRequest request)
        {
            request.CodigoEmpresa = base.CurrentUser.CodigoEmpresa;
            return await this.Handle(request);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id) =>
            await this.Handle(new GetIngresoHaciendaRequest
            {
                Id = id,
                CodigoEmpresa = base.CurrentUser.CodigoEmpresa
            });

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateIngresoHaciendaRequest request)
        {
            request.CodigoEmpresa = base.CurrentUser.CodigoEmpresa;
            return await this.Handle(request);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] UpdateIngresoHaciendaRequest request)
        {
            request.Id = id;
            request.CodigoEmpresa = base.CurrentUser.CodigoEmpresa;
            return await this.Handle(request);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid id) =>
            await this.Handle(new DeleteIngresoHaciendaRequest
            {
                Id = id,
                CodigoEmpresa = base.CurrentUser.CodigoEmpresa
            });

        // --- Workflow ---

        [HttpPost("{id}/enviar-aprobacion")]
        public async Task<IActionResult> EnviarAprobacionAsync([FromRoute] Guid id) =>
            await this.Handle(new EnviarAprobacionIngresoHaciendaRequest
            {
                Id = id,
                CodigoEmpresa = base.CurrentUser.CodigoEmpresa
            });

        [HttpPost("{id}/aprobar")]
        [Authorize(Roles = "ABASTADMIN,ADMIN")]
        public async Task<IActionResult> AprobarAsync([FromRoute] Guid id) =>
            await this.Handle(new AprobarIngresoHaciendaRequest
            {
                Id = id,
                CodigoEmpresa = base.CurrentUser.CodigoEmpresa,
                UsuarioId = base.CurrentUser.Id
            });

        [HttpPost("{id}/rechazar")]
        [Authorize(Roles = "ABASTADMIN,ADMIN")]
        public async Task<IActionResult> RechazarAsync([FromRoute] Guid id) =>
            await this.Handle(new RechazarIngresoHaciendaRequest
            {
                Id = id,
                CodigoEmpresa = base.CurrentUser.CodigoEmpresa
            });

        [HttpPost("{id}/anular")]
        [Authorize(Roles = "ABASTADMIN,ADMIN")]
        public async Task<IActionResult> AnularAsync([FromRoute] Guid id) =>
            await this.Handle(new AnularIngresoHaciendaRequest
            {
                Id = id,
                CodigoEmpresa = base.CurrentUser.CodigoEmpresa,
                UsuarioId = base.CurrentUser.Id
            });
    }
}
