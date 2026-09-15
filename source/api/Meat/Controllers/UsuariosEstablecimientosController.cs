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

        // La app la lee al entrar para el usuario logueado, sea cual sea su rol: cada uno ve los
        // suyos y el ADMIN los de cualquier usuario de su empresa.
        [HttpGet]
        public async Task<IActionResult> GetAsync([FromRoute] Guid usuarioId)
        {
            if (usuarioId != base.CurrentUser.Id && !this.User.IsInRole("ADMIN"))
                return this.Forbid();

            return await this.Handle(
                new GetUsuarioEstablecimientosRequest { Id = usuarioId, EmpresaId = base.CurrentUser.EmpresaId }
            );
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> AddAsync([FromRoute] Guid usuarioId, [FromBody] AddUsuarioEstablecimientoBody body) => await this.Handle(
            new AddUsuarioEstablecimientoRequest
            {
                UsuarioId = usuarioId,
                EstablecimientoId = body.EstablecimientoId,
                EsMain = body.EsMain
            }
        );

        [HttpPatch("{id}/SetMain")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> SetMainAsync([FromRoute] Guid usuarioId, [FromRoute] Guid id) => await this.Handle(
            new SetMainUsuarioEstablecimientoRequest { UsuarioId = usuarioId, UsuarioEstablecimientoId = id }
        );

        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> RemoveAsync([FromRoute] Guid usuarioId, [FromRoute] Guid id) => await this.Handle(
            new RemoveUsuarioEstablecimientoRequest { UsuarioEstablecimientoId = id }
        );
    }
}
