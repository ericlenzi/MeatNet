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

        [HttpGet("{codigo}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] string codigo) =>
            await this.Handle(new GetUnidadFaenaRequest { Codigo = codigo });

        [HttpPost]
        [Authorize(Roles = "ABASTADMIN,ADMIN")]
        public async Task<IActionResult> CreateAsync([FromBody] CreateUnidadFaenaRequest request) =>
            await this.Handle(request);

        [HttpPut("{codigo}")]
        [Authorize(Roles = "ABASTADMIN,ADMIN")]
        public async Task<IActionResult> UpdateAsync([FromRoute] string codigo, [FromBody] UpdateUnidadFaenaRequestFromBody body) =>
            await this.Handle(new UpdateUnidadFaenaRequest
            {
                Codigo = codigo,
                EspecieId = body.EspecieId,
                Nombre = body.Nombre,
                CantidadCuartos = body.CantidadCuartos,
                PiezasPorAnimal = body.PiezasPorAnimal,
                PorDefecto = body.PorDefecto,
                CodigoMaterial = body.CodigoMaterial,
                ERP_Codigo = body.ERP_Codigo,
                Activo = body.Activo
            });

        [HttpDelete("{codigo}")]
        [Authorize(Roles = "ABASTADMIN,ADMIN")]
        public async Task<IActionResult> DeleteAsync([FromRoute] string codigo) =>
            await this.Handle(new DeleteUnidadFaenaRequest { Codigo = codigo });
    }
}
