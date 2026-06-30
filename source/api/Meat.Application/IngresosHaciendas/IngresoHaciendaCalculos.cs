using Meat.Domain.IngresosHaciendas;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Meat.Application.IngresosHaciendas
{
    /// <summary>
    /// Calculos del detalle de hacienda: el peso del camion (bruto - tara) se reparte por
    /// tipo especie en el registro de pesadas; el peso promedio de cada ubicacion se calcula
    /// como PesoIngreso(tipo especie) / cantidad total ubicada de ese tipo especie.
    /// </summary>
    public static class IngresoHaciendaCalculos
    {
        public static List<IngresoHaciendaUbicacion> BuildUbicaciones(
            Guid ingresoHaciendaId,
            IEnumerable<IngresoHaciendaPesadaInput> pesadas,
            IEnumerable<IngresoHaciendaUbicacionInput> ubicaciones)
        {
            var pesoPorTipo = pesadas
                .GroupBy(p => p.TipoEspecieId ?? string.Empty)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.PesoIngreso));

            var cantidadPorTipo = ubicaciones
                .GroupBy(u => u.TipoEspecieId ?? string.Empty)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.Cantidad));

            return ubicaciones.Select(u =>
            {
                var key = u.TipoEspecieId ?? string.Empty;
                var peso = pesoPorTipo.TryGetValue(key, out var pp) ? pp : 0;
                var cantidadTotal = cantidadPorTipo.TryGetValue(key, out var cc) ? cc : 0;
                var pesoPromedio = cantidadTotal > 0 ? peso / cantidadTotal : 0;

                return new IngresoHaciendaUbicacion
                {
                    Id = Guid.NewGuid(),
                    IngresoHaciendaId = ingresoHaciendaId,
                    TropaId = null,
                    TipoEspecieId = u.TipoEspecieId,
                    AlmacenId = u.AlmacenId,
                    Cantidad = u.Cantidad,
                    PesoPromedio = pesoPromedio,
                    EstadoHaciendaId = u.EstadoHaciendaId
                };
            }).ToList();
        }
    }
}
