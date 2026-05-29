using Meat.Application.Establecimientos.GetEstablecimientos;
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
        [Authorize(Roles = "Admin, Abastecimiento")]
        public async Task<IActionResult> GetEstablecimientosAsync([FromQuery] GetEstablecimientosRequest request)
        {
            // request.NumeroSucursal = base.CurrentUser.NumeroSucursal;
            return await this.Handle(request);
        }

        //[HttpGet("{id}")]
        //[Authorize(Roles = "Admin, Abastecimiento")]
        //public async Task<IActionResult> GetEstablecimientoAsync([FromRoute] Guid id) => await this.Handle(
        //    new GetEstablecimientoRequest
        //    {
        //        Id = id,
        //    }
        //);

        //[HttpGet("Codigo/{codigo}")]
        //[Authorize(Roles = "Admin, Abastecimiento")]
        //public async Task<IActionResult> GetEstablecimientoByCodigoAsync([FromRoute] string codigo)
        //{
        //    return await this.Handle(
        //        new GetEstablecimientoByCodigoRequest()
        //        {
        //            Codigo = codigo
        //        }
        //    );
        //}
    }
}