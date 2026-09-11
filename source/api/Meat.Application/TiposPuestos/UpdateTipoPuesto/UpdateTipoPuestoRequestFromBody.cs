using System.ComponentModel.DataAnnotations;

namespace Meat.Application.TiposPuestos.UpdateTipoPuesto
{
    public class UpdateTipoPuestoRequestFromBody
    {
        [Required]
        public string Nombre { get; set; }
        public bool Activo { get; set; }
    }
}
