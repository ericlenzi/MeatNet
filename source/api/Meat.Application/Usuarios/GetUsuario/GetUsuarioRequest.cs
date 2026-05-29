using MediatR;
using System;

namespace Meat.Application.Usuarios.GetUsuario
{


    public class GetUsuarioRequest : IRequest<GetUsuarioResponse>
    {
        public Guid Id { get; set; }
    }
}
