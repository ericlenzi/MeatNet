using MediatR;
using Meat.Repositories;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Meat.Application.NumeradoresTropas.GetNumeradorTropa
{
    public class GetNumeradorTropaHandler : IRequestHandler<GetNumeradorTropaRequest, GetNumeradorTropaResponse>
    {
        private readonly MeatContext context;

        public GetNumeradorTropaHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetNumeradorTropaResponse> Handle(GetNumeradorTropaRequest request, CancellationToken cancellationToken)
        {
            return await (
                from nt in this.context.NumeradoresTropas
                where nt.Id == request.Id
                join ce in this.context.ClientesEstablecimientos on nt.ClienteEstablecimientoId equals ce.Id
                join c in this.context.Clientes on ce.ClienteId equals c.Id
                join e in this.context.Establecimientos on ce.EstablecimientoId equals e.Id
                join esp in this.context.Especies on nt.EspecieCodigo equals esp.Codigo
                select new GetNumeradorTropaResponse
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
            ).FirstOrDefaultAsync(cancellationToken);
        }
    }
}
