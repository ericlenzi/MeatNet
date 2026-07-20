using System;
using System.ComponentModel.DataAnnotations;

namespace Meat.Application.Puestos.UpdatePuesto
{
    public class UpdatePuestoRequestFromBody
    {
        [Required]
        public string NumeroPuesto { get; set; }

        [Required]
        public Guid EstablecimientoId { get; set; }

        public string Erp_Codigo { get; set; }
    }
}
