using MediatR;
using Meat.Application.Denticiones.CreateDenticion;
using Meat.Application.Denticiones.DeleteDenticion;
using Meat.Application.Denticiones.GetDenticion;
using Meat.Application.Denticiones.GetDenticiones;
using Meat.Application.Denticiones.UpdateDenticion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Meat.Controllers
{
    /// <summary>
    /// Catalogo de denticiones por especie: el recuento de incisivos permanentes con el que se
    /// estima la edad del animal. Es uno de los cuatro datos que el tipificador registra en el
    /// palco, comun a todas las empresas, asi que lo mantiene el SUPERADMIN desde la empresa
    /// administrativa.
    ///
    /// La lectura queda abierta a cualquier usuario autenticado: el Tipificador lo necesita para
    /// llenar su combo al romanear.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Authorize()]
    public class DenticionesController : MeatBaseController
    {
        public DenticionesController(IMediator mediator)
            : base(mediator)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery] GetDenticionesRequest request)
            => await this.Handle(request);

        [HttpGet("{codigo}")]
        public async Task<IActionResult> GetByCodigoAsync([FromRoute] string codigo) =>
            await this.Handle(new GetDenticionRequest { Codigo = codigo });

        [HttpPost]
        [Authorize(Roles = "SUPERADMIN")]
        public async Task<IActionResult> CreateAsync([FromBody] CreateDenticionRequest request) =>
            await this.Handle(request);

        [HttpPut("{codigo}")]
        [Authorize(Roles = "SUPERADMIN")]
        public async Task<IActionResult> UpdateAsync([FromRoute] string codigo, [FromBody] UpdateDenticionRequestFromBody body) =>
            await this.Handle(new UpdateDenticionRequest
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
            await this.Handle(new DeleteDenticionRequest { Codigo = codigo });
    }
}
