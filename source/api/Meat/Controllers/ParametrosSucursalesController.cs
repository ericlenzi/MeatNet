using Meat.Application.ParametrosSucursales.GetParametrosSucursales;
using Meat.Application.ParametrosSucursales.GetParametroSucursal;
using Meat.Application.ParametrosSucursales.GetParametroSucursalByCodigo;
using Meat.Application.ParametrosSucursales.UpdateParametroSucursal;
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
    public class ParametrosSucursalesController : MeatBaseController
    {
        public ParametrosSucursalesController(IMediator mediator)
            : base(mediator)
        {
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Abastecimiento")]
        public async Task<IActionResult> GetParametrosSucursalesAsync([FromQuery] GetParametrosSucursalesRequest request)
        {
            return await this.Handle(request);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Abastecimiento")]
        public async Task<IActionResult> GetParametroSucursalByIdAsync([FromRoute] Guid id) => await this.Handle(
            new GetParametroSucursalRequest
            {
                Id = id,
            }
        );

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Abastecimiento")]
        public async Task<IActionResult> UpdateParametroSucursalAsync([FromRoute] Guid id, [FromBody] UpdateParametroSucursalRequestFromBody body) => await this.Handle(
            new UpdateParametroSucursalRequest()
            {
                Id = id,
                SucursalId = body.SucursalId,
                ParametroId = id,
                Valor = body.Valor
            }
        );

        [HttpGet("ByCodigo/{codigo}")]
        [Authorize(Roles = "Admin, Abastecimiento, Cajero, Encargado")]
        public async Task<IActionResult> GetParametroSucursalByCodigoAsync([FromRoute] string codigo) => await Handle(
            new GetParametroSucursalByCodigoRequest
            {
                Codigo = codigo,
            }
        );
    }
}
