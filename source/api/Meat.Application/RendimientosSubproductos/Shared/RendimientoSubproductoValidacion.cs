using Meat.Application.Shared;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.RendimientosSubproductos.Shared
{
    /// <summary>
    /// Lo que tiene que cumplir un rendimiento de subproducto, en el alta y en la edicion.
    /// </summary>
    public static class RendimientoSubproductoValidacion
    {
        /// <summary>Tipos de material que son subproducto: lo unico que se puede estimar asi.</summary>
        private static readonly string[] TiposSubproducto = { "SUB_PROD", "MENUD" };

        public static async Task ValidarAsync(
            MeatContext context,
            string especieId,
            Guid materialId,
            double porcentaje,
            Guid? excepto,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(especieId))
                throw new ValidationException("La especie es requerida.");

            if (!await context.Especies.AnyAsync(e => e.Codigo == especieId, cancellationToken))
                throw new ValidationException("La especie indicada no existe.");

            var material = await context.Materiales
                .FirstOrDefaultAsync(m => m.Id == materialId, cancellationToken);
            if (material == null)
                throw new ValidationException("El material indicado no existe.");
            if (!material.Activo)
                throw new ValidationException("El material indicado no esta activo.");

            // Un rendimiento estima subproducto. Una media res no se estima: se pesa.
            if (!TiposSubproducto.Contains(material.TipoMaterialId))
                throw new ValidationException(
                    "El material tiene que ser un subproducto (cuero, sebo, menudencias). La carne se pesa en el romaneo, no se estima.");

            if (porcentaje <= 0 || porcentaje >= 100)
                throw new ValidationException("El porcentaje debe ser mayor a 0 y menor a 100.");

            var duplicado = await context.RendimientosSubproductos
                .AnyAsync(r => r.EspecieId == especieId
                    && r.MaterialId == materialId
                    && (excepto == null || r.Id != excepto.Value), cancellationToken);
            if (duplicado)
                throw new ValidationException("Ya hay un rendimiento cargado para esa especie y ese subproducto.");

            // Tope de cordura: los subproductos de una res suman una fraccion de su peso (cuero,
            // sebo y menudencias rondan el 16% juntos). Pasar de 100 no es un ajuste fino, es un
            // error de carga que despues aparece como kilos inventados en el analisis.
            var sumaOtros = await context.RendimientosSubproductos
                .Where(r => r.EspecieId == especieId
                    && r.Activo
                    && (excepto == null || r.Id != excepto.Value))
                .SumAsync(r => (double?)r.Porcentaje, cancellationToken) ?? 0;

            if (sumaOtros + porcentaje > 100)
                throw new ValidationException(
                    $"Los rendimientos de la especie sumarian {sumaOtros + porcentaje:0.##}% del peso de la res. Revise los porcentajes.");
        }
    }
}
