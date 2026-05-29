using System;
using System.ComponentModel.DataAnnotations;

namespace Meat.Application.Usuarios.UpdateUsuario
{
    public class UpdateUsuarioRequestFromBody
    {
        public string UserName { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Apellido { get; set; }

        public string Email { get; set; }

        public string Legajo { get; set; }

        [Required]
        public string RolId { get; set; }

        public Guid EmpresaId { get; set; }

        [Required]
        public bool Activo { get; set; }
    }
}