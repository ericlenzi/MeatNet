using MediatR;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.RendimientosSubproductos.GetRendimientoSubproducto
{
    public class GetRendimientoSubproductoHandler
        : IRequestHandler<GetRendimientoSubproductoRequest, GetRendimientoSubproductoResponse>
    {
        private readonly MeatContext context;

        public GetRendimientoSubproductoHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetRendimientoSubproductoResponse> Handle(
            GetRendimientoSubproductoRequest request, CancellationToken cancellationToken)
        {
            return await (
                from r in this.context.RendimientosSubproductos
                join m in this.context.Materiales on r.MaterialId equals m.Id
                join e in this.context.Especies on r.EspecieId equals e.Codigo into ej
                from e in ej.DefaultIfEmpty()
                where r.Id == request.Id
                select new GetRendimientoSubproductoResponse
                {
                    Id = r.Id,
                    EspecieId = r.EspecieId,
                    EspecieNombre = e != null ? e.Nombre : null,
                    MaterialId = r.MaterialId,
                    MaterialNombre = m.Nombre,
                    Porcentaje = r.Porcentaje,
                    Activo = r.Activo
                }
            ).FirstOrDefaultAsync(cancellationToken);
        }
    }
}
