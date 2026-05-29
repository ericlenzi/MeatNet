using System.ComponentModel.DataAnnotations;

namespace Meat.Application.Roles.UpdateRol
{
    public class UpdateRolRequestFromBody
    {
        [Required]
        public string Nombre { get; set; }

        [Required]
        public bool Activo { get; set; }
    }
}
