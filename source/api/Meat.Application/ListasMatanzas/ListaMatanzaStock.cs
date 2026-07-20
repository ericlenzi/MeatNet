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
    /// Calculos de stock de faena por (Tropa, Corral, TipoEspecie) para la Planificacion:
    /// En Pie (lo que dejo el Ingreso aprobado), Reservado (lo comprometido por LMs
    /// Confirmadas/En Ejecucion) y Disponible = En Pie - Reservado.
    /// </summary>
    public static class ListaMatanzaStock
    {
        /// <summary>
        /// Animales En Pie por (Tropa, Corral, TipoEspecie) de tropas Recepcionadas de la especie
        /// en el establecimiento, ya descontado lo faenado historicamente (Romaneo → CantidadFaenada).
        /// </summary>
        public static async Task<Dictionary<(Guid TropaId, Guid AlmacenId, string TipoEspecieId), int>> GetEnPieAsync(
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
                group u by new { TropaId = t.Id, u.AlmacenId, u.TipoEspecieId } into g
                select new { g.Key.TropaId, g.Key.AlmacenId, g.Key.TipoEspecieId, Cantidad = g.Sum(x => x.Cantidad) })
                .ToListAsync(cancellationToken);

            var faenado = await GetFaenadoAsync(context, establecimientoId, especieId, cancellationToken);

            return rows.ToDictionary(
                r => (r.TropaId, r.AlmacenId, r.TipoEspecieId),
                r => r.Cantidad - (faenado.TryGetValue((r.TropaId, r.AlmacenId, r.TipoEspecieId), out var f) ? f : 0));
        }

        /// <summary>
        /// Animales ya faenados por (Tropa, Corral, TipoEspecie) = suma de CantidadFaenada de todos
        /// los renglones de la especie en el establecimiento (cualquier estado de LM: la faena es
        /// permanente). Es la "resta derivada" del consumo real de stock (Ejecucion de Faena).
        /// </summary>
        public static async Task<Dictionary<(Guid TropaId, Guid AlmacenId, string TipoEspecieId), int>> GetFaenadoAsync(
            MeatContext context, Guid establecimientoId, string especieId, CancellationToken cancellationToken)
        {
            var rows = await (
                from d in context.ListasMatanzasDetalles
                join lm in context.ListasMatanzas on d.ListaMatanzaId equals lm.Id
                where lm.EstablecimientoId == establecimientoId
                    && lm.EspecieId == especieId
                    && d.CantidadFaenada > 0
                group d by new { d.TropaId, d.AlmacenId, d.TipoEspecieId } into g
                select new { g.Key.TropaId, g.Key.AlmacenId, g.Key.TipoEspecieId, Cantidad = g.Sum(x => x.CantidadFaenada) })
                .ToListAsync(cancellationToken);

            return rows.ToDictionary(r => (r.TropaId, r.AlmacenId, r.TipoEspecieId), r => r.Cantidad);
        }

        /// <summary>Animales reservados por (Tropa, Corral, TipoEspecie) por LMs Confirmadas / En Ejecucion (excluyendo opcionalmente una LM).</summary>
        public static async Task<Dictionary<(Guid TropaId, Guid AlmacenId, string TipoEspecieId), int>> GetReservadoAsync(
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
                group d by new { d.TropaId, d.AlmacenId, d.TipoEspecieId } into g
                select new { g.Key.TropaId, g.Key.AlmacenId, g.Key.TipoEspecieId, Cantidad = g.Sum(x => x.Cantidad - x.CantidadFaenada) })
                .ToListAsync(cancellationToken);

            return rows.ToDictionary(r => (r.TropaId, r.AlmacenId, r.TipoEspecieId), r => r.Cantidad);
        }
    }
}
