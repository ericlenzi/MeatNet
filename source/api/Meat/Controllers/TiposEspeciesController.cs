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
    /// <summary>
    /// TipoEspecie es el nomenclador de categorias de hacienda del rubro, comun a todas las
    /// empresas: no lleva EmpresaId, asi que una modificacion afecta a todas. Por eso lo
    /// mantiene el SUPERADMIN desde la empresa administrativa.
    ///
    /// La lectura queda abierta a cualquier usuario autenticado: la categoria es una FK que
    /// aparece en pantallas operativas (ingreso de hacienda, planificacion, tipificador).
    /// Lo que cada empresa ajusta va por EmpresasTiposEspeciesController.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Authorize()]
    public class TiposEspeciesController : MeatBaseController
    {
        public TiposEspeciesController(IMediator mediator)
            : base(mediator)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetTiposEspeciesAsync([FromQuery] GetTiposEspeciesRequest request)
            => await this.Handle(request);

        [HttpGet("{codigo}")]
        public async Task<IActionResult> GetTipoEspecieAsync([FromRoute] string codigo) =>
            await this.Handle(new GetTipoEspecieRequest { Codigo = codigo });

        [HttpPost]
        [Authorize(Roles = "SUPERADMIN")]
        public async Task<IActionResult> CreateTipoEspecieAsync([FromBody] CreateTipoEspecieRequest request) =>
            await this.Handle(request);

        [HttpPut("{codigo}")]
        [Authorize(Roles = "SUPERADMIN")]
        public async Task<IActionResult> UpdateTipoEspecieAsync([FromRoute] string codigo, [FromBody] UpdateTipoEspecieRequestFromBody body) =>
            await this.Handle(new UpdateTipoEspecieRequest
            {
                Codigo = codigo,
                Nombre = body.Nombre,
                EspecieId = body.EspecieId,
                TipoSexoId = body.TipoSexoId,
                PesoTeoricoReferencia = body.PesoTeoricoReferencia,
                Activo = body.Activo,
            });

        [HttpDelete("{codigo}")]
        [Authorize(Roles = "SUPERADMIN")]
        public async Task<IActionResult> DeleteTipoEspecieAsync([FromRoute] string codigo) =>
            await this.Handle(new DeleteTipoEspecieRequest { Codigo = codigo });
    }
}
