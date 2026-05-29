using MediatR;
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

        [HttpPost]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request) => await this.Handle(request);

        [HttpPut("CambiarContraseña")]
        public async Task<IActionResult> ChangePassAsync([FromBody] CambiarContraseñaUsuarioRequest request)
        {
            request.UsuarioId = base.CurrentUser.Id;
            return await this.Handle(request);
        }
    }
}