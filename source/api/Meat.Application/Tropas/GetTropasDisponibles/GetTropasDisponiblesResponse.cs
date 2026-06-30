using System;
using System.Collections.Generic;

namespace Meat.Application.Tropas.GetTropasDisponibles
{
    public class GetTropasDisponiblesResponse
    {
        public IEnumerable<TropaDisponibleItem> Data { get; set; } = new List<TropaDisponibleItem>();
    }

    public class TropaDisponibleItem
    {
        public Guid Id { get; set; }
        public long NumeroTropa { get; set; }

        public string EspecieCodigo { get; set; }
        public string EspecieNombre { get; set; }

        public string ClienteNombre { get; set; }

        public Guid IngresoHaciendaId { get; set; }
        public long NumeroIngreso { get; set; }

        public Guid EstablecimientoId { get; set; }
        public string EstablecimientoNombre { get; set; }

        public int CabezasEnPie { get; set; }
        public double PesoKGEnPie { get; set; }
    }
}
