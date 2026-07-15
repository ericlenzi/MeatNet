using MediatR;
using Meat.Application.IngresosHaciendas;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.ListasMatanzas.GetDisponibilidadFaena
{
    public class GetDisponibilidadFaenaHandler : IRequestHandler<GetDisponibilidadFaenaRequest, GetDisponibilidadFaenaResponse>
    {
        private readonly MeatContext context;

        public GetDisponibilidadFaenaHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetDisponibilidadFaenaResponse> Handle(GetDisponibilidadFaenaRequest request, CancellationToken cancellationToken)
        {
            // En Pie por (Tropa, Corral) con metadatos, filtrado por empresa/establecimiento/especie
            var enPieRows = await (
                from u in this.context.IngresosHaciendasUbicaciones
                join t in this.context.Tropas on u.TropaId equals (System.Guid?)t.Id
                join i in this.context.IngresosHaciendas on u.IngresoHaciendaId equals i.Id
                join a in this.context.Almacenes on u.AlmacenId equals a.Id
                join c in this.context.Clientes on i.ClienteId equals c.Id
                join est in this.context.Establecimientos on i.EstablecimientoId equals est.Id
                join emp in this.context.Empresas on est.EmpresaId equals emp.Id
                where i.EstadoIngresoId == EstadosIngreso.Aprobado
                    && t.EstadoTropaId == EstadosTropa.Recepcionada
                    && u.EstadoHaciendaId == EstadosHacienda.EnPie
                    && emp.CodigoEmpresa == request.CodigoEmpresa
                    && est.Id == request.EstablecimientoId
                    && t.EspecieCodigo == request.EspecieId
                group new { u, c } by new
                {
                    TropaId = t.Id,
                    t.NumeroTropa,
                    AlmacenId = a.Id,
                    AlmacenNombre = a.Nombre,
                    ClienteId = c.Id,
                    ClienteNombre = c.Nombre
                } into g
                select new
                {
                    g.Key.TropaId,
                    g.Key.NumeroTropa,
                    g.Key.AlmacenId,
                    g.Key.AlmacenNombre,
                    g.Key.ClienteId,
                    g.Key.ClienteNombre,
                    EnPie = g.Sum(x => x.u.Cantidad),
                    PesoTotal = g.Sum(x => x.u.Cantidad * x.u.PesoPromedio)
                })
                .ToListAsync(cancellationToken);

            var reservado = await ListaMatanzaStock.GetReservadoAsync(
                this.context, request.EstablecimientoId, request.EspecieId, request.ExcludeListaId, cancellationToken);

            var data = enPieRows
                .Select(r =>
                {
                    var res = reservado.TryGetValue((r.TropaId, r.AlmacenId), out var rr) ? rr : 0;
                    return new DisponibilidadFaenaItem
                    {
                        TropaId = r.TropaId,
                        NumeroTropa = r.NumeroTropa,
                        AlmacenId = r.AlmacenId,
                        AlmacenNombre = r.AlmacenNombre,
                        ClienteId = r.ClienteId,
                        ClienteNombre = r.ClienteNombre,
                        EnPie = r.EnPie,
                        Reservado = res,
                        Disponible = r.EnPie - res,
                        PesoPromedio = r.EnPie > 0 ? r.PesoTotal / r.EnPie : 0
                    };
                })
                .Where(x => x.Disponible > 0)
                .OrderBy(x => x.NumeroTropa).ThenBy(x => x.AlmacenNombre)
                .ToList();

            return new GetDisponibilidadFaenaResponse { Data = data };
        }
    }
}
