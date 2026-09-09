using MediatR;
using Meat.Application.EmpresasTiposEspecies.CreateEmpresaTipoEspecie;
using Meat.Application.EmpresasTiposEspecies.DeleteEmpresaTipoEspecie;
using Meat.Application.EmpresasTiposEspecies.GetEmpresaTipoEspecie;
using Meat.Application.EmpresasTiposEspecies.GetEmpresasTiposEspecies;
using Meat.Application.EmpresasTiposEspecies.UpdateEmpresaTipoEspecie;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Meat.Controllers
{
    /// <summary>
    /// Configuracion de la empresa sobre el catalogo global de categorias: con cuales opera,
    /// con que peso teorico y con que codigo de ERP. Es la fuente de los combos operativos.
    ///
    /// La lectura queda abierta a cualquier usuario autenticado (los combos aparecen en ingreso
    /// de hacienda y en el tipificador); la escritura es del ADMIN de la empresa.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [Authorize()]
    public class EmpresasTiposEspeciesController : MeatBaseController
    {
        public EmpresasTiposEspeciesController(IMediator mediator)
            : base(mediator)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery] GetEmpresasTiposEspeciesRequest request)
            => await this.Handle(request);

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id) =>
            await this.Handle(new GetEmpresaTipoEspecieRequest { Id = id });

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> CreateAsync([FromBody] CreateEmpresaTipoEspecieRequest request) =>
            await this.Handle(request);

        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] UpdateEmpresaTipoEspecieRequestFromBody body) =>
            await this.Handle(new UpdateEmpresaTipoEspecieRequest
            {
                Id = id,
                PesoTeorico = body.PesoTeorico,
                ERP_Codigo = body.ERP_Codigo,
                Activo = body.Activo,
            });

        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid id) =>
            await this.Handle(new DeleteEmpresaTipoEspecieRequest { Id = id });
    }
}
