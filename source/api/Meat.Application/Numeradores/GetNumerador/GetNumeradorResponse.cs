using System;

namespace Meat.Application.Numeradores.GetNumerador
{
    public class GetNumeradorResponse
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
