using MediatR;
using Meat.Application.IngresosHaciendas;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Tropas.GetTropasDisponibles
{
    public class GetTropasDisponiblesHandler : IRequestHandler<GetTropasDisponiblesRequest, GetTropasDisponiblesResponse>
    {
        private readonly MeatContext context;

        public GetTropasDisponiblesHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetTropasDisponiblesResponse> Handle(GetTropasDisponiblesRequest request, CancellationToken cancellationToken)
        {
            var query =
                from t in this.context.Tropas
                join i in this.context.IngresosHaciendas on t.IngresoHaciendaId equals i.Id
                join esp in this.context.Especies on t.EspecieCodigo equals esp.Codigo
                join c in this.context.Clientes on i.ClienteId equals c.Id
                join est in this.context.Establecimientos on i.EstablecimientoId equals est.Id
                join emp in this.context.Empresas on est.EmpresaId equals emp.Id
                where t.EstadoTropaId == EstadosTropa.Recepcionada
                    && i.EstadoIngresoId == EstadosIngreso.Aprobado
                    && emp.CodigoEmpresa == request.CodigoEmpresa
                    && (request.EstablecimientoId == null || est.Id == request.EstablecimientoId)
                select new TropaDisponibleItem
                {
                    Id = t.Id,
                    NumeroTropa = t.NumeroTropa,
                    EspecieCodigo = t.EspecieCodigo,
                    EspecieNombre = esp.Nombre,
                    ClienteNombre = c.Nombre,
                    IngresoHaciendaId = i.Id,
                    NumeroIngreso = i.NumeroIngreso,
                    EstablecimientoId = est.Id,
                    EstablecimientoNombre = est.Nombre,
                    CabezasEnPie = this.context.IngresosHaciendasUbicaciones
                        .Where(u => u.TropaId == t.Id && u.EstadoHaciendaId == EstadosHacienda.EnPie)
                        .Sum(u => (int?)u.Cantidad) ?? 0,
                    PesoKGEnPie = this.context.IngresosHaciendasUbicaciones
                        .Where(u => u.TropaId == t.Id && u.EstadoHaciendaId == EstadosHacienda.EnPie)
                        .Sum(u => (double?)(u.Cantidad * u.PesoPromedio)) ?? 0
                };

            var data = await query
                .OrderBy(x => x.EstablecimientoNombre)
                .ThenBy(x => x.NumeroTropa)
                .ToListAsync(cancellationToken);

            return new GetTropasDisponiblesResponse { Data = data };
        }
    }
}
