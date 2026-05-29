using Meat.Application.Empresas.CreateEmpresa;
using Meat.Application.Empresas.DeleteEmpresa;
using Meat.Application.Empresas.GetEmpresa;
using Meat.Application.Empresas.GetEmpresas;
using Meat.Application.Empresas.UpdateEmpresa;
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
    public class EmpresasController : MeatBaseController
    {
        public EmpresasController(IMediator mediator)
            : base(mediator)
        {
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetEmpresasAsync([FromQuery] GetEmpresasRequest request)
        {
            return await this.Handle(request);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetEmpresaAsync([FromRoute] Guid id) => await this.Handle(
            new GetEmpresaRequest
            {
                Id = id
            }
        );

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> CreateEmpresaAsync([FromBody] CreateEmpresaRequest request) => await Handle(request);

        [HttpPut("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> UpdateEmpresaAsync([FromRoute] Guid id, [FromBody] UpdateEmpresaRequestFromBody body) => await this.Handle(
            new UpdateEmpresaRequest()
            {
                Id = id,
                CodigoEmpresa = body.CodigoEmpresa,
                Nombre = body.Nombre,
                TipoEmpresaId = body.TipoEmpresaId,
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
        public async Task<IActionResult> DeleteEmpresaByIdAsync([FromRoute] Guid id) => await this.Handle(
            new DeleteEmpresaRequest
            {
                Id = id
            }
        );
    }
}
