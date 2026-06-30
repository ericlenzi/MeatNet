using MediatR;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Almacenes.GetAlmacen
{
    public class GetAlmacenHandler : IRequestHandler<GetAlmacenRequest, GetAlmacenResponse>
    {
        private readonly MeatContext context;

        public GetAlmacenHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetAlmacenResponse> Handle(GetAlmacenRequest request, CancellationToken cancellationToken)
        {
            var data = await (
                from a in this.context.Almacenes
                join est in this.context.Establecimientos on a.EstablecimientoId equals est.Id
                join emp in this.context.Empresas on est.EmpresaId equals emp.Id
                join ta in this.context.TiposAlmacenes on a.TipoAlmacenId equals ta.Codigo into taj
                from ta in taj.DefaultIfEmpty()
                where a.Id == request.Id && emp.CodigoEmpresa == request.CodigoEmpresa
                select new GetAlmacenResponse
                {
                    Id = a.Id,
                    CodigoAlmacen = a.CodigoAlmacen,
                    Nombre = a.Nombre,
                    CantidadAnimales = a.CantidadAnimales,
                    TipoAlmacenId = a.TipoAlmacenId,
                    TipoAlmacenNombre = ta != null ? ta.Nombre : null,
                    EstablecimientoId = a.EstablecimientoId,
                    EstablecimientoNombre = est.Nombre,
                    Activo = a.Activo,
                    ERP_Codigo = a.ERP_Codigo
                }
            ).FirstOrDefaultAsync(cancellationToken);

            return data;
        }
    }
}
