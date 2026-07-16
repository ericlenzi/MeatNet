using Meat.Domain.Tropas;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Tropas
{
    /// <summary>
    /// Helper para registrar eventos en el historial append-only de la Tropa.
    /// Los procesos que afectan a una tropa lo invocan con una sola llamada;
    /// aqui se resuelve la Secuencia (orden del evento dentro de la tropa).
    /// El movimiento se agrega al contexto pero NO se persiste: el handler que
    /// llama es responsable del SaveChanges (para que participe de su transaccion).
    /// </summary>
    public static class TropaMovimientos
    {
        public static async Task RegistrarAsync(
            MeatContext context,
            Guid tropaId,
            string tipoMovimiento,
            string estadoResultanteId,
            string detalle,
            Guid? usuarioId,
            string referenciaTipo,
            Guid? referenciaId,
            CancellationToken cancellationToken)
        {
            // Secuencia = ultima persistida + ultima ya agregada en esta unidad de
            // trabajo (Local), para soportar varios movimientos de una tropa nueva
            // en un mismo SaveChanges sin colisionar el indice unico (TropaId, Secuencia).
            var ultimaDb = await context.TropasMovimientos
                .Where(m => m.TropaId == tropaId)
                .MaxAsync(m => (int?)m.Secuencia, cancellationToken) ?? 0;

            var ultimaLocal = context.TropasMovimientos.Local
                .Where(m => m.TropaId == tropaId)
                .Select(m => m.Secuencia)
                .DefaultIfEmpty(0)
                .Max();

            var movimiento = TropaMovimientoFactory.Create();
            movimiento.TropaId = tropaId;
            movimiento.Secuencia = System.Math.Max(ultimaDb, ultimaLocal) + 1;
            movimiento.Fecha = DateTime.Now;
            movimiento.UsuarioId = usuarioId;
            movimiento.TipoMovimiento = tipoMovimiento;
            movimiento.EstadoResultanteId = estadoResultanteId;
            movimiento.Detalle = detalle;
            movimiento.ReferenciaTipo = referenciaTipo;
            movimiento.ReferenciaId = referenciaId;

            context.TropasMovimientos.Add(movimiento);
        }
    }
}
