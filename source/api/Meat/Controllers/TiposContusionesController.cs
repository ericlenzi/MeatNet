using MediatR;
using Meat.Application.TiposContusiones.CreateTipoContusion;
using Meat.Application.TiposContusiones.DeleteTipoContusion;
using Meat.Application.TiposContusiones.GetTipoContusion;
using Meat.Application.TiposContusiones.GetTiposContusiones;
using Meat.Application.TiposContusiones.UpdateTipoContusion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Meat.Controllers
{
    /// <summary>
    /// Catalogo de grados de contusion por especie: el golpe visible en la media res. Es uno de
    /// los cuatro datos que el tipificador registra en el palco, comun a todas las empresas, asi
    /// que lo mantiene el SUPERADMIN desde la empresa administrativa.
    ///
    /// La lectura queda abierta a cualquier usuario autenticado: el Tipificador lo necesita para
    /// llenar su combo al romanear.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Authorize()]
    public class TiposContusionesController : MeatBaseController
    {
        public TiposContusionesController(IMediator mediator)
            : base(mediator)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery] GetTiposContusionesRequest request)
            => await this.Handle(request);

        [HttpGet("{codigo}")]
        public async Task<IActionResult> GetByCodigoAsync([FromRoute] string codigo) =>
            await this.Handle(new GetTipoContusionRequest { Codigo = codigo });

        [HttpPost]
        [Authorize(Roles = "SUPERADMIN")]
        public async Task<IActionResult> CreateAsync([FromBody] CreateTipoContusionRequest request) =>
            await this.Handle(request);

        [HttpPut("{codigo}")]
        [Authorize(Roles = "SUPERADMIN")]
        public async Task<IActionResult> UpdateAsync([FromRoute] string codigo, [FromBody] UpdateTipoContusionRequestFromBody body) =>
            await this.Handle(new UpdateTipoContusionRequest
            {
                Codigo = codigo,
                Nombre = body.Nombre,
                EspecieId = body.EspecieId,
                Orden = body.Orden,
                Activo = body.Activo,
            });

        [HttpDelete("{codigo}")]
        [Authorize(Roles = "SUPERADMIN")]
        public async Task<IActionResult> DeleteAsync([FromRoute] string codigo) =>
            await this.Handle(new DeleteTipoContusionRequest { Codigo = codigo });
    }
}
