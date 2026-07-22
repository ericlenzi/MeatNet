using Meat.Application.Shared;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Numeradores
{
    /// <summary>Tipos de numerador usados por los procesos (columna Numeradores.TipoNumerador).</summary>
    public static class TiposNumerador
    {
        public const string Romaneo = "ROMANEO";
        public const string ListaMatanza = "LISTAMATANZA";
    }

    /// <summary>
    /// Reserva de correlativos sobre la tabla Numeradores, con alcance
    /// (Establecimiento + Especie + TipoNumerador).
    /// </summary>
    public static class Correlativos
    {
        /// <summary>
        /// Reserva el proximo numero con un UPDATE atomico: el UPDATE toma el lock exclusivo de la
        /// fila del numerador y lo retiene hasta el commit, de modo que dos procesos concurrentes no
        /// pueden obtener el mismo numero (un read-modify-write en memoria si lo permitiria, y un
        /// MAX+1 tambien). Debe llamarse dentro de una transaccion, para que el numero se revierta
        /// si el alta falla despues y no queden huecos.
        /// </summary>
        public static async Task<long> ReservarAsync(
            MeatContext context,
            Guid establecimientoId,
            string especieCodigo,
            string tipoNumerador,
            string descripcion,
            CancellationToken cancellationToken)
        {
            const string incrementar = @"
UPDATE Numeradores
SET UltimoNumero = UltimoNumero + 1, FechaActualizacion = SYSDATETIME()
WHERE EstablecimientoId = {0} AND EspecieCodigo = {1} AND TipoNumerador = {2} AND FechaBaja IS NULL";

            var parametros = new object[] { establecimientoId, especieCodigo, tipoNumerador };

            var filas = await context.Database.ExecuteSqlRawAsync(incrementar, parametros, cancellationToken);
            if (filas == 0)
            {
                // Primera vez para este establecimiento/especie: se crea el numerador. El INSERT
                // condicional mas el indice unico de Numeradores evitan duplicarlo si dos procesos
                // llegan a la vez.
                await context.Database.ExecuteSqlRawAsync(@"
INSERT INTO Numeradores (Id, EstablecimientoId, EspecieCodigo, Codigo, Descripcion, TipoNumerador, UltimoNumero, Activo, FechaActualizacion)
SELECT {0}, {1}, {2}, {3}, {4}, {3}, 0, 1, SYSDATETIME()
WHERE NOT EXISTS (
    SELECT 1 FROM Numeradores
    WHERE EstablecimientoId = {1} AND EspecieCodigo = {2} AND TipoNumerador = {3} AND FechaBaja IS NULL)",
                    new object[] { Guid.NewGuid(), establecimientoId, especieCodigo, tipoNumerador, descripcion },
                    cancellationToken);

                filas = await context.Database.ExecuteSqlRawAsync(incrementar, parametros, cancellationToken);
                if (filas == 0)
                    throw new ValidationException($"No se pudo reservar el numero de {descripcion}.");
            }

            // Dentro de la transaccion el UPDATE retiene el lock, asi que esta lectura devuelve el
            // numero que reservo este proceso: nadie pudo incrementarlo en el medio.
            return await context.Numeradores
                .AsNoTracking()
                .Where(n => n.EstablecimientoId == establecimientoId
                    && n.EspecieCodigo == especieCodigo
                    && n.TipoNumerador == tipoNumerador)
                .Select(n => (long)n.UltimoNumero)
                .FirstAsync(cancellationToken);
        }
    }
}
