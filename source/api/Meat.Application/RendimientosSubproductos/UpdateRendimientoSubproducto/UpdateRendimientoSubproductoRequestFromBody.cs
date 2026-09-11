using System;
using System.ComponentModel.DataAnnotations;

namespace Meat.Application.RendimientosSubproductos.UpdateRendimientoSubproducto
{
    public class UpdateRendimientoSubproductoRequestFromBody
    {
        [Required]
        public string EspecieId { get; set; }
        [Required]
        public Guid MaterialId { get; set; }
        public double Porcentaje { get; set; }
        public bool Activo { get; set; }
    }
}
