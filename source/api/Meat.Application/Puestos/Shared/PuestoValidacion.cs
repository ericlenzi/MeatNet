using Meat.Application.Shared;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.Puestos.Shared
{
    /// <summary>
    /// Lo que tienen que cumplir los datos de un puesto, en el alta y en la edicion.
    /// El puesto es la triple configuracion Establecimiento + Especie + tipo de puesto, asi que
    /// las tres puntas se validan juntas: un palco vacuno en una planta que no faena vacunos no
    /// es un dato incompleto, es un dato imposible.
    /// </summary>
    public static class PuestoValidacion
    {
        public static async Task ValidarAsync(
            MeatContext context,
            Guid establecimientoId,
            string especieId,
            string nombre,
            string tipoPuestoId,
            string tipoMedicionId,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ValidationException("El nombre es requerido.");

            var establecimiento = await context.Establecimientos
                .FirstOrDefaultAsync(e => e.Id == establecimientoId, cancellationToken);
            if (establecimiento == null)
                throw new ValidationException("El establecimiento indicado no existe.");

            if (string.IsNullOrWhiteSpace(especieId))
                throw new ValidationException("La especie es requerida.");

            var especieHabilitada = await context.EstablecimientosEspecies
                .AnyAsync(ee => ee.EstablecimientoId == establecimientoId
                    && ee.EspecieId == especieId, cancellationToken);
            if (!especieHabilitada)
                throw new ValidationException("La especie no esta habilitada para el establecimiento.");

            if (string.IsNullOrWhiteSpace(tipoPuestoId))
                throw new ValidationException("El tipo de puesto es requerido.");

            var tipoPuestoValido = await context.TiposPuestos
                .AnyAsync(t => t.Codigo == tipoPuestoId && t.Activo, cancellationToken);
            if (!tipoPuestoValido)
                throw new ValidationException("El tipo de puesto indicado no existe o no esta activo.");

            if (string.IsNullOrWhiteSpace(tipoMedicionId))
                throw new ValidationException("El tipo de medicion por defecto es requerido.");

            var tipoMedicionValido = await context.TiposMediciones
                .AnyAsync(t => t.Codigo == tipoMedicionId && t.Activo, cancellationToken);
            if (!tipoMedicionValido)
                throw new ValidationException("El tipo de medicion indicado no existe o no esta activo.");
        }
    }
}
