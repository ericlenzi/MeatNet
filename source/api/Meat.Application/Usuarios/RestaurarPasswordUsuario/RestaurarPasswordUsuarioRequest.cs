using Meat.Application.Shared;
using MediatR;
using System;

namespace Meat.Application.Usuarios.RestaurarPasswordUsuario
{
    public class RestaurarPasswordUsuarioRequest : RequestBase, IRequest<RestaurarPasswordUsuarioResponse>
    {
        public Guid Id { get; set; }
    }
}
