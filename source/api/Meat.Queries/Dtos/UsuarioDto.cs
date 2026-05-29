using System;

namespace Meat.Queries.Dtos
{
    public class UsuarioDto
    {
        public Guid Id { get; set; }
        public string Legajo { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public int Rol { get; set; }
        public bool Responsable { get; set; }
        public int Estado { get; set; }
        public string UserName { get; set; }
        public Guid? EmpleadoId { get; set; }
        public Boolean EsSuperUsuario { get; set; }
    }
}
