using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Meat.Application.Tipificaciones.CreateTipificacion;
using Meat.Application.Tipificaciones.DeleteTipificacion;
using Meat.Application.Tipificaciones.GetTipificacion;
using Meat.Application.Tipificaciones.GetTipificaciones;
using Meat.Application.Tipificaciones.UpdateTipificacion;
using System.Threading.Tasks;

namespace Meat.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "ABAST,ABASTADMIN,ADMIN")]
    public class TipificacionesController : MeatBaseController
    {
        public TipificacionesController(IMediator mediator) : base(mediator) { }

        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery] GetTipificacionesRequest request)
        {
            request.CodigoEmpresa = base.CurrentUser.CodigoEmpresa;
            return await this.Handle(request);
        }

        [HttpGet("{codigo}")]
        public async Task<IActionResult> GetByCodigoAsync([FromRoute] string codigo) =>
            await this.Handle(new GetTipificacionRequest
            {
                Codigo = codigo,
                CodigoEmpresa = base.CurrentUser.CodigoEmpresa
            });

        [HttpPost]
        [Authorize(Roles = "ABASTADMIN,ADMIN")]
        public async Task<IActionResult> CreateAsync([FromBody] CreateTipificacionRequest request)
        {
            request.CodigoEmpresa = base.CurrentUser.CodigoEmpresa;
            return await this.Handle(request);
        }

        [HttpPut("{codigo}")]
        [Authorize(Roles = "ABASTADMIN,ADMIN")]
        public async Task<IActionResult> UpdateAsync([FromRoute] string codigo, [FromBody] UpdateTipificacionRequestFromBody body) =>
            await this.Handle(new UpdateTipificacionRequest
            {
                Codigo = codigo,
                CodigoEmpresa = base.CurrentUser.CodigoEmpresa,
                Descripcion = body.Descripcion,
                EspecieId = body.EspecieId,
                TipoEspecieId = body.TipoEspecieId,
                UnidadFaenaId = body.UnidadFaenaId,
                DestinoComercialId = body.DestinoComercialId,
                TipificacionOficialId = body.TipificacionOficialId,
                PesoDesde = body.PesoDesde,
                PesoHasta = body.PesoHasta,
                UnidadMedidaId = body.UnidadMedidaId,
                Activo = body.Activo
            });

        [HttpDelete("{codigo}")]
        [Authorize(Roles = "ABASTADMIN,ADMIN")]
        public async Task<IActionResult> DeleteAsync([FromRoute] string codigo) =>
            await this.Handle(new DeleteTipificacionRequest
            {
                Codigo = codigo,
                CodigoEmpresa = base.CurrentUser.CodigoEmpresa
            });
    }
}
