using Meat.Application.IngresosHaciendas;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.ListasMatanzas
{
    /// <summary>
    /// Calculos de stock de faena por (Tropa, Corral) para la Planificacion:
    /// En Pie (lo que dejo el Ingreso aprobado), Reservado (lo comprometido por LMs
    /// Confirmadas/En Ejecucion) y Disponible = En Pie - Reservado.
    /// </summary>
    public static class ListaMatanzaStock
    {
        /// <summary>Animales En Pie por (Tropa, Corral) de tropas Recepcionadas de la especie en el establecimiento.</summary>
        public static async Task<Dictionary<(Guid TropaId, Guid AlmacenId), int>> GetEnPieAsync(
            MeatContext context, Guid establecimientoId, string especieId, CancellationToken cancellationToken)
        {
            var rows = await (
                from u in context.IngresosHaciendasUbicaciones
                join t in context.Tropas on u.TropaId equals (Guid?)t.Id
                join i in context.IngresosHaciendas on u.IngresoHaciendaId equals i.Id
                where i.EstadoIngresoId == EstadosIngreso.Aprobado
                    && t.EstadoTropaId == EstadosTropa.Recepcionada
                    && u.EstadoHaciendaId == EstadosHacienda.EnPie
                    && i.EstablecimientoId == establecimientoId
                    && t.EspecieCodigo == especieId
                group u by new { TropaId = t.Id, u.AlmacenId } into g
                select new { g.Key.TropaId, g.Key.AlmacenId, Cantidad = g.Sum(x => x.Cantidad) })
                .ToListAsync(cancellationToken);

            return rows.ToDictionary(r => (r.TropaId, r.AlmacenId), r => r.Cantidad);
        }

        /// <summary>Animales reservados por (Tropa, Corral) por LMs Confirmadas / En Ejecucion (excluyendo opcionalmente una LM).</summary>
        public static async Task<Dictionary<(Guid TropaId, Guid AlmacenId), int>> GetReservadoAsync(
            MeatContext context, Guid establecimientoId, string especieId, Guid? excludeListaId, CancellationToken cancellationToken)
        {
            var rows = await (
                from d in context.ListasMatanzasDetalles
                join lm in context.ListasMatanzas on d.ListaMatanzaId equals lm.Id
                where (lm.EstadoListaMatanzaId == EstadosListaMatanza.Confirmada
                        || lm.EstadoListaMatanzaId == EstadosListaMatanza.EnEjecucion)
                    && lm.EstablecimientoId == establecimientoId
                    && lm.EspecieId == especieId
                    && (excludeListaId == null || lm.Id != excludeListaId)
                group d by new { d.TropaId, d.AlmacenId } into g
                select new { g.Key.TropaId, g.Key.AlmacenId, Cantidad = g.Sum(x => x.Cantidad - x.CantidadFaenada) })
                .ToListAsync(cancellationToken);

            return rows.ToDictionary(r => (r.TropaId, r.AlmacenId), r => r.Cantidad);
        }
    }
}
