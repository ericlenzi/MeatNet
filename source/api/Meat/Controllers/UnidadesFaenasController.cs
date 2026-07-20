using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Meat.Application.UnidadesFaenas.CreateUnidadFaena;
using Meat.Application.UnidadesFaenas.DeleteUnidadFaena;
using Meat.Application.UnidadesFaenas.GetUnidadFaena;
using Meat.Application.UnidadesFaenas.GetUnidadesFaenas;
using Meat.Application.UnidadesFaenas.UpdateUnidadFaena;
using System;
using System.Threading.Tasks;

namespace Meat.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "ABAST,ABASTADMIN,ADMIN")]
    public class UnidadesFaenasController : MeatBaseController
    {
        public UnidadesFaenasController(IMediator mediator) : base(mediator) { }

        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery] GetUnidadesFaenasRequest request) =>
            await this.Handle(request);

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id) =>
            await this.Handle(new GetUnidadFaenaRequest { Id = id });

        [HttpPost]
        [Authorize(Roles = "ABASTADMIN,ADMIN")]
        public async Task<IActionResult> CreateAsync([FromBody] CreateUnidadFaenaRequest request) =>
            await this.Handle(request);

        [HttpPut("{id}")]
        [Authorize(Roles = "ABASTADMIN,ADMIN")]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] UpdateUnidadFaenaRequestFromBody body) =>
            await this.Handle(new UpdateUnidadFaenaRequest
            {
                Id = id,
                EspecieId = body.EspecieId,
                Numero = body.Numero,
                Nombre = body.Nombre,
                CantidadCuartos = body.CantidadCuartos,
                PiezasPorAnimal = body.PiezasPorAnimal,
                PorDefecto = body.PorDefecto,
                CodigoMaterial = body.CodigoMaterial,
                ERP_Codigo = body.ERP_Codigo,
                Activo = body.Activo
            });

        [HttpDelete("{id}")]
        [Authorize(Roles = "ABASTADMIN,ADMIN")]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid id) =>
            await this.Handle(new DeleteUnidadFaenaRequest { Id = id });
    }
}
