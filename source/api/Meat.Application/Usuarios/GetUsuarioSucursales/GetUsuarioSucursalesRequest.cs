using Meat.Application.Shared;
using MediatR;
using System;

namespace Meat.Application.Usuarios.GetUsuarioSucursales
{
    public class GetUsuarioSucursalesRequest : RequestBase, IRequest<GetUsuarioSucursalesResponse>
    {
        public Guid Id { get; set; }
    }
}
