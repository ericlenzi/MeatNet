using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Meat.Application.Clientes.GetClienteEstablecimientos;
using Meat.Application.Clientes.AddClienteEstablecimiento;
using Meat.Application.Clientes.RemoveClienteEstablecimiento;
using System;
using System.Threading.Tasks;

namespace Meat.Controllers
{
    [ApiController]
    [Route("Clientes/{clienteId}/Establecimientos")]
    [Authorize(Roles = "ADMIN")]
    public class ClientesEstablecimientosController : MeatBaseController
    {
        public ClientesEstablecimientosController(IMediator mediator)
            : base(mediator)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync([FromRoute] Guid clienteId) => await this.Handle(
            new GetClienteEstablecimientosRequest { ClienteId = clienteId }
        );

        [HttpPost]
        public async Task<IActionResult> AddAsync([FromRoute] Guid clienteId, [FromBody] AddClienteEstablecimientoBody body) => await this.Handle(
            new AddClienteEstablecimientoRequest
            {
                ClienteId = clienteId,
                EstablecimientoId = body.EstablecimientoId,
                CodigoRenspa = body.CodigoRenspa,
                NumeroCUIG = body.NumeroCUIG
            }
        );

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveAsync([FromRoute] Guid clienteId, [FromRoute] Guid id) => await this.Handle(
            new RemoveClienteEstablecimientoRequest { ClienteEstablecimientoId = id }
        );
    }
}
