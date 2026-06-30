using System;

namespace Meat.Application.IngresosHaciendas
{
    /// <summary>Linea del registro de pesadas (entrada desde el cliente).</summary>
    public class IngresoHaciendaPesadaInput
    {
        public string TipoEspecieId { get; set; }
        public double PesoIngreso { get; set; }
    }

    /// <summary>Linea de ubicacion en corral (entrada desde el cliente).</summary>
    public class IngresoHaciendaUbicacionInput
    {
        public string TipoEspecieId { get; set; }
        public Guid AlmacenId { get; set; }
        public int Cantidad { get; set; }
        public string EstadoHaciendaId { get; set; }
    }
}
