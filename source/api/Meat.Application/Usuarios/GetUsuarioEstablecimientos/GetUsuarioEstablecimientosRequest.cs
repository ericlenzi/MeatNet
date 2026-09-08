using Meat.Application.Shared;
using MediatR;
using System;

namespace Meat.Application.Usuarios.GetUsuarioEstablecimientos
{
    public class GetUsuarioEstablecimientosRequest : RequestBase, IRequest<GetUsuarioEstablecimientosResponse>
    {
        public Guid Id { get; set; }
    }
}
