using System;

namespace Meat.Application.Puestos.GetPuesto
{
    public class GetPuestoResponse
    {
        public Guid Id { get; set; }
        public string CodigoPuesto { get; set; }
        public string Nombre { get; set; }
        public Guid EstablecimientoId { get; set; }
        public string EstablecimientoNombre { get; set; }
        public string EspecieId { get; set; }
        public string EspecieNombre { get; set; }
        public string TipoPuestoId { get; set; }
        public string TipoMedicionId { get; set; }
        public bool Activo { get; set; }
    }
}
