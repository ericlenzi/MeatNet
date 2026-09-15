using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Meat.Application.Autenticacion;
using Meat.Application.Usuarios.CambiarContraseñaUsuario;
using System.Threading.Tasks;

namespace Meat.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AutenticacionController : MeatBaseController
    {
        public AutenticacionController(IMediator mediator)
            : base(mediator)
        {
        }

        // Unico endpoint anonimo de la API: el resto exige usuario autenticado (FallbackPolicy).
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request) => await this.Handle(request);

        // Cada usuario cambia su propia password: el Id sale del token, no del cuerpo.
        [HttpPut("CambiarContraseña")]
        public async Task<IActionResult> ChangePassAsync([FromBody] CambiarContraseñaUsuarioRequest request)
        {
            request.UsuarioId = base.CurrentUser.Id;
            return await this.Handle(request);
        }
    }
}
