using MediatR;
using Meat.Application.TiposEspecies.CreateTipoEspecie;
using Meat.Application.TiposEspecies.DeleteTipoEspecie;
using Meat.Application.TiposEspecies.GetTipoEspecie;
using Meat.Application.TiposEspecies.GetTiposEspecies;
using Meat.Application.TiposEspecies.UpdateTipoEspecie;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Meat.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "ADMIN")]
    public class TiposEspeciesController : MeatBaseController
    {
        public TiposEspeciesController(IMediator mediator)
            : base(mediator)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetTiposEspeciesAsync([FromQuery] GetTiposEspeciesRequest request)
            => await this.Handle(request);

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTipoEspecieAsync([FromRoute] string id) =>
            await this.Handle(new GetTipoEspecieRequest { Id = id });

        [HttpPost]
        public async Task<IActionResult> CreateTipoEspecieAsync([FromBody] CreateTipoEspecieRequest request) =>
            await this.Handle(request);

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTipoEspecieAsync([FromRoute] string id, [FromBody] UpdateTipoEspecieRequestFromBody body) =>
            await this.Handle(new UpdateTipoEspecieRequest
            {
                Id = id,
                Nombre = body.Nombre,
                EspecieId = body.EspecieId,
                TipoSexoId = body.TipoSexoId,
                CodigoMaterialDesde = body.CodigoMaterialDesde,
                CodigoMaterialHasta = body.CodigoMaterialHasta,
                Activo = body.Activo,
            });

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTipoEspecieAsync([FromRoute] string id) =>
            await this.Handle(new DeleteTipoEspecieRequest { Id = id });
    }
}
