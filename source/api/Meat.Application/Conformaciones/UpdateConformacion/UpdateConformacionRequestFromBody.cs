using System.ComponentModel.DataAnnotations;

namespace Meat.Application.Conformaciones.UpdateConformacion
{
    public class UpdateConformacionRequestFromBody
    {
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string EspecieId { get; set; }
        public int Orden { get; set; }
        public bool Activo { get; set; }
    }
}
