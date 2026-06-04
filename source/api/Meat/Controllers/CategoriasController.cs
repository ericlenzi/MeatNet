using MediatR;
using Meat.Application.Categorias.CreateCategoria;
using Meat.Application.Categorias.DeleteCategoria;
using Meat.Application.Categorias.GetCategoria;
using Meat.Application.Categorias.GetCategorias;
using Meat.Application.Categorias.UpdateCategoria;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Meat.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "ADMIN")]
    public class CategoriasController : MeatBaseController
    {
        public CategoriasController(IMediator mediator)
            : base(mediator)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetCategoriasAsync([FromQuery] GetCategoriasRequest request)
            => await this.Handle(request);

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoriaAsync([FromRoute] string id) =>
            await this.Handle(new GetCategoriaRequest { Id = id });

        [HttpPost]
        public async Task<IActionResult> CreateCategoriaAsync([FromBody] CreateCategoriaRequest request) =>
            await this.Handle(request);

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategoriaAsync([FromRoute] string id, [FromBody] UpdateCategoriaRequestFromBody body) =>
            await this.Handle(new UpdateCategoriaRequest
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
        public async Task<IActionResult> DeleteCategoriaAsync([FromRoute] string id) =>
            await this.Handle(new DeleteCategoriaRequest { Id = id });
    }
}
