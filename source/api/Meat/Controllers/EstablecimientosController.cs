using Meat.Application.Establecimientos.GetEstablecimientos;
using Meat.Application.Establecimientos.GetEstablecimiento;
using Meat.Application.Establecimientos.CreateEstablecimiento;
using Meat.Application.Establecimientos.UpdateEstablecimiento;
using Meat.Application.Establecimientos.DeleteEstablecimiento;
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
    public class EstablecimientosController : MeatBaseController
    {
        public EstablecimientosController(IMediator mediator)
            : base(mediator)
        {
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, ABAST")]
        public async Task<IActionResult> GetEstablecimientosAsync([FromQuery] GetEstablecimientosRequest request)
        {
            request.CodigoEmpresa = base.CurrentUser.CodigoEmpresa;
            return await this.Handle(request);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "ADMIN, ABAST")]
        public async Task<IActionResult> GetEstablecimientoAsync([FromRoute] Guid id) => await this.Handle(
            new GetEstablecimientoRequest { Id = id, CodigoEmpresa = base.CurrentUser.CodigoEmpresa }
        );

        [HttpPost]
        [Authorize(Roles = "ADMIN, ABAST")]
        public async Task<IActionResult> CreateEstablecimientoAsync([FromBody] CreateEstablecimientoRequest request)
        {
            request.CodigoEmpresa = base.CurrentUser.CodigoEmpresa;
            return await Handle(request);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN, ABAST")]
        public async Task<IActionResult> UpdateEstablecimientoAsync([FromRoute] Guid id, [FromBody] UpdateEstablecimientoRequestFromBody body) => await this.Handle(
            new UpdateEstablecimientoRequest()
            {
                Id = id,
                CodigoEmpresa = base.CurrentUser.CodigoEmpresa,
                Nombre = body.Nombre,
                SucursalId = body.SucursalId,
                EspecieId = body.EspecieId,
                NumeroSenasa = body.NumeroSenasa,
                NumeroOncca = body.NumeroOncca,
                Activo = body.Activo
            }
        );

        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN, ABAST")]
        public async Task<IActionResult> DeleteEstablecimientoByIdAsync([FromRoute] Guid id) => await this.Handle(
            new DeleteEstablecimientoRequest { Id = id, CodigoEmpresa = base.CurrentUser.CodigoEmpresa }
        );
    }
}
