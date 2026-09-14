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
        // Identificadores entre comillas: PostgreSQL pasa a minusculas los que no las llevan.
        // La fecha la pone la API (unico reloj), no now() de la base.
        private const string Incrementar = @"
UPDATE meat.""Numeradores""
SET ""UltimoNumero"" = ""UltimoNumero"" + 1, ""FechaActualizacion"" = {3}
WHERE ""EstablecimientoId"" = {0} AND ""EspecieCodigo"" = {1} AND ""TipoNumerador"" = {2} AND ""FechaBaja"" IS NULL
RETURNING ""UltimoNumero"" AS ""Value""";

        /// <summary>
        /// Reserva el proximo numero con un UPDATE atomico: el UPDATE toma el lock exclusivo de la
        /// fila del numerador y lo retiene hasta el commit, de modo que dos procesos concurrentes no
        /// pueden obtener el mismo numero (un read-modify-write en memoria si lo permitiria, y un
        /// MAX+1 tambien). El RETURNING devuelve el numero reservado en la misma sentencia.
        /// Debe llamarse dentro de una transaccion, para que el numero se revierta si el alta falla
        /// despues y no queden huecos.
        /// </summary>
        public static async Task<long> ReservarAsync(
            MeatContext context,
            Guid establecimientoId,
            string especieCodigo,
            string tipoNumerador,
            string descripcion,
            CancellationToken cancellationToken)
        {
            var ahora = DateTime.Now;
            var parametros = new object[] { establecimientoId, especieCodigo, tipoNumerador, ahora };

            var numero = await IncrementarAsync(context, parametros, cancellationToken);
            if (numero == null)
            {
                // Primera vez para este establecimiento/especie: se crea el numerador, en la empresa
                // del establecimiento. El INSERT condicional mas el indice unico de Numeradores
                // evitan duplicarlo si dos procesos llegan a la vez.
                await context.Database.ExecuteSqlRawAsync(@"
INSERT INTO meat.""Numeradores"" (""Id"", ""EmpresaId"", ""EstablecimientoId"", ""EspecieCodigo"", ""Codigo"", ""Descripcion"", ""TipoNumerador"", ""UltimoNumero"", ""Activo"", ""FechaActualizacion"")
SELECT {0}, e.""EmpresaId"", e.""Id"", {2}, {3}, {4}, {3}, 0, true, {5}
FROM meat.""Establecimientos"" e
WHERE e.""Id"" = {1}
  AND NOT EXISTS (
    SELECT 1 FROM meat.""Numeradores""
    WHERE ""EstablecimientoId"" = {1} AND ""EspecieCodigo"" = {2} AND ""TipoNumerador"" = {3} AND ""FechaBaja"" IS NULL)",
                    new object[] { Guid.NewGuid(), establecimientoId, especieCodigo, tipoNumerador, descripcion, ahora },
                    cancellationToken);

                numero = await IncrementarAsync(context, parametros, cancellationToken);
                if (numero == null)
                    throw new ValidationException($"No se pudo reservar el numero de {descripcion}.");
            }

            return numero.Value;
        }

        private static async Task<int?> IncrementarAsync(MeatContext context, object[] parametros, CancellationToken cancellationToken)
        {
            // ToListAsync y no FirstOrDefaultAsync: un operador LINQ haria que EF envuelva el UPDATE
            // en un SELECT, que PostgreSQL no acepta.
            var filas = await context.Database
                .SqlQueryRaw<int>(Incrementar, parametros)
                .ToListAsync(cancellationToken);

            return filas.Count == 0 ? null : filas[0];
        }
    }
}
