using MediatR;
using Meat.Application.IngresosHaciendas;
using Meat.Application.ListasMatanzas;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
                    x.a.Capacidad,
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
                    CapacidadCorral = g.Key.Capacidad,
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

            // Reservado por (Tropa, Corral, TipoEspecie): lo comprometido por listas de
            // matanza Confirmadas / En Ejecucion. Esos animales ya no estan disponibles.
            var reservadoRows = await (
                from d in this.context.ListasMatanzasDetalles
                join lm in this.context.ListasMatanzas on d.ListaMatanzaId equals lm.Id
                join est in this.context.Establecimientos on lm.EstablecimientoId equals est.Id
                join emp in this.context.Empresas on est.EmpresaId equals emp.Id
                where (lm.EstadoListaMatanzaId == EstadosListaMatanza.Confirmada
                        || lm.EstadoListaMatanzaId == EstadosListaMatanza.EnEjecucion)
                    && emp.CodigoEmpresa == request.CodigoEmpresa
                    && (request.EstablecimientoId == null || lm.EstablecimientoId == request.EstablecimientoId)
                group d by new { d.TropaId, d.AlmacenId, d.TipoEspecieId } into g
                select new { g.Key.TropaId, g.Key.AlmacenId, g.Key.TipoEspecieId, Reservado = g.Sum(x => x.Cantidad - x.CantidadFaenada) })
                .ToListAsync(cancellationToken);

            var reservadoPorCategoria = reservadoRows
                .ToDictionary(r => (r.TropaId, r.AlmacenId, r.TipoEspecieId), r => r.Reservado);

            // Faenado por (Tropa, Corral, TipoEspecie): consumo real de la Ejecucion de Faena.
            // Baja el En Pie (cualquier estado de LM; la faena es permanente).
            var faenadoRows = await (
                from d in this.context.ListasMatanzasDetalles
                join lm in this.context.ListasMatanzas on d.ListaMatanzaId equals lm.Id
                join est in this.context.Establecimientos on lm.EstablecimientoId equals est.Id
                join emp in this.context.Empresas on est.EmpresaId equals emp.Id
                where d.CantidadFaenada > 0
                    && emp.CodigoEmpresa == request.CodigoEmpresa
                    && (request.EstablecimientoId == null || lm.EstablecimientoId == request.EstablecimientoId)
                group d by new { d.TropaId, d.AlmacenId, d.TipoEspecieId } into g
                select new { g.Key.TropaId, g.Key.AlmacenId, g.Key.TipoEspecieId, Faenado = g.Sum(x => x.CantidadFaenada) })
                .ToListAsync(cancellationToken);

            var faenadoPorCategoria = faenadoRows
                .ToDictionary(r => (r.TropaId, r.AlmacenId, r.TipoEspecieId), r => r.Faenado);

            // Cada fila de existencia es una (Tropa, Corral, TipoEspecie): faenado y reservado se
            // asignan directo por categoria (sin reparto heuristico).
            foreach (var item in data)
            {
                var pesoPromedio = item.CantidadUN > 0 ? item.PesoKG / item.CantidadUN : 0;

                // En Pie efectivo = recibido - faenado (consumo real).
                var fae = faenadoPorCategoria.TryGetValue((item.TropaId, item.AlmacenId, item.TipoEspecieId), out var ff) ? ff : 0;
                item.CantidadUN = Math.Max(0, item.CantidadUN - fae);
                item.PesoKG = item.CantidadUN * pesoPromedio;

                var res = reservadoPorCategoria.TryGetValue((item.TropaId, item.AlmacenId, item.TipoEspecieId), out var rr)
                    ? Math.Min(rr, item.CantidadUN)
                    : 0;
                item.Reservado = res;
                item.Disponible = item.CantidadUN - res;
                item.DisponibleKG = item.Disponible * pesoPromedio;
            }

            return new GetExistenciaHaciendaResponse { Data = data };
        }
    }
}
