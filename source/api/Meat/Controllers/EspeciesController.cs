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
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetEspeciesAsync([FromQuery] GetEspeciesRequest request)
        {
            return await this.Handle(request);
        }

        [HttpGet("{codigo}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetEspecieByCodigoAsync([FromRoute] string codigo) => await this.Handle(
            new GetEspecieRequest { Codigo = codigo }
        );

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> CreateEspecieAsync([FromBody] CreateEspecieRequest request)
        {
            return await Handle(request);
        }

        [HttpPut("{codigo}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> UpdateEspecieAsync([FromRoute] string codigo, [FromBody] UpdateEspecieRequestFromBody body) => await this.Handle(
            new UpdateEspecieRequest()
            {
                Codigo = codigo,
                Nombre = body.Nombre,
                Activo = body.Activo
            }
        );

        [HttpDelete("{codigo}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeleteEspecieByCodigoAsync([FromRoute] string codigo) => await this.Handle(
            new DeleteEspecieRequest { Codigo = codigo }
        );
    }
}
