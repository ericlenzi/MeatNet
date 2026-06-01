using MediatR;
using System;

namespace Meat.Application.Usuarios.GetUsuarioSucursales
{
    public class GetUsuarioSucursalesRequest : IRequest<GetUsuarioSucursalesResponse>
    {
        public Guid UsuarioId { get; set; }
    }
}
