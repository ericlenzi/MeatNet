using MediatR;
using System;

namespace Meat.Application.Usuarios.SetMainUsuarioSucursal
{
    public class SetMainUsuarioSucursalRequest : IRequest<SetMainUsuarioSucursalResponse>
    {
        public Guid UsuarioId { get; set; }
        public Guid UsuarioSucursalId { get; set; }
    }
}
