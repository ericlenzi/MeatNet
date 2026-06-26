using MediatR;
using Meat.Repositories;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Meat.Application.NumeradoresTropas.GetNumeradoresTropas
{
    public class GetNumeradoresTropasHandler : IRequestHandler<GetNumeradoresTropasRequest, GetNumeradoresTropasResponse>
    {
        private readonly MeatContext context;

        public GetNumeradoresTropasHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetNumeradoresTropasResponse> Handle(GetNumeradoresTropasRequest request, CancellationToken cancellationToken)
        {
            var items = await (
                from nt in this.context.NumeradoresTropas
                join ce in this.context.ClientesEstablecimientos on nt.ClienteEstablecimientoId equals ce.Id
                join c in this.context.Clientes on ce.ClienteId equals c.Id
                join e in this.context.Establecimientos on ce.EstablecimientoId equals e.Id
                join esp in this.context.Especies on nt.EspecieCodigo equals esp.Codigo
                orderby c.Nombre, e.Nombre, esp.Nombre
                select new NumeradorTropaItem
                {
                    Id = nt.Id,
                    ClienteEstablecimientoId = ce.Id,
                    CodigoCliente = c.CodigoCliente,
                    NombreCliente = c.Nombre,
                    CodigoEstablecimiento = e.CodigoEstablecimiento,
                    NombreEstablecimiento = e.Nombre,
                    EspecieCodigo = esp.Codigo,
                    EspecieNombre = esp.Nombre,
                    UltimoNumeroTropa = nt.UltimoNumeroTropa
                }
            ).ToListAsync(cancellationToken);

            return new GetNumeradoresTropasResponse { Data = items };
        }
    }
}
