using Meat.Application.Shared;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.ListasMatanzas
{
    /// <summary>
    /// El puesto (palco) donde se va a faenar la lista. Se pide siempre: la Ejecucion de Faena
    /// entra por el puesto y solo muestra las listas que tiene asignadas, asi que una lista sin
    /// puesto no la ve nadie en el palco.
    ///
    /// El puesto tiene que ser de la misma planta y de la misma especie que la lista: el palco
    /// vacuno y el palco porcino son dos puestos distintos.
    /// </summary>
    public static class PuestoListaMatanza
    {
        public static async Task ValidarAsync(
            MeatContext context,
            Guid? puestoId,
            Guid establecimientoId,
            string especieId,
            CancellationToken cancellationToken)
        {
            if (!puestoId.HasValue)
                throw new ValidationException(
                    "Debe indicar el puesto (palco) donde se va a faenar. Si no hay ninguno, "
                    + "configurelo en Datos Maestros > Puestos para este establecimiento y especie.");

            var puesto = await context.Puestos
                .FirstOrDefaultAsync(p => p.Id == puestoId.Value, cancellationToken);

            if (puesto == null)
                throw new ValidationException("El puesto indicado no existe.");
            if (puesto.EstablecimientoId != establecimientoId)
                throw new ValidationException("El puesto indicado no pertenece al establecimiento.");
            if (puesto.EspecieId != especieId)
                throw new ValidationException("El puesto indicado no opera con la especie de la lista.");
            if (!puesto.Activo)
                throw new ValidationException("El puesto indicado no esta activo.");
        }
    }
}
