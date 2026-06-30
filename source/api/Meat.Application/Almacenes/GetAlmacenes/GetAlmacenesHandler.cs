using MediatR;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Almacenes.GetAlmacenes
{
    public class GetAlmacenesHandler : IRequestHandler<GetAlmacenesRequest, GetAlmacenesResponse>
    {
        private readonly MeatContext context;

        public GetAlmacenesHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetAlmacenesResponse> Handle(GetAlmacenesRequest request, CancellationToken cancellationToken)
        {
            var data = await (
                from a in this.context.Almacenes
                join est in this.context.Establecimientos on a.EstablecimientoId equals est.Id
                join emp in this.context.Empresas on est.EmpresaId equals emp.Id
                where emp.CodigoEmpresa == request.CodigoEmpresa
                    && (request.EstablecimientoId == null || a.EstablecimientoId == request.EstablecimientoId)
                    && (request.Estado == null || a.Activo == request.Estado)
                orderby a.Nombre
                select new AlmacenItem
                {
                    Id = a.Id,
                    CodigoAlmacen = a.CodigoAlmacen,
                    Nombre = a.Nombre,
                    CantidadAnimales = a.CantidadAnimales,
                    TipoAlmacenId = a.TipoAlmacenId,
                    EstablecimientoId = a.EstablecimientoId,
                    Activo = a.Activo
                }
            ).ToListAsync(cancellationToken);

            return new GetAlmacenesResponse { Data = data };
        }
    }
}
