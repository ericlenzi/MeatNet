using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Meat.Application.Materiales.CreateMaterial;
using Meat.Application.Materiales.DeleteMaterial;
using Meat.Application.Materiales.GetMaterial;
using Meat.Application.Materiales.GetMateriales;
using Meat.Application.Materiales.UpdateMaterial;
using System;
using System.Threading.Tasks;

namespace Meat.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "ABAST,ABASTADMIN,ADMIN")]
    public class MaterialesController : MeatBaseController
    {
        public MaterialesController(IMediator mediator) : base(mediator) { }

        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery] GetMaterialesRequest request) =>
            await this.Handle(request);

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id) =>
            await this.Handle(new GetMaterialRequest { Id = id });

        [HttpPost]
        [Authorize(Roles = "ABASTADMIN,ADMIN")]
        public async Task<IActionResult> CreateAsync([FromBody] CreateMaterialRequest request) =>
            await this.Handle(request);

        [HttpPut("{id}")]
        [Authorize(Roles = "ABASTADMIN,ADMIN")]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] UpdateMaterialRequestFromBody body) =>
            await this.Handle(new UpdateMaterialRequest
            {
                Id = id,
                Nombre = body.Nombre,
                TipoMaterialId = body.TipoMaterialId,
                UnidadMedidaId = body.UnidadMedidaId,
                PesoTeorico = body.PesoTeorico,
                ERP_Codigo = body.ERP_Codigo,
                Activo = body.Activo
            });

        [HttpDelete("{id}")]
        [Authorize(Roles = "ABASTADMIN,ADMIN")]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid id) =>
            await this.Handle(new DeleteMaterialRequest { Id = id });
    }
}
