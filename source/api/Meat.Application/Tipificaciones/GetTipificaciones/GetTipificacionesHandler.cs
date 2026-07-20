using MediatR;
using Meat.Application.Shared;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Tipificaciones.GetTipificaciones
{
    public class GetTipificacionesHandler : IRequestHandler<GetTipificacionesRequest, GetTipificacionesResponse>
    {
        private readonly MeatContext context;

        public GetTipificacionesHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetTipificacionesResponse> Handle(GetTipificacionesRequest request, CancellationToken cancellationToken)
        {
            var queryable =
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
                where t.CodigoEmpresa == request.CodigoEmpresa
                    && (request.Estado == null || t.Activo == request.Estado)
                    && (request.EspecieId == null || t.EspecieId == request.EspecieId)
                    && (request.TipoEspecieId == null || t.TipoEspecieId == request.TipoEspecieId)
                    && (string.IsNullOrEmpty(request.Filter)
                        || t.Codigo.Contains(request.Filter)
                        || t.Descripcion.Contains(request.Filter))
                orderby t.Puntos descending, t.Codigo
                select new TipificacionItem
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
                    Puntos = t.Puntos,
                    Activo = t.Activo
                };

            var totalRows = await queryable.CountAsync(cancellationToken);
            var data = await queryable.Page(request.PageSize, request.PageIndex).ToListAsync(cancellationToken);

            return new GetTipificacionesResponse { Data = data, TotalRows = totalRows };
        }
    }
}
