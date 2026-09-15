using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Meat.Application.Usuarios.CreateUsuario;
using Meat.Application.Usuarios.DeleteUsuario;
using Meat.Application.Usuarios.GetUsuario;
using Meat.Application.Usuarios.GetUsuarios;
using Meat.Application.Usuarios.UpdateUsuario;
using Meat.Application.Usuarios.RestaurarPasswordUsuario;
using System;
using System.Threading.Tasks;

namespace Meat.Controllers
{
    // El ABM de usuarios es del ADMIN de cada empresa (el menu solo se lo muestra a ese rol) y el
    // query filter lo limita a los usuarios de su empresa. Cada usuario cambia su propia password
    // por /Autenticacion/CambiarContraseña.
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "ADMIN")]
    public class UsuariosController : MeatBaseController
    {
        public UsuariosController(IMediator mediator)
            : base(mediator)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetUsuariosAsync([FromQuery] GetUsuariosRequest request)
            => await this.Handle(request);

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUsuarioByIdAsync([FromRoute] Guid id) => await this.Handle(
            new GetUsuarioRequest { Id = id }
        );

        [HttpPost]
        public async Task<IActionResult> CreateUsuarioAsync([FromBody] CreateUsuarioRequest request)
            => await this.Handle(request);

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUsuarioAsync([FromRoute] Guid id, [FromBody] UpdateUsuarioRequestFromBody body) => await this.Handle(
            new UpdateUsuarioRequest()
            {
                Id = id,
                UserName = body.UserName,
                Nombre = body.Nombre,
                Apellido = body.Apellido,
                Email = body.Email,
                Legajo = body.Legajo,
                RolId = body.RolId,
                Activo = body.Activo,
            }
        );

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuarioByIdAsync([FromRoute] Guid id) => await this.Handle(
            new DeleteUsuarioRequest { Id = id }
        );

        [HttpPut("{id}/RestaurarPassword")]
        public async Task<IActionResult> RestaurarPasswordAsync([FromRoute] Guid id) => await this.Handle(
            new RestaurarPasswordUsuarioRequest
            {
                Id = id,
                EmpresaId = base.CurrentUser.EmpresaId
            }
        );
    }
}
