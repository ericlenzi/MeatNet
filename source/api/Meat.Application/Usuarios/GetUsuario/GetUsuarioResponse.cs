using System;

namespace Meat.Application.Usuarios.GetUsuario
{
    public class GetUsuarioResponse
    {
        public Guid Id { get; set; }

        public string UserName { get; set; }

        public string Legajo { get; set; }

        public string Nombre { get; set; }

        public string Apellido { get; set; }

        public string Email { get; set; }

        public string RolId { get; set; }

        public bool Activo { get; set; }
    }
}