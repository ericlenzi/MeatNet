using MediatR;
using Meat.Application.Shared;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Puestos.GetPuestos
{
    public class GetPuestosHandler : IRequestHandler<GetPuestosRequest, GetPuestosResponse>
    {
        private readonly MeatContext context;

        public GetPuestosHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetPuestosResponse> Handle(GetPuestosRequest request, CancellationToken cancellationToken)
        {
            // Sin filtro por empresa: lo aplica el query filter global del MeatContext.
            var queryable =
                from p in this.context.Puestos
                join est in this.context.Establecimientos on p.EstablecimientoId equals est.Id
                join e in this.context.Especies on p.EspecieId equals e.Codigo into ej
                from e in ej.DefaultIfEmpty()
                join tp in this.context.TiposPuestos on p.TipoPuestoId equals tp.Codigo into tpj
                from tp in tpj.DefaultIfEmpty()
                join tm in this.context.TiposMediciones on p.TipoMedicionId equals tm.Codigo into tmj
                from tm in tmj.DefaultIfEmpty()
                where (request.EstablecimientoId == null || p.EstablecimientoId == request.EstablecimientoId)
                    && (request.EspecieId == null || p.EspecieId == request.EspecieId)
                    && (request.Estado == null || p.Activo == request.Estado)
                    && (string.IsNullOrEmpty(request.Filter)
                        || p.CodigoPuesto.Contains(request.Filter)
                        || p.Nombre.Contains(request.Filter))
                orderby p.CodigoPuesto
                select new PuestoItem
                {
                    Id = p.Id,
                    CodigoPuesto = p.CodigoPuesto,
                    Nombre = p.Nombre,
                    EstablecimientoId = p.EstablecimientoId,
                    EstablecimientoNombre = est.Nombre,
                    EspecieId = p.EspecieId,
                    EspecieNombre = e != null ? e.Nombre : null,
                    TipoPuestoId = p.TipoPuestoId,
                    TipoPuestoNombre = tp != null ? tp.Nombre : null,
                    TipoMedicionId = p.TipoMedicionId,
                    TipoMedicionNombre = tm != null ? tm.Nombre : null,
                    Activo = p.Activo
                };

            var totalRows = await queryable.CountAsync(cancellationToken);
            var data = await queryable.Page(request.PageSize, request.PageIndex).ToListAsync(cancellationToken);

            return new GetPuestosResponse { Data = data, TotalRows = totalRows };
        }
    }
}
