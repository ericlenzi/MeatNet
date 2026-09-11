using MediatR;
using Meat.Application.TiposMediciones.CreateTipoMedicion;
using Meat.Application.TiposMediciones.DeleteTipoMedicion;
using Meat.Application.TiposMediciones.GetTipoMedicion;
using Meat.Application.TiposMediciones.GetTiposMediciones;
using Meat.Application.TiposMediciones.UpdateTipoMedicion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Meat.Controllers
{
    /// <summary>
    /// TipoMedicion es el metodo con el que se toma la medicion en el puesto (M manual,
    /// B balanza, A automatica). Es comun a todas las empresas, asi que lo mantiene el
    /// SUPERADMIN desde la empresa administrativa.
    ///
    /// La lectura queda abierta a cualquier autenticado: el Tipificador la necesita para la
    /// cabecera del romaneo y el alta de puestos para su valor por defecto.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Authorize()]
    public class TiposMedicionesController : MeatBaseController
    {
        public TiposMedicionesController(IMediator mediator) : base(mediator) { }

        [HttpGet]
        public async Task<IActionResult> GetTiposMedicionesAsync([FromQuery] GetTiposMedicionesRequest request) =>
            await this.Handle(request);

        [HttpGet("{codigo}")]
        public async Task<IActionResult> GetTipoMedicionAsync([FromRoute] string codigo) =>
            await this.Handle(new GetTipoMedicionRequest { Codigo = codigo });

        [HttpPost]
        [Authorize(Roles = "SUPERADMIN")]
        public async Task<IActionResult> CreateTipoMedicionAsync([FromBody] CreateTipoMedicionRequest request) =>
            await this.Handle(request);

        [HttpPut("{codigo}")]
        [Authorize(Roles = "SUPERADMIN")]
        public async Task<IActionResult> UpdateTipoMedicionAsync([FromRoute] string codigo, [FromBody] UpdateTipoMedicionRequestFromBody body) =>
            await this.Handle(new UpdateTipoMedicionRequest
            {
                Codigo = codigo,
                Nombre = body.Nombre,
                Activo = body.Activo,
            });

        [HttpDelete("{codigo}")]
        [Authorize(Roles = "SUPERADMIN")]
        public async Task<IActionResult> DeleteTipoMedicionAsync([FromRoute] string codigo) =>
            await this.Handle(new DeleteTipoMedicionRequest { Codigo = codigo });
    }
}
