using System;
using System.Collections.Generic;

namespace Meat.Application.IngresosHaciendas.AprobarIngresoHacienda
{
    public class AprobarIngresoHaciendaResponse
    {
        /// <summary>Tropas generadas al aprobar (una por especie).</summary>
        public List<TropaGenerada> Tropas { get; set; } = new List<TropaGenerada>();

        /// <summary>Advertencia no bloqueante (p. ej. diferencia de pesaje fuera de tolerancia).</summary>
        public string Advertencia { get; set; }
    }

    public class TropaGenerada
    {
        public Guid Id { get; set; }
        public string EspecieCodigo { get; set; }
        public long NumeroTropa { get; set; }
    }
}
