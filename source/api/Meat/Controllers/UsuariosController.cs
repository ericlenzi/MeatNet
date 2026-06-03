using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Meat.Application.Usuarios.CreateUsuario;
using Meat.Application.Usuarios.DeleteUsuario;
using Meat.Application.Usuarios.GetUsuario;
using Meat.Application.Usuarios.GetUsuarios;
using Meat.Application.Usuarios.IsAutorizador;
using Meat.Application.Usuarios.UpdateUsuario;
using Meat.Application.Usuarios.ImportUsuarios;
using System;
using System.Threading.Tasks;
using Meat.Application.Usuarios.ResetearPasswordUsuario;
using Meat.Application.Usuarios.RestaurarPasswordUsuario;
namespace Meat.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsuariosController : MeatBaseController
    {
        public UsuariosController(IMediator mediator)
            : base(mediator)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetUsuariosAsync([FromQuery] GetUsuariosRequest request)
        {
            request.CodigoEmpresa = base.CurrentUser.CodigoEmpresa;
            return await this.Handle(request);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUsuarioByIdAsync([FromRoute] Guid id) => await this.Handle(
            new GetUsuarioRequest
            {
                Id = id,
                CodigoEmpresa = base.CurrentUser.CodigoEmpresa
            }
        );

        [HttpPost]
        public async Task<IActionResult> CreateUsuarioAsync([FromBody] CreateUsuarioRequest request)
        {
            request.CodigoEmpresa = base.CurrentUser.CodigoEmpresa;
            return await this.Handle(request);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUsuarioAsync([FromRoute] Guid id, [FromBody] UpdateUsuarioRequestFromBody body) => await this.Handle(
            new UpdateUsuarioRequest()
            {
                Id = id,
                CodigoEmpresa = base.CurrentUser.CodigoEmpresa,
                UserName = body.UserName,
                Nombre = body.Nombre,
                Apellido = body.Apellido,
                Email = body.Email,
                Legajo = body.Legajo,
                RolId = body.RolId,
                EmpresaId = body.EmpresaId,
                Activo = body.Activo,
            }
        );

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuarioByIdAsync([FromRoute] Guid id) => await this.Handle(
            new DeleteUsuarioRequest
            {
                Id = id,
                CodigoEmpresa = base.CurrentUser.CodigoEmpresa
            }
        );

        [HttpPost("IsAutorizador")]
        public async Task<IActionResult> IsAutorizador([FromBody] IsAutorizadorRequest request) => await this.Handle(request);

        [HttpPost("Import")]
        [AllowAnonymous]
        public async Task<IActionResult> ImportUsuariosAsync([FromBody] ImportUsuariosRequest request) => await this.Handle(request);

        [HttpPut("ResetearPassword")]
        public async Task<IActionResult> ChangePassAsync([FromBody] ResetearPasswordUsuarioRequest request)
        {
            return await this.Handle(request);
        }

        [HttpPut("{id}/RestaurarPassword")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> RestaurarPasswordAsync([FromRoute] Guid id) => await this.Handle(
            new RestaurarPasswordUsuarioRequest
            {
                UsuarioId = id,
                CodigoEmpresa = base.CurrentUser.CodigoEmpresa
            }
        );
    }
}