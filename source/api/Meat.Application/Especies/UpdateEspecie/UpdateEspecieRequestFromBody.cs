using System.ComponentModel.DataAnnotations;

namespace Meat.Application.Especies.UpdateEspecie
{
    public class UpdateEspecieRequestFromBody
    {
        [Required]
        public string Nombre { get; set; }
        public bool Activo { get; set; }
    }
}
