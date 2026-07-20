using MediatR;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Tipificaciones.GetTipificacion
{
    public class GetTipificacionHandler : IRequestHandler<GetTipificacionRequest, GetTipificacionResponse>
    {
        private readonly MeatContext context;

        public GetTipificacionHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetTipificacionResponse> Handle(GetTipificacionRequest request, CancellationToken cancellationToken)
        {
            return await (
                from t in this.context.Tipificaciones
                join e in this.context.Especies on t.EspecieId equals e.Codigo into ej
                from e in ej.DefaultIfEmpty()
                join te in this.context.TiposEspecies on t.TipoEspecieId equals te.Id into tej
                from te in tej.DefaultIfEmpty()
                join uf in this.context.UnidadesFaenas on t.UnidadFaenaId equals uf.Id into ufj
                from uf in ufj.DefaultIfEmpty()
                join dc in this.context.DestinosComerciales on t.DestinoComercialId equals dc.Codigo into dcj
                from dc in dcj.DefaultIfEmpty()
                join tofi in this.context.TipificacionesOficiales on t.TipificacionOficialId equals tofi.Codigo into tofij
                from tofi in tofij.DefaultIfEmpty()
                join um in this.context.UnidadesMedidas on t.UnidadMedidaId equals um.Codigo into umj
                from um in umj.DefaultIfEmpty()
                where t.Codigo == request.Codigo && t.CodigoEmpresa == request.CodigoEmpresa
                select new GetTipificacionResponse
                {
                    Codigo = t.Codigo,
                    Descripcion = t.Descripcion,
                    EspecieId = t.EspecieId,
                    EspecieNombre = e != null ? e.Nombre : null,
                    TipoEspecieId = t.TipoEspecieId,
                    TipoEspecieNombre = te != null ? te.Nombre : null,
                    UnidadFaenaId = t.UnidadFaenaId,
                    UnidadFaenaNombre = uf != null ? uf.Nombre : null,
                    DestinoComercialId = t.DestinoComercialId,
                    DestinoComercialNombre = dc != null ? dc.Nombre : null,
                    TipificacionOficialId = t.TipificacionOficialId,
                    TipificacionOficialNombre = tofi != null ? tofi.Nombre : null,
                    PesoDesde = t.PesoDesde,
                    PesoHasta = t.PesoHasta,
                    UnidadMedidaId = t.UnidadMedidaId,
                    UnidadMedidaNombre = um != null ? um.Nombre : null,
                    Puntos = t.Puntos,
                    Activo = t.Activo
                }
            ).FirstOrDefaultAsync(cancellationToken);
        }
    }
}
