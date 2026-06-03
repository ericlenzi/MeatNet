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

        [HttpGet]
        public async Task<IActionResult> GetAsync([FromRoute] Guid usuarioId) => await this.Handle(
            new GetUsuarioSucursalesRequest { UsuarioId = usuarioId, CodigoEmpresa = base.CurrentUser.CodigoEmpresa }
        );

        [HttpPost]
        public async Task<IActionResult> AddAsync([FromRoute] Guid usuarioId, [FromBody] AddUsuarioSucursalBody body) => await this.Handle(
            new AddUsuarioSucursalRequest
            {
                UsuarioId = usuarioId,
                SucursalId = body.SucursalId,
                EsMain = body.EsMain
            }
        );

        [HttpPatch("{id}/SetMain")]
        public async Task<IActionResult> SetMainAsync([FromRoute] Guid usuarioId, [FromRoute] Guid id) => await this.Handle(
            new SetMainUsuarioSucursalRequest { UsuarioId = usuarioId, UsuarioSucursalId = id }
        );

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveAsync([FromRoute] Guid usuarioId, [FromRoute] Guid id) => await this.Handle(
            new RemoveUsuarioSucursalRequest { UsuarioSucursalId = id }
        );
    }
}
