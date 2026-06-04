using MediatR;
using System;
using System.Text.Json.Serialization;

namespace Meat.Application.Usuarios.DeleteUsuario
{
    public class DeleteUsuarioRequest : IRequest<DeleteUsuarioResponse>
    {
        public Guid Id { get; set; }
    }
}
