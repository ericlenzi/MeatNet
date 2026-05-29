using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Meat.Application.Puestos.GetPuestos;
using Meat.Application.Puestos.GetPuesto;
using Meat.Application.Puestos.CreatePuesto;
using Meat.Application.Puestos.UpdatePuesto;
using Meat.Application.Puestos.DeletePuesto;
using System.Threading.Tasks;

namespace Meat.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize()]
    public class PuestosController : MeatBaseController
    {
        public PuestosController(IMediator mediator)
            : base(mediator)
        {
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Abastecimiento, Encargado, Cajero")]
        public async Task<IActionResult> GetPuestosAsync([FromQuery] GetPuestosRequest request)
        {
            return await this.Handle(request);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Abastecimiento")]
        public async Task<IActionResult> GetPuestoByIdAsync([FromRoute] Guid id) => await this.Handle(
            new GetPuestoRequest
            {
                Id = id,
            }
        );

        [HttpPost]
        [Authorize(Roles = "Admin, Abastecimiento")]
        public async Task<IActionResult> CreatePuestoAsync([FromBody] CreatePuestoRequest request) => await Handle(request);

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Abastecimiento")]
        public async Task<IActionResult> UpdatPuestoAsync([FromRoute] Guid id, [FromBody] UpdatePuestoRequestFromBody body) => await this.Handle(
            new UpdatePuestoRequest()
            {
                Id = id,
                SucursalId = body.SucursalId,
                NumeroPuesto = body.NumeroPuesto,
                Erp_Codigo = body.Erp_Codigo
            }
        );

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, Abastecimiento")]
        public async Task<IActionResult> DeletePuestoByIdAsync([FromRoute] Guid id) => await this.Handle(
            new DeletePuestoRequest
            {
                Id = id,
            }
        );
    }
}