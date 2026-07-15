using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Meat.Application.ListasMatanzas.AgregarRenglonListaMatanza;
using Meat.Application.ListasMatanzas.CancelarListaMatanza;
using Meat.Application.ListasMatanzas.ConfirmarListaMatanza;
using Meat.Application.ListasMatanzas.CreateListaMatanza;
using Meat.Application.ListasMatanzas.DeleteListaMatanza;
using Meat.Application.ListasMatanzas.DesconfirmarListaMatanza;
using Meat.Application.ListasMatanzas.EditarRenglonListaMatanza;
using Meat.Application.ListasMatanzas.FaenaEmergenciaListaMatanza;
using Meat.Application.ListasMatanzas.QuitarRenglonListaMatanza;
using Meat.Application.ListasMatanzas.FinalizarListaMatanza;
using Meat.Application.ListasMatanzas.GetDisponibilidadFaena;
using Meat.Application.ListasMatanzas.GetListaMatanza;
using Meat.Application.ListasMatanzas.GetListasMatanzas;
using Meat.Application.ListasMatanzas.IniciarListaMatanza;
using Meat.Application.ListasMatanzas.UpdateListaMatanza;
using System;
using System.Threading.Tasks;

namespace Meat.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "ABAST,ABASTADMIN,ADMIN")]
    public class ListasMatanzasController : MeatBaseController
    {
        public ListasMatanzasController(IMediator mediator) : base(mediator) { }

        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery] GetListasMatanzasRequest request)
        {
            request.CodigoEmpresa = base.CurrentUser.CodigoEmpresa;
            return await this.Handle(request);
        }

        [HttpGet("disponibilidad")]
        public async Task<IActionResult> GetDisponibilidadAsync([FromQuery] GetDisponibilidadFaenaRequest request)
        {
            request.CodigoEmpresa = base.CurrentUser.CodigoEmpresa;
            return await this.Handle(request);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id) =>
            await this.Handle(new GetListaMatanzaRequest
            {
                Id = id,
                CodigoEmpresa = base.CurrentUser.CodigoEmpresa
            });

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateListaMatanzaRequest request)
        {
            request.CodigoEmpresa = base.CurrentUser.CodigoEmpresa;
            return await this.Handle(request);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] UpdateListaMatanzaRequest request)
        {
            request.Id = id;
            request.CodigoEmpresa = base.CurrentUser.CodigoEmpresa;
            return await this.Handle(request);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid id) =>
            await this.Handle(new DeleteListaMatanzaRequest
            {
                Id = id,
                CodigoEmpresa = base.CurrentUser.CodigoEmpresa
            });

        // --- Edicion controlada post-confirmacion (auditada) ---

        [HttpPost("{id}/renglones")]
        public async Task<IActionResult> AgregarRenglonAsync([FromRoute] Guid id, [FromBody] AgregarRenglonListaMatanzaRequest request)
        {
            request.Id = id;
            request.CodigoEmpresa = base.CurrentUser.CodigoEmpresa;
            request.UsuarioId = base.CurrentUser.Id;
            return await this.Handle(request);
        }

        [HttpPut("{id}/renglones/{renglonId}")]
        public async Task<IActionResult> EditarRenglonAsync([FromRoute] Guid id, [FromRoute] Guid renglonId, [FromBody] EditarRenglonListaMatanzaRequest request)
        {
            request.Id = id;
            request.RenglonId = renglonId;
            request.CodigoEmpresa = base.CurrentUser.CodigoEmpresa;
            request.UsuarioId = base.CurrentUser.Id;
            return await this.Handle(request);
        }

        [HttpDelete("{id}/renglones/{renglonId}")]
        public async Task<IActionResult> QuitarRenglonAsync([FromRoute] Guid id, [FromRoute] Guid renglonId) =>
            await this.Handle(new QuitarRenglonListaMatanzaRequest
            {
                Id = id,
                RenglonId = renglonId,
                CodigoEmpresa = base.CurrentUser.CodigoEmpresa,
                UsuarioId = base.CurrentUser.Id
            });

        [HttpPost("{id}/emergencia")]
        public async Task<IActionResult> FaenaEmergenciaAsync([FromRoute] Guid id, [FromBody] FaenaEmergenciaListaMatanzaRequest request)
        {
            request.Id = id;
            request.CodigoEmpresa = base.CurrentUser.CodigoEmpresa;
            request.UsuarioId = base.CurrentUser.Id;
            return await this.Handle(request);
        }

        // --- Workflow ---

        [HttpPost("{id}/confirmar")]
        public async Task<IActionResult> ConfirmarAsync([FromRoute] Guid id) =>
            await this.Handle(new ConfirmarListaMatanzaRequest
            {
                Id = id,
                CodigoEmpresa = base.CurrentUser.CodigoEmpresa,
                UsuarioId = base.CurrentUser.Id
            });

        [HttpPost("{id}/desconfirmar")]
        public async Task<IActionResult> DesconfirmarAsync([FromRoute] Guid id) =>
            await this.Handle(new DesconfirmarListaMatanzaRequest
            {
                Id = id,
                CodigoEmpresa = base.CurrentUser.CodigoEmpresa,
                UsuarioId = base.CurrentUser.Id
            });

        [HttpPost("{id}/iniciar")]
        public async Task<IActionResult> IniciarAsync([FromRoute] Guid id) =>
            await this.Handle(new IniciarListaMatanzaRequest
            {
                Id = id,
                CodigoEmpresa = base.CurrentUser.CodigoEmpresa,
                UsuarioId = base.CurrentUser.Id
            });

        [HttpPost("{id}/finalizar")]
        public async Task<IActionResult> FinalizarAsync([FromRoute] Guid id, [FromBody] FinalizarListaMatanzaRequest request)
        {
            request.Id = id;
            request.CodigoEmpresa = base.CurrentUser.CodigoEmpresa;
            request.UsuarioId = base.CurrentUser.Id;
            return await this.Handle(request);
        }

        [HttpPost("{id}/cancelar")]
        public async Task<IActionResult> CancelarAsync([FromRoute] Guid id, [FromBody] CancelarListaMatanzaRequest request)
        {
            request.Id = id;
            request.CodigoEmpresa = base.CurrentUser.CodigoEmpresa;
            request.UsuarioId = base.CurrentUser.Id;
            return await this.Handle(request);
        }
    }
}
