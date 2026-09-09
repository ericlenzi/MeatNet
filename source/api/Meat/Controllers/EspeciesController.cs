using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Meat.Application.Especies.GetEspecies;
using Meat.Application.Especies.GetEspecie;
using Meat.Application.Especies.CreateEspecie;
using Meat.Application.Especies.UpdateEspecie;
using Meat.Application.Especies.DeleteEspecie;
using System.Threading.Tasks;

namespace Meat.Controllers
{
    /// <summary>
    /// Especie es un catalogo comun a todas las empresas: no lleva EmpresaId, asi que una
    /// modificacion afecta a todas. Por eso lo mantiene el SUPERADMIN desde la empresa
    /// administrativa, y no el ADMIN de una empresa en particular.
    ///
    /// La lectura queda abierta a cualquier usuario autenticado: la especie es una FK que
    /// aparece en pantallas operativas (planificacion de faena, ingreso de hacienda), y
    /// restringirla dejaba esos combos vacios para los perfiles de abastecimiento.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Authorize()]
    public class EspeciesController : MeatBaseController
    {
        public EspeciesController(IMediator mediator)
            : base(mediator)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetEspeciesAsync([FromQuery] GetEspeciesRequest request)
        {
            return await this.Handle(request);
        }

        [HttpGet("{codigo}")]
        public async Task<IActionResult> GetEspecieByCodigoAsync([FromRoute] string codigo) => await this.Handle(
            new GetEspecieRequest { Codigo = codigo }
        );

        [HttpPost]
        [Authorize(Roles = "SUPERADMIN")]
        public async Task<IActionResult> CreateEspecieAsync([FromBody] CreateEspecieRequest request)
        {
            return await Handle(request);
        }

        [HttpPut("{codigo}")]
        [Authorize(Roles = "SUPERADMIN")]
        public async Task<IActionResult> UpdateEspecieAsync([FromRoute] string codigo, [FromBody] UpdateEspecieRequestFromBody body) => await this.Handle(
            new UpdateEspecieRequest()
            {
                Codigo = codigo,
                Nombre = body.Nombre,
                Activo = body.Activo
            }
        );

        [HttpDelete("{codigo}")]
        [Authorize(Roles = "SUPERADMIN")]
        public async Task<IActionResult> DeleteEspecieByCodigoAsync([FromRoute] string codigo) => await this.Handle(
            new DeleteEspecieRequest { Codigo = codigo }
        );
    }
}
