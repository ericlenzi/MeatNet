using Meat.Application.Shared;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.DespiecesMateriales.Shared
{
    /// <summary>Validaciones comunes de una regla de despiece (mini-BOM).</summary>
    public static class DespieceMaterialValidacion
    {
        public static async Task ValidateAsync(
            MeatContext context,
            Guid? idActual,
            Guid materialOrigenId,
            Guid materialDestinoId,
            int cantidad,
            double rendimiento,
            CancellationToken cancellationToken)
        {
            if (materialOrigenId == materialDestinoId)
                throw new ValidationException("El material origen y el destino no pueden ser el mismo.");

            if (cantidad < 1)
                throw new ValidationException("La cantidad debe ser al menos 1.");

            if (rendimiento <= 0 || rendimiento > 1)
                throw new ValidationException("El rendimiento debe estar entre 0 y 1.");

            if (!await context.Materiales.AnyAsync(m => m.Id == materialOrigenId, cancellationToken))
                throw new ValidationException("El material origen no existe.");

            if (!await context.Materiales.AnyAsync(m => m.Id == materialDestinoId, cancellationToken))
                throw new ValidationException("El material destino no existe.");

            var duplicado = await context.DespiecesMateriales.AnyAsync(
                d => d.MaterialOrigenId == materialOrigenId
                    && d.MaterialDestinoId == materialDestinoId
                    && (idActual == null || d.Id != idActual),
                cancellationToken);
            if (duplicado)
                throw new ValidationException("Ya existe un despiece para ese par origen-destino.");

            // La suma de rendimientos activos por material origen no puede superar 1 (100%).
            var sumaOtros = await context.DespiecesMateriales
                .Where(d => d.MaterialOrigenId == materialOrigenId && d.Activo
                    && (idActual == null || d.Id != idActual))
                .SumAsync(d => (double?)d.Rendimiento, cancellationToken) ?? 0;

            if (sumaOtros + rendimiento > 1.0001)
                throw new ValidationException(
                    $"La suma de rendimientos del material origen superaria 1 (100%). Disponible: {(1 - sumaOtros):0.###}.");
        }
    }
}
