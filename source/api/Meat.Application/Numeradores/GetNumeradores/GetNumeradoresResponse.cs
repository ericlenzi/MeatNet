using System;
using System.Collections.Generic;

namespace Meat.Application.Numeradores.GetNumeradores
{
    public class GetNumeradoresResponse
    {
        public IEnumerable<NumeradorItem> Data { get; set; } = new List<NumeradorItem>();
    }

    public class NumeradorItem
    {
        public Guid Id { get; set; }
        public Guid EstablecimientoId { get; set; }
        public string EstablecimientoNombre { get; set; }
        public string EspecieCodigo { get; set; }
        public string EspecieNombre { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public string TipoNumerador { get; set; }
        public int UltimoNumero { get; set; }
        public bool Activo { get; set; }
    }
}
