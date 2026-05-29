using MediatR;
using System;

namespace Meat.Application.Usuarios.UpdateUsuario
{
    public class UpdateUsuarioRequest : IRequest<UpdateUsuarioResponse>
    {
        public Guid Id { get; set; }

        public string UserName { get; set; }

        public string Nombre { get; set; }

        public string Apellido { get; set; }

        public string Email { get; set; }

        public string Legajo { get; set; }

        public string RolId { get; set; }

        public Guid EmpresaId { get; set; }

        public bool Activo { get; set; }
    }
}