using MediatR;
using Meat.Application.TiposPuestos.CreateTipoPuesto;
using Meat.Application.TiposPuestos.DeleteTipoPuesto;
using Meat.Application.TiposPuestos.GetTipoPuesto;
using Meat.Application.TiposPuestos.GetTiposPuestos;
using Meat.Application.TiposPuestos.UpdateTipoPuesto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Meat.Controllers
{
    /// <summary>
    /// TipoPuesto clasifica los puestos de la planta (PAL - PALCO) y es comun a todas las
    /// empresas: no lleva EmpresaId, asi que lo mantiene el SUPERADMIN desde la empresa
    /// administrativa.
    ///
    /// La lectura queda abierta a cualquier autenticado: es la FK que aparece en el alta de
    /// puestos y restringirla deja el combo vacio.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Authorize()]
    public class TiposPuestosController : MeatBaseController
    {
        public TiposPuestosController(IMediator mediator) : base(mediator) { }

        [HttpGet]
        public async Task<IActionResult> GetTiposPuestosAsync([FromQuery] GetTiposPuestosRequest request) =>
            await this.Handle(request);

        [HttpGet("{codigo}")]
        public async Task<IActionResult> GetTipoPuestoAsync([FromRoute] string codigo) =>
            await this.Handle(new GetTipoPuestoRequest { Codigo = codigo });

        [HttpPost]
        [Authorize(Roles = "SUPERADMIN")]
        public async Task<IActionResult> CreateTipoPuestoAsync([FromBody] CreateTipoPuestoRequest request) =>
            await this.Handle(request);

        [HttpPut("{codigo}")]
        [Authorize(Roles = "SUPERADMIN")]
        public async Task<IActionResult> UpdateTipoPuestoAsync([FromRoute] string codigo, [FromBody] UpdateTipoPuestoRequestFromBody body) =>
            await this.Handle(new UpdateTipoPuestoRequest
            {
                Codigo = codigo,
                Nombre = body.Nombre,
                Activo = body.Activo,
            });

        [HttpDelete("{codigo}")]
        [Authorize(Roles = "SUPERADMIN")]
        public async Task<IActionResult> DeleteTipoPuestoAsync([FromRoute] string codigo) =>
            await this.Handle(new DeleteTipoPuestoRequest { Codigo = codigo });
    }
}
