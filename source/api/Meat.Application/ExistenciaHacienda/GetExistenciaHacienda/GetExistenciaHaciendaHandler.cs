using MediatR;
using Meat.Application.IngresosHaciendas;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.ExistenciaHacienda.GetExistenciaHacienda
{
    public class GetExistenciaHaciendaHandler : IRequestHandler<GetExistenciaHaciendaRequest, GetExistenciaHaciendaResponse>
    {
        private readonly MeatContext context;

        public GetExistenciaHaciendaHandler(MeatContext context)
        {
            this.context = context;
        }

        public async Task<GetExistenciaHaciendaResponse> Handle(GetExistenciaHaciendaRequest request, CancellationToken cancellationToken)
        {
            var baseQuery =
                from u in this.context.IngresosHaciendasUbicaciones
                join t in this.context.Tropas on u.TropaId equals (Guid?)t.Id
                join i in this.context.IngresosHaciendas on u.IngresoHaciendaId equals i.Id
                join a in this.context.Almacenes on u.AlmacenId equals a.Id
                join te in this.context.TiposEspecies on u.TipoEspecieId equals te.Id
                join c in this.context.Clientes on i.ClienteId equals c.Id
                join est in this.context.Establecimientos on i.EstablecimientoId equals est.Id
                join emp in this.context.Empresas on est.EmpresaId equals emp.Id
                where i.EstadoIngresoId == EstadosIngreso.Aprobado
                    && t.EstadoTropaId == EstadosTropa.Recepcionada
                    && u.EstadoHaciendaId == EstadosHacienda.EnPie
                    && emp.CodigoEmpresa == request.CodigoEmpresa
                    && (request.EstablecimientoId == null || est.Id == request.EstablecimientoId)
                select new { u, t, a, te, c };

            var data = await baseQuery
                .GroupBy(x => new
                {
                    x.a.Id,
                    AlmacenNombre = x.a.Nombre,
                    x.a.CantidadAnimales,
                    TipoEspecieId = x.te.Id,
                    TipoEspecieNombre = x.te.Nombre,
                    ClienteId = x.c.Id,
                    ClienteNombre = x.c.Nombre,
                    TropaId = x.t.Id,
                    x.t.NumeroTropa
                })
                .Select(g => new ExistenciaCorralItem
                {
                    AlmacenId = g.Key.Id,
                    AlmacenNombre = g.Key.AlmacenNombre,
                    CapacidadCorral = g.Key.CantidadAnimales,
                    TipoEspecieId = g.Key.TipoEspecieId,
                    TipoEspecieNombre = g.Key.TipoEspecieNombre,
                    ClienteId = g.Key.ClienteId,
                    ClienteNombre = g.Key.ClienteNombre,
                    TropaId = g.Key.TropaId,
                    NumeroTropa = g.Key.NumeroTropa,
                    CantidadUN = g.Sum(x => x.u.Cantidad),
                    PesoKG = g.Sum(x => x.u.Cantidad * x.u.PesoPromedio)
                })
                .ToListAsync(cancellationToken);

            return new GetExistenciaHaciendaResponse { Data = data };
        }
    }
}
