using System;
using System.Collections.Generic;

namespace Meat.Application.ExistenciaHacienda.GetExistenciaHacienda
{
    public class GetExistenciaHaciendaResponse
    {
        public IEnumerable<ExistenciaCorralItem> Data { get; set; } = new List<ExistenciaCorralItem>();
    }

    public class ExistenciaCorralItem
    {
        public Guid AlmacenId { get; set; }
        public string AlmacenNombre { get; set; }
        public int CapacidadCorral { get; set; }

        public string TipoEspecieId { get; set; }
        public string TipoEspecieNombre { get; set; }

        public Guid ClienteId { get; set; }
        public string ClienteNombre { get; set; }

        public Guid TropaId { get; set; }
        public long NumeroTropa { get; set; }

        public int CantidadUN { get; set; }
        public double PesoKG { get; set; }

        // Comprometido por listas de matanza Confirmadas / En Ejecucion, y saldo disponible
        public int Reservado { get; set; }
        public int Disponible { get; set; }
        public double DisponibleKG { get; set; }
    }
}
