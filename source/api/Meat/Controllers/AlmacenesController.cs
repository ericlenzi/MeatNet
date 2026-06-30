using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Meat.Application.Almacenes.CreateAlmacen;
using Meat.Application.Almacenes.DeleteAlmacen;
using Meat.Application.Almacenes.GetAlmacen;
using Meat.Application.Almacenes.GetAlmacenes;
using Meat.Application.Almacenes.UpdateAlmacen;
using System;
using System.Threading.Tasks;

namespace Meat.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "ABAST,ABASTADMIN,ADMIN")]
    public class AlmacenesController : MeatBaseController
    {
        public AlmacenesController(IMediator mediator) : base(mediator) { }

        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery] GetAlmacenesRequest request)
        {
            request.CodigoEmpresa = base.CurrentUser.CodigoEmpresa;
            return await this.Handle(request);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id) =>
            await this.Handle(new GetAlmacenRequest
            {
                Id = id,
                CodigoEmpresa = base.CurrentUser.CodigoEmpresa
            });

        [HttpPost]
        [Authorize(Roles = "ABASTADMIN,ADMIN")]
        public async Task<IActionResult> CreateAsync([FromBody] CreateAlmacenRequest request)
        {
            request.CodigoEmpresa = base.CurrentUser.CodigoEmpresa;
            return await this.Handle(request);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "ABASTADMIN,ADMIN")]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] UpdateAlmacenRequest request)
        {
            request.Id = id;
            request.CodigoEmpresa = base.CurrentUser.CodigoEmpresa;
            return await this.Handle(request);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "ABASTADMIN,ADMIN")]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid id) =>
            await this.Handle(new DeleteAlmacenRequest
            {
                Id = id,
                CodigoEmpresa = base.CurrentUser.CodigoEmpresa
            });
    }
}
