using System;
using System.ComponentModel.DataAnnotations;

namespace Meat.Application.Puestos.UpdatePuesto
{
    public class UpdatePuestoRequestFromBody
    {
        [Required]
        public string Nombre { get; set; }
        [Required]
        public Guid EstablecimientoId { get; set; }
        [Required]
        public string EspecieId { get; set; }
        [Required]
        public string TipoPuestoId { get; set; }
        [Required]
        public string TipoMedicionId { get; set; }
        public bool Activo { get; set; }
    }
}
