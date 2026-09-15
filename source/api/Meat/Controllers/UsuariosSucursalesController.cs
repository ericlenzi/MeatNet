using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Meat.Application.Usuarios.GetUsuarioSucursales;
using Meat.Application.Usuarios.AddUsuarioSucursal;
using Meat.Application.Usuarios.RemoveUsuarioSucursal;
using Meat.Application.Usuarios.SetMainUsuarioSucursal;
using System;
using System.Threading.Tasks;

namespace Meat.Controllers
{
    [ApiController]
    [Route("Usuarios/{usuarioId}/Sucursales")]
    [Authorize()]
    public class UsuariosSucursalesController : MeatBaseController
    {
        public UsuariosSucursalesController(IMediator mediator)
            : base(mediator)
        {
        }

        // La app la lee al entrar para el usuario logueado, sea cual sea su rol: cada uno ve las
        // suyas y el ADMIN las de cualquier usuario de su empresa.
        [HttpGet]
        public async Task<IActionResult> GetAsync([FromRoute] Guid usuarioId)
        {
            if (usuarioId != base.CurrentUser.Id && !this.User.IsInRole("ADMIN"))
                return this.Forbid();

            return await this.Handle(
                new GetUsuarioSucursalesRequest { Id = usuarioId, EmpresaId = base.CurrentUser.EmpresaId }
            );
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> AddAsync([FromRoute] Guid usuarioId, [FromBody] AddUsuarioSucursalBody body) => await this.Handle(
            new AddUsuarioSucursalRequest
            {
                UsuarioId = usuarioId,
                SucursalId = body.SucursalId,
                EsMain = body.EsMain
            }
        );

        [HttpPatch("{id}/SetMain")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> SetMainAsync([FromRoute] Guid usuarioId, [FromRoute] Guid id) => await this.Handle(
            new SetMainUsuarioSucursalRequest { UsuarioId = usuarioId, UsuarioSucursalId = id }
        );

        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> RemoveAsync([FromRoute] Guid usuarioId, [FromRoute] Guid id) => await this.Handle(
            new RemoveUsuarioSucursalRequest { UsuarioSucursalId = id }
        );
    }
}
