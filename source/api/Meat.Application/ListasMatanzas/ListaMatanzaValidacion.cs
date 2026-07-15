using Meat.Application.Shared;
using Meat.Domain.ListasMatanzas;
using Meat.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meat.Application.ListasMatanzas
{
    /// <summary>Validaciones de renglones de la Lista de Matanza (armado en Borrador).</summary>
    public static class ListaMatanzaValidacion
    {
        /// <summary>
        /// Valida que cada renglon tenga cantidad &gt; 0 y que la suma por (Tropa, Corral)
        /// no supere los animales En Pie de esa tropa en ese corral (R-04, R-05 en Borrador).
        /// </summary>
        public static async Task ValidateRenglonesAsync(
            MeatContext context, Guid establecimientoId, string especieId,
            IEnumerable<ListaMatanzaDetalleInput> renglones, CancellationToken cancellationToken)
        {
            var lista = renglones?.ToList() ?? new List<ListaMatanzaDetalleInput>();

            if (lista.Any(r => r.Cantidad <= 0))
                throw new ValidationException("La cantidad de cada renglon debe ser mayor a cero.");

            if (!lista.Any())
                return;

            var enPie = await ListaMatanzaStock.GetEnPieAsync(context, establecimientoId, especieId, cancellationToken);

            foreach (var grupo in lista.GroupBy(r => (r.TropaId, r.AlmacenId)))
            {
                if (!enPie.TryGetValue(grupo.Key, out var disponibleEnPie))
                    throw new ValidationException("Una tropa/corral seleccionado no tiene hacienda En Pie disponible para faena.");

                var total = grupo.Sum(r => r.Cantidad);
                if (total > disponibleEnPie)
                    throw new ValidationException(
                        $"La cantidad planificada ({total}) supera los animales En Pie de la tropa/corral ({disponibleEnPie}).");
            }
        }

        /// <summary>
        /// Valida que el nuevo total planificado por esta LM para una (Tropa, Corral)
        /// no supere el disponible: En Pie menos lo reservado por otras LM
        /// Confirmadas/En Ejecucion (R-05 post-confirmacion).
        /// </summary>
        public static async Task ValidateDisponibilidadAsync(
            MeatContext context, ListaMatanza lm, Guid tropaId, Guid almacenId,
            int nuevoTotalLista, CancellationToken cancellationToken)
        {
            var enPie = await ListaMatanzaStock.GetEnPieAsync(
                context, lm.EstablecimientoId, lm.EspecieId, cancellationToken);
            var reservadoOtras = await ListaMatanzaStock.GetReservadoAsync(
                context, lm.EstablecimientoId, lm.EspecieId, lm.Id, cancellationToken);

            var ep = enPie.TryGetValue((tropaId, almacenId), out var e) ? e : 0;
            if (ep == 0)
                throw new ValidationException("La tropa/corral no tiene hacienda En Pie disponible para faena.");

            var res = reservadoOtras.TryGetValue((tropaId, almacenId), out var r) ? r : 0;
            if (nuevoTotalLista > ep - res)
                throw new ValidationException(
                    $"No hay stock disponible suficiente para la tropa/corral: " +
                    $"En Pie {ep}, reservado por otras listas {res}, se solicitan {nuevoTotalLista}.");
        }
    }
}
