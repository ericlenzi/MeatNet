using Meat.Application.Shared;
using System;
using System.Collections.Generic;

namespace Meat.Application.IngresosHaciendas.GetIngresosHaciendas
{
    public class GetIngresosHaciendasResponse : ResponseListBase<IEnumerable<IngresoHaciendaListItem>>
    {
    }

    public class IngresoHaciendaListItem
    {
        public Guid Id { get; set; }
        public long NumeroIngreso { get; set; }
        public DateTime FechaHoraIngreso { get; set; }
        public string NumeroDte { get; set; }
        public Guid EstablecimientoId { get; set; }
        public string EstablecimientoNombre { get; set; }
        public string ClienteNombre { get; set; }
        public string EstadoIngresoId { get; set; }
        public string EstadoIngresoNombre { get; set; }
        public int TotalCabezas { get; set; }
        public double PesoNeto { get; set; }
    }
}
