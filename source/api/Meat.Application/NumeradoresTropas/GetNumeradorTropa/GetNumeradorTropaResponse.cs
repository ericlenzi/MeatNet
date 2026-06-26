using System;

namespace Meat.Application.NumeradoresTropas.GetNumeradorTropa
{
    public class GetNumeradorTropaResponse
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
