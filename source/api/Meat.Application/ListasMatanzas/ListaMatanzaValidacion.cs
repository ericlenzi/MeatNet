using Meat.Application.IngresosHaciendas;
using Meat.Application.Shared;
using Meat.Domain.ListasMatanzas;
using Meat.Repositories;
using Microsoft.EntityFrameworkCore;
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
        /// Valida que cada renglon tenga cantidad &gt; 0 y que la suma por (Tropa, Corral,
        /// TipoEspecie) no supere los animales En Pie de esa categoria (R-04, R-05 en Borrador).
        /// </summary>
        public static async Task ValidateRenglonesAsync(
            MeatContext context, Guid establecimientoId, string especieId,
            IEnumerable<ListaMatanzaDetalleInput> renglones, CancellationToken cancellationToken)
        {
            var lista = renglones?.ToList() ?? new List<ListaMatanzaDetalleInput>();

            if (lista.Any(r => r.Cantidad <= 0))
                throw new ValidationException("La cantidad de cada renglon debe ser mayor a cero.");

            if (lista.Any(r => string.IsNullOrEmpty(r.TipoEspecieId)))
                throw new ValidationException("Cada renglon debe indicar la categoria (tipo de especie).");

            if (!lista.Any())
                return;

            var enPie = await ListaMatanzaStock.GetEnPieAsync(context, establecimientoId, especieId, cancellationToken);

            foreach (var grupo in lista.GroupBy(r => (r.TropaId, r.AlmacenId, r.TipoEspecieId)))
            {
                if (!enPie.TryGetValue(grupo.Key, out var disponibleEnPie))
                    throw new ValidationException("Una tropa/corral/categoria seleccionado no tiene hacienda En Pie disponible para faena.");

                var total = grupo.Sum(r => r.Cantidad);
                if (total > disponibleEnPie)
                    throw new ValidationException(
                        $"La cantidad planificada ({total}) supera los animales En Pie de la tropa/corral/categoria ({disponibleEnPie}).");
            }
        }

        /// <summary>
        /// Valida que el nuevo <b>pendiente</b> planificado por esta LM para una (Tropa, Corral,
        /// TipoEspecie) no supere el disponible: En Pie menos lo reservado por otras LM
        /// Confirmadas/En Ejecucion (R-05 post-confirmacion).
        /// <para>
        /// El pendiente (Cantidad - CantidadFaenada), no la Cantidad bruta: el En Pie que devuelve
        /// <see cref="ListaMatanzaStock.GetEnPieAsync"/> ya viene neto de lo faenado, asi que sumar
        /// la cantidad bruta descontaria dos veces la faena propia de la lista.
        /// </para>
        /// </summary>
        public static async Task ValidateDisponibilidadAsync(
            MeatContext context, ListaMatanza lm, Guid tropaId, Guid almacenId, string tipoEspecieId,
            int nuevoPendienteLista, CancellationToken cancellationToken)
        {
            var enPie = await ListaMatanzaStock.GetEnPieAsync(
                context, lm.EstablecimientoId, lm.EspecieId, cancellationToken);
            var reservadoOtras = await ListaMatanzaStock.GetReservadoAsync(
                context, lm.EstablecimientoId, lm.EspecieId, lm.Id, cancellationToken);

            var ep = enPie.TryGetValue((tropaId, almacenId, tipoEspecieId), out var e) ? e : 0;
            if (ep == 0)
                throw new ValidationException("La tropa/corral/categoria no tiene hacienda En Pie disponible para faena.");

            var res = reservadoOtras.TryGetValue((tropaId, almacenId, tipoEspecieId), out var r) ? r : 0;
            if (nuevoPendienteLista > ep - res)
                throw new ValidationException(
                    $"No hay stock disponible suficiente para la tropa/corral/categoria: " +
                    $"En Pie {ep}, reservado por otras listas {res}, se solicitan {nuevoPendienteLista}.");
        }

        /// <summary>
        /// Valida que un almacen destino sea una CAMARA activa del establecimiento (R-A2).
        /// </summary>
        public static async Task ValidateDestinoAsync(
            MeatContext context, Guid establecimientoId, Guid almacenDestinoId, CancellationToken cancellationToken)
        {
            var ok = await (
                from a in context.Almacenes
                join ta in context.TiposAlmacenes on a.TipoAlmacenId equals ta.Codigo
                where a.Id == almacenDestinoId
                    && a.EstablecimientoId == establecimientoId
                    && a.Activo
                    && ta.Familia == FamiliaAlmacen.Camara
                select a.Id).AnyAsync(cancellationToken);
            if (!ok)
                throw new ValidationException("El destino de faena debe ser una camara activa del establecimiento.");
        }

        /// <summary>
        /// Valida los destinos (camaras) informados en los renglones de una LM en Borrador.
        /// El destino es opcional en Borrador; solo se validan los que vengan informados (R-A2).
        /// </summary>
        public static async Task ValidateDestinosAsync(
            MeatContext context, Guid establecimientoId,
            IEnumerable<ListaMatanzaDetalleInput> renglones, CancellationToken cancellationToken)
        {
            var ids = (renglones ?? Enumerable.Empty<ListaMatanzaDetalleInput>())
                .Where(r => r.AlmacenDestinoId.HasValue)
                .Select(r => r.AlmacenDestinoId.Value)
                .Distinct()
                .ToList();
            if (!ids.Any())
                return;

            var validos = await (
                from a in context.Almacenes
                join ta in context.TiposAlmacenes on a.TipoAlmacenId equals ta.Codigo
                where ids.Contains(a.Id)
                    && a.EstablecimientoId == establecimientoId
                    && a.Activo
                    && ta.Familia == FamiliaAlmacen.Camara
                select a.Id).ToListAsync(cancellationToken);

            if (validos.Count != ids.Count)
                throw new ValidationException("Un destino de faena seleccionado no es una camara activa del establecimiento.");
        }
    }
}
