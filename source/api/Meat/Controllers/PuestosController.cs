using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Meat.Application.Puestos.CreatePuesto;
using Meat.Application.Puestos.DeletePuesto;
using Meat.Application.Puestos.GetPuesto;
using Meat.Application.Puestos.GetPuestos;
using Meat.Application.Puestos.UpdatePuesto;
using System;
using System.Threading.Tasks;

namespace Meat.Controllers
{
    /// <summary>
    /// Puestos de la planta (el palco de faena), configurados por Establecimiento + Especie.
    /// La lectura la necesitan pantallas operativas (el alta de la lista de matanza elige el
    /// palco), asi que queda en el rol operativo; el alta y la edicion son de administracion.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "ABAST,ABASTADMIN,ADMIN")]
    public class PuestosController : MeatBaseController
    {
        public PuestosController(IMediator mediator)
            : base(mediator)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetPuestosAsync([FromQuery] GetPuestosRequest request) =>
            await this.Handle(request);

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPuestoByIdAsync([FromRoute] Guid id) =>
            await this.Handle(new GetPuestoRequest { Id = id });

        [HttpPost]
        [Authorize(Roles = "ABASTADMIN,ADMIN")]
        public async Task<IActionResult> CreatePuestoAsync([FromBody] CreatePuestoRequest request) =>
            await this.Handle(request);

        [HttpPut("{id}")]
        [Authorize(Roles = "ABASTADMIN,ADMIN")]
        public async Task<IActionResult> UpdatePuestoAsync([FromRoute] Guid id, [FromBody] UpdatePuestoRequestFromBody body) =>
            await this.Handle(new UpdatePuestoRequest
            {
                Id = id,
                Nombre = body.Nombre,
                EstablecimientoId = body.EstablecimientoId,
                EspecieId = body.EspecieId,
                TipoPuestoId = body.TipoPuestoId,
                TipoMedicionId = body.TipoMedicionId,
                Activo = body.Activo
            });

        [HttpDelete("{id}")]
        [Authorize(Roles = "ABASTADMIN,ADMIN")]
        public async Task<IActionResult> DeletePuestoByIdAsync([FromRoute] Guid id) =>
            await this.Handle(new DeletePuestoRequest { Id = id });
    }
}
