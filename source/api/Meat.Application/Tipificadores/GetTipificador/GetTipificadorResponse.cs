using System;

namespace Meat.Application.Tipificadores.GetTipificador
{
    public class GetTipificadorResponse
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public string Matricula { get; set; }
        public Guid EstablecimientoId { get; set; }
        public string EstablecimientoNombre { get; set; }
        public string EspecieId { get; set; }
        public string EspecieNombre { get; set; }
        public bool PorDefecto { get; set; }
        public bool Activo { get; set; }
    }
}
