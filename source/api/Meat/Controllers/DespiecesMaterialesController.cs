using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Meat.Application.DespiecesMateriales.CreateDespieceMaterial;
using Meat.Application.DespiecesMateriales.DeleteDespieceMaterial;
using Meat.Application.DespiecesMateriales.GetDespieceMaterial;
using Meat.Application.DespiecesMateriales.GetDespiecesMateriales;
using Meat.Application.DespiecesMateriales.UpdateDespieceMaterial;
using System;
using System.Threading.Tasks;

namespace Meat.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "ABAST,ABASTADMIN,ADMIN")]
    public class DespiecesMaterialesController : MeatBaseController
    {
        public DespiecesMaterialesController(IMediator mediator) : base(mediator) { }

        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery] GetDespiecesMaterialesRequest request) =>
            await this.Handle(request);

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id) =>
            await this.Handle(new GetDespieceMaterialRequest { Id = id });

        [HttpPost]
        [Authorize(Roles = "ABASTADMIN,ADMIN")]
        public async Task<IActionResult> CreateAsync([FromBody] CreateDespieceMaterialRequest request) =>
            await this.Handle(request);

        [HttpPut("{id}")]
        [Authorize(Roles = "ABASTADMIN,ADMIN")]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] UpdateDespieceMaterialRequestFromBody body) =>
            await this.Handle(new UpdateDespieceMaterialRequest
            {
                Id = id,
                MaterialOrigenId = body.MaterialOrigenId,
                MaterialDestinoId = body.MaterialDestinoId,
                Cantidad = body.Cantidad,
                Rendimiento = body.Rendimiento,
                Activo = body.Activo
            });

        [HttpDelete("{id}")]
        [Authorize(Roles = "ABASTADMIN,ADMIN")]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid id) =>
            await this.Handle(new DeleteDespieceMaterialRequest { Id = id });
    }
}
