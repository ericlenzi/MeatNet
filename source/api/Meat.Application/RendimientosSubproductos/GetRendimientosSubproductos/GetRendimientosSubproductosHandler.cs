using MediatR;
using Meat.Application.Shared;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.RendimientosSubproductos.GetRendimientosSubproductos
{
    public class GetRendimientosSubproductosHandler
        : IRequestHandler<GetRendimientosSubproductosRequest, GetRendimientosSubproductosResponse>
    {
        private readonly MeatContext context;

        public GetRendimientosSubproductosHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetRendimientosSubproductosResponse> Handle(
            GetRendimientosSubproductosRequest request, CancellationToken cancellationToken)
        {
            // Sin filtro por empresa: lo aplica el query filter global del MeatContext.
            var queryable =
                from r in this.context.RendimientosSubproductos
                join m in this.context.Materiales on r.MaterialId equals m.Id
                join e in this.context.Especies on r.EspecieId equals e.Codigo into ej
                from e in ej.DefaultIfEmpty()
                where (request.EspecieId == null || r.EspecieId == request.EspecieId)
                    && (request.Estado == null || r.Activo == request.Estado)
                    && (string.IsNullOrEmpty(request.Filter)
                        || m.Nombre.Contains(request.Filter)
                        || m.CodigoMaterial.Contains(request.Filter))
                orderby r.EspecieId, m.Nombre
                select new RendimientoSubproductoItem
                {
                    Id = r.Id,
                    EspecieId = r.EspecieId,
                    EspecieNombre = e != null ? e.Nombre : null,
                    MaterialId = r.MaterialId,
                    MaterialCodigo = m.CodigoMaterial,
                    MaterialNombre = m.Nombre,
                    Porcentaje = r.Porcentaje,
                    Activo = r.Activo
                };

            var totalRows = await queryable.CountAsync(cancellationToken);
            var data = await queryable.Page(request.PageSize, request.PageIndex).ToListAsync(cancellationToken);

            return new GetRendimientosSubproductosResponse { Data = data, TotalRows = totalRows };
        }
    }
}
