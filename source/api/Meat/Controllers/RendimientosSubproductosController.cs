using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Meat.Application.RendimientosSubproductos.CreateRendimientoSubproducto;
using Meat.Application.RendimientosSubproductos.DeleteRendimientoSubproducto;
using Meat.Application.RendimientosSubproductos.GetRendimientoSubproducto;
using Meat.Application.RendimientosSubproductos.GetRendimientosSubproductos;
using Meat.Application.RendimientosSubproductos.UpdateRendimientoSubproducto;
using System;
using System.Threading.Tasks;

namespace Meat.Controllers
{
    /// <summary>
    /// Rendimiento de subproductos por especie: el porcentaje del peso de la res con el que el
    /// Analisis de Faena estima cuero, sebo y menudencias (R-A9). La lectura la usa el analisis,
    /// asi que queda en el rol operativo; el alta y la edicion son de administracion.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "ABAST,ABASTADMIN,ADMIN")]
    public class RendimientosSubproductosController : MeatBaseController
    {
        public RendimientosSubproductosController(IMediator mediator) : base(mediator) { }

        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery] GetRendimientosSubproductosRequest request) =>
            await this.Handle(request);

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id) =>
            await this.Handle(new GetRendimientoSubproductoRequest { Id = id });

        [HttpPost]
        [Authorize(Roles = "ABASTADMIN,ADMIN")]
        public async Task<IActionResult> CreateAsync([FromBody] CreateRendimientoSubproductoRequest request) =>
            await this.Handle(request);

        [HttpPut("{id}")]
        [Authorize(Roles = "ABASTADMIN,ADMIN")]
        public async Task<IActionResult> UpdateAsync(
            [FromRoute] Guid id, [FromBody] UpdateRendimientoSubproductoRequestFromBody body) =>
            await this.Handle(new UpdateRendimientoSubproductoRequest
            {
                Id = id,
                EspecieId = body.EspecieId,
                MaterialId = body.MaterialId,
                Porcentaje = body.Porcentaje,
                Activo = body.Activo
            });

        [HttpDelete("{id}")]
        [Authorize(Roles = "ABASTADMIN,ADMIN")]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid id) =>
            await this.Handle(new DeleteRendimientoSubproductoRequest { Id = id });
    }
}
