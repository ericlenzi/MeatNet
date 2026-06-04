using MediatR;
using System.ComponentModel.DataAnnotations;
using System;
using System.Text.Json.Serialization;

namespace Meat.Application.Usuarios.CreateUsuario
{
    public class CreateUsuarioRequest : IRequest<CreateUsuarioResponse>
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Apellido { get; set; }

        public string Email { get; set; }

        public string Legajo { get; set; }

        [Required]
        public string RolId { get; set; }

        [Required]
        public bool Activo { get; set; }
    }
}