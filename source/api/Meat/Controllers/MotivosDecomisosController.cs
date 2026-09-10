using MediatR;
using Meat.Application.MotivosDecomisos.CreateMotivoDecomiso;
using Meat.Application.MotivosDecomisos.DeleteMotivoDecomiso;
using Meat.Application.MotivosDecomisos.GetMotivoDecomiso;
using Meat.Application.MotivosDecomisos.GetMotivosDecomisos;
using Meat.Application.MotivosDecomisos.UpdateMotivoDecomiso;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Meat.Controllers
{
    /// <summary>
    /// Catalogo de motivos de decomiso por especie: la causa sanitaria por la que la inspeccion
    /// condena una res o retira kilos de una media res. Es el nomenclador del rubro, comun a
    /// todas las empresas, asi que lo mantiene el SUPERADMIN desde la empresa administrativa.
    ///
    /// La lectura queda abierta a cualquier usuario autenticado: el Tipificador lo necesita para
    /// llenar su combo al registrar un decomiso.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Authorize()]
    public class MotivosDecomisosController : MeatBaseController
    {
        public MotivosDecomisosController(IMediator mediator)
            : base(mediator)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery] GetMotivosDecomisosRequest request)
            => await this.Handle(request);

        [HttpGet("{codigo}")]
        public async Task<IActionResult> GetByCodigoAsync([FromRoute] string codigo) =>
            await this.Handle(new GetMotivoDecomisoRequest { Codigo = codigo });

        [HttpPost]
        [Authorize(Roles = "SUPERADMIN")]
        public async Task<IActionResult> CreateAsync([FromBody] CreateMotivoDecomisoRequest request) =>
            await this.Handle(request);

        [HttpPut("{codigo}")]
        [Authorize(Roles = "SUPERADMIN")]
        public async Task<IActionResult> UpdateAsync([FromRoute] string codigo, [FromBody] UpdateMotivoDecomisoRequestFromBody body) =>
            await this.Handle(new UpdateMotivoDecomisoRequest
            {
                Codigo = codigo,
                Nombre = body.Nombre,
                EspecieId = body.EspecieId,
                Orden = body.Orden,
                ExigeContusion = body.ExigeContusion,
                Activo = body.Activo,
            });

        [HttpDelete("{codigo}")]
        [Authorize(Roles = "SUPERADMIN")]
        public async Task<IActionResult> DeleteAsync([FromRoute] string codigo) =>
            await this.Handle(new DeleteMotivoDecomisoRequest { Codigo = codigo });
    }
}
