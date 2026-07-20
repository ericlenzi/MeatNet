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
                join ta in this.context.TiposAlmacenes on a.TipoAlmacenId equals ta.Codigo into taj
                from ta in taj.DefaultIfEmpty()
                where emp.CodigoEmpresa == request.CodigoEmpresa
                    && (request.EstablecimientoId == null || a.EstablecimientoId == request.EstablecimientoId)
                    && (request.Estado == null || a.Activo == request.Estado)
                    && (request.Familia == null || (ta != null && ta.Familia == request.Familia))
                orderby a.Nombre
                select new AlmacenItem
                {
                    Id = a.Id,
                    CodigoAlmacen = a.CodigoAlmacen,
                    Nombre = a.Nombre,
                    Capacidad = a.Capacidad,
                    TipoAlmacenId = a.TipoAlmacenId,
                    TipoAlmacenFamilia = ta != null ? ta.Familia : null,
                    EstablecimientoId = a.EstablecimientoId,
                    Activo = a.Activo
                }
            ).ToListAsync(cancellationToken);

            return new GetAlmacenesResponse { Data = data };
        }
    }
}
