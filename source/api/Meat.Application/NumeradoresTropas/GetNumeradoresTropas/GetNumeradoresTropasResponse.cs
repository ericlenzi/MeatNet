using System;
using System.Collections.Generic;

namespace Meat.Application.NumeradoresTropas.GetNumeradoresTropas
{
    public class GetNumeradoresTropasResponse
    {
        public IEnumerable<NumeradorTropaItem> Data { get; set; }
    }

    public class NumeradorTropaItem
    {
        public Guid Id { get; set; }
        public Guid ClienteEstablecimientoId { get; set; }
        public string CodigoCliente { get; set; }
        public string NombreCliente { get; set; }
        public string CodigoEstablecimiento { get; set; }
        public string NombreEstablecimiento { get; set; }
        public string EspecieCodigo { get; set; }
        public string EspecieNombre { get; set; }
        public long UltimoNumeroTropa { get; set; }
    }
}
