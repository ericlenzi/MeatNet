using MediatR;
using Meat.Application.GradosEngrasamiento.CreateGradoEngrasamiento;
using Meat.Application.GradosEngrasamiento.DeleteGradoEngrasamiento;
using Meat.Application.GradosEngrasamiento.GetGradoEngrasamiento;
using Meat.Application.GradosEngrasamiento.GetGradosEngrasamiento;
using Meat.Application.GradosEngrasamiento.UpdateGradoEngrasamiento;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Meat.Controllers
{
    /// <summary>
    /// Catalogo de grados de engrasamiento por especie: la cobertura de grasa de la media res. Es uno de los ejes de la
    /// tipificacion oficial, comun a todas las empresas, asi que lo mantiene el SUPERADMIN desde
    /// la empresa administrativa.
    ///
    /// La lectura queda abierta a cualquier usuario autenticado: el Tipificador lo necesita para
    /// llenar su combo al romanear.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Authorize()]
    public class GradosEngrasamientoController : MeatBaseController
    {
        public GradosEngrasamientoController(IMediator mediator)
            : base(mediator)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery] GetGradosEngrasamientoRequest request)
            => await this.Handle(request);

        [HttpGet("{codigo}")]
        public async Task<IActionResult> GetByCodigoAsync([FromRoute] string codigo) =>
            await this.Handle(new GetGradoEngrasamientoRequest { Codigo = codigo });

        [HttpPost]
        [Authorize(Roles = "SUPERADMIN")]
        public async Task<IActionResult> CreateAsync([FromBody] CreateGradoEngrasamientoRequest request) =>
            await this.Handle(request);

        [HttpPut("{codigo}")]
        [Authorize(Roles = "SUPERADMIN")]
        public async Task<IActionResult> UpdateAsync([FromRoute] string codigo, [FromBody] UpdateGradoEngrasamientoRequestFromBody body) =>
            await this.Handle(new UpdateGradoEngrasamientoRequest
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
            await this.Handle(new DeleteGradoEngrasamientoRequest { Codigo = codigo });
    }
}
