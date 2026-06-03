using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Meat.Application.Usuarios.GetUsuarioEstablecimientos;
using Meat.Application.Usuarios.AddUsuarioEstablecimiento;
using Meat.Application.Usuarios.RemoveUsuarioEstablecimiento;
using Meat.Application.Usuarios.SetMainUsuarioEstablecimiento;
using System;
using System.Threading.Tasks;

namespace Meat.Controllers
{
    [ApiController]
    [Route("Usuarios/{usuarioId}/Establecimientos")]
    [Authorize()]
    public class UsuariosEstablecimientosController : MeatBaseController
    {
        public UsuariosEstablecimientosController(IMediator mediator)
            : base(mediator)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync([FromRoute] Guid usuarioId) => await this.Handle(
            new GetUsuarioEstablecimientosRequest { UsuarioId = usuarioId, CodigoEmpresa = base.CurrentUser.CodigoEmpresa }
        );

        [HttpPost]
        public async Task<IActionResult> AddAsync([FromRoute] Guid usuarioId, [FromBody] AddUsuarioEstablecimientoBody body) => await this.Handle(
            new AddUsuarioEstablecimientoRequest
            {
                UsuarioId = usuarioId,
                EstablecimientoId = body.EstablecimientoId,
                EsMain = body.EsMain
            }
        );

        [HttpPatch("{id}/SetMain")]
        public async Task<IActionResult> SetMainAsync([FromRoute] Guid usuarioId, [FromRoute] Guid id) => await this.Handle(
            new SetMainUsuarioEstablecimientoRequest { UsuarioId = usuarioId, UsuarioEstablecimientoId = id }
        );

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveAsync([FromRoute] Guid usuarioId, [FromRoute] Guid id) => await this.Handle(
            new RemoveUsuarioEstablecimientoRequest { UsuarioEstablecimientoId = id }
        );
    }
}
