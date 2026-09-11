using Meat.Application.Shared;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Tipificadores.Shared
{
    /// <summary>
    /// Lo que tienen que cumplir los datos de un tipificador, en el alta y en la edicion.
    /// La habilitacion es por Establecimiento + Especie, igual que el Puesto: quien tipifica
    /// vacunos en una planta no queda habilitado para los porcinos de otra.
    /// </summary>
    public static class TipificadorValidacion
    {
        public static async Task ValidarAsync(
            MeatContext context,
            Guid establecimientoId,
            string especieId,
            string nombre,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ValidationException("El nombre es requerido.");

            var existeEstablecimiento = await context.Establecimientos
                .AnyAsync(e => e.Id == establecimientoId, cancellationToken);
            if (!existeEstablecimiento)
                throw new ValidationException("El establecimiento indicado no existe.");

            if (string.IsNullOrWhiteSpace(especieId))
                throw new ValidationException("La especie es requerida.");

            var especieHabilitada = await context.EstablecimientosEspecies
                .AnyAsync(ee => ee.EstablecimientoId == establecimientoId
                    && ee.EspecieId == especieId, cancellationToken);
            if (!especieHabilitada)
                throw new ValidationException("La especie no esta habilitada para el establecimiento.");
        }

        /// <summary>
        /// Un solo tipificador por defecto por Establecimiento + Especie: al marcar uno se
        /// destildan los demas, igual que hace el CRUD de UnidadesFaenas con su unidad por
        /// defecto. Sin esto, el indice unico filtrado rebota el guardado con un error de SQL.
        /// </summary>
        public static async Task DestildarOtrosPorDefectoAsync(
            MeatContext context,
            Guid establecimientoId,
            string especieId,
            Guid? excepto,
            CancellationToken cancellationToken)
        {
            var otros = await context.Tipificadores
                .Where(t => t.EstablecimientoId == establecimientoId
                    && t.EspecieId == especieId
                    && t.PorDefecto
                    && (excepto == null || t.Id != excepto.Value))
                .ToListAsync(cancellationToken);

            foreach (var t in otros)
            {
                t.PorDefecto = false;
                t.FechaActualizacion = DateTime.Now;
            }
        }
    }
}
