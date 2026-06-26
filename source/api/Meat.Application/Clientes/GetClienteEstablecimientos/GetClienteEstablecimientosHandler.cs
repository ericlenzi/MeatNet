using MediatR;
using Microsoft.EntityFrameworkCore;
using Meat.Repositories;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Clientes.GetClienteEstablecimientos
{
    public class GetClienteEstablecimientosHandler : IRequestHandler<GetClienteEstablecimientosRequest, GetClienteEstablecimientosResponse>
    {
        private readonly MeatContext context;

        public GetClienteEstablecimientosHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetClienteEstablecimientosResponse> Handle(GetClienteEstablecimientosRequest request, CancellationToken cancellationToken)
        {
            var items = await (
                from ce in this.context.ClientesEstablecimientos
                where ce.ClienteId == request.ClienteId
                join e in this.context.Establecimientos on ce.EstablecimientoId equals e.Id
                join s in this.context.Sucursales on e.SucursalId equals s.Id
                orderby s.Nombre, e.Nombre
                select new ClienteEstablecimientoItem
                {
                    Id = ce.Id,
                    EstablecimientoId = e.Id,
                    CodigoEstablecimiento = e.CodigoEstablecimiento,
                    Nombre = e.Nombre,
                    SucursalId = e.SucursalId,
                    CodigoSucursal = s.CodigoSucursal,
                    NombreSucursal = s.Nombre,
                    CodigoRenspa = ce.CodigoRenspa,
                    NumeroCUIG = ce.NumeroCUIG
                }
            ).ToListAsync(cancellationToken);

            return new GetClienteEstablecimientosResponse { Data = items };
        }
    }
}
