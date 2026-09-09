using MediatR;
using Meat.Application.Conformaciones.CreateConformacion;
using Meat.Application.Conformaciones.DeleteConformacion;
using Meat.Application.Conformaciones.GetConformacion;
using Meat.Application.Conformaciones.GetConformaciones;
using Meat.Application.Conformaciones.UpdateConformacion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Meat.Controllers
{
    /// <summary>
    /// Catalogo de grados de conformacion por especie: el desarrollo muscular de la media res. Es uno de los ejes de la
    /// tipificacion oficial, comun a todas las empresas, asi que lo mantiene el SUPERADMIN desde
    /// la empresa administrativa.
    ///
    /// La lectura queda abierta a cualquier usuario autenticado: el Tipificador lo necesita para
    /// llenar su combo al romanear.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Authorize()]
    public class ConformacionesController : MeatBaseController
    {
        public ConformacionesController(IMediator mediator)
            : base(mediator)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery] GetConformacionesRequest request)
            => await this.Handle(request);

        [HttpGet("{codigo}")]
        public async Task<IActionResult> GetByCodigoAsync([FromRoute] string codigo) =>
            await this.Handle(new GetConformacionRequest { Codigo = codigo });

        [HttpPost]
        [Authorize(Roles = "SUPERADMIN")]
        public async Task<IActionResult> CreateAsync([FromBody] CreateConformacionRequest request) =>
            await this.Handle(request);

        [HttpPut("{codigo}")]
        [Authorize(Roles = "SUPERADMIN")]
        public async Task<IActionResult> UpdateAsync([FromRoute] string codigo, [FromBody] UpdateConformacionRequestFromBody body) =>
            await this.Handle(new UpdateConformacionRequest
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
            await this.Handle(new DeleteConformacionRequest { Codigo = codigo });
    }
}
