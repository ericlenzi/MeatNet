using MediatR;
using System;

namespace Meat.Application.Usuarios.RemoveUsuarioSucursal
{
    public class RemoveUsuarioSucursalRequest : IRequest<RemoveUsuarioSucursalResponse>
    {
        public Guid UsuarioSucursalId { get; set; }
    }
}
