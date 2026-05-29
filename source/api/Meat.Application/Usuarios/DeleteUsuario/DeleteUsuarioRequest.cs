using MediatR;
using System;

namespace Meat.Application.Usuarios.DeleteUsuario
{
    public class DeleteUsuarioRequest : IRequest<DeleteUsuarioResponse>
    {
        public Guid Id { get; set; }
    }
}
