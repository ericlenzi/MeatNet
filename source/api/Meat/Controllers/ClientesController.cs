using Meat.Application.Clientes.GetClientes;
using Meat.Application.Clientes.GetCliente;
using Meat.Application.Clientes.CreateCliente;
using Meat.Application.Clientes.UpdateCliente;
using Meat.Application.Clientes.DeleteCliente;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Meat.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize()]
    public class ClientesController : MeatBaseController
    {
        public ClientesController(IMediator mediator)
            : base(mediator)
        {
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetClientesAsync([FromQuery] GetClientesRequest request)
            => await this.Handle(request);

        [HttpGet("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetClienteAsync([FromRoute] Guid id) => await this.Handle(
            new GetClienteRequest { Id = id }
        );

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> CreateClienteAsync([FromBody] CreateClienteRequest request)
            => await Handle(request);

        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> UpdateClienteAsync([FromRoute] Guid id, [FromBody] UpdateClienteRequestFromBody body) => await this.Handle(
            new UpdateClienteRequest()
            {
                Id = id,
                Nombre = body.Nombre,
                TipoClienteId = body.TipoClienteId,
                NumeroCuit = body.NumeroCuit,
                NumeroIngresosBrutos = body.NumeroIngresosBrutos,
                NumeroInscripcionRuca = body.NumeroInscripcionRuca,
                CodigoActividad = body.CodigoActividad,
                ERP_Codigo = body.ERP_Codigo,
                Activo = body.Activo
            }
        );

        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeleteClienteByIdAsync([FromRoute] Guid id) => await this.Handle(
            new DeleteClienteRequest { Id = id }
        );
    }
}
