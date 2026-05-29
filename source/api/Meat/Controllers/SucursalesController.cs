using Meat.Application.Sucursales.CreateSucursal;
using Meat.Application.Sucursales.DeleteSucursal;
using Meat.Application.Sucursales.GetSucursal;
using Meat.Application.Sucursales.GetSucursales;
using Meat.Application.Sucursales.GetSucursalByCodigo;
using Meat.Application.Sucursales.UpdateSucursal;
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
    public class SucursalesController : MeatBaseController
    {
        public SucursalesController(IMediator mediator)
            : base(mediator)
        {
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ABAST")]
        public async Task<IActionResult> GetSucursalesAsync([FromQuery] GetSucursalesRequest request)
        {
            // request.NumeroSucursal = base.CurrentUser.NumeroSucursal;
            return await this.Handle(request);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "ADMIN, ABAST")]
        public async Task<IActionResult> GetSucursalAsync([FromRoute] Guid id) => await this.Handle(
            new GetSucursalRequest
            {
                Id = id,
            }
        );

        [HttpPost]
        [Authorize(Roles = "ADMIN, ABAST")]
        public async Task<IActionResult> CreateSucursalAsync([FromBody] CreateSucursalRequest request) => await Handle(request);

        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN, ABAST")]
        public async Task<IActionResult> UpdatSucursalAsync([FromRoute] Guid id, [FromBody] UpdateSucursalRequestFromBody body) => await this.Handle(
            new UpdateSucursalRequest()
            {
                Id = id,
                Nombre = body.Nombre,
                EmpresaId = body.EmpresaId,
                Activa = body.Activa
            }
        );

        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN, ABAST")]
        public async Task<IActionResult> DeleteSucursalByIdAsync([FromRoute] Guid id) => await this.Handle(
            new DeleteSucursalRequest
            {
                Id = id,
            }
        );

        [HttpGet("Codigo/{codigo}")]
        [Authorize(Roles = "ADMIN, ABAST")]
        public async Task<IActionResult> GetSucursalByCodigoAsync([FromRoute] string codigo)
        {
            return await this.Handle(
                new GetSucursalByCodigoRequest()
                {
                    Codigo = codigo
                }
            );
        }
    }
}