using MediatR;
using System;

namespace Meat.Application.Usuarios.RemoveUsuarioEstablecimiento
{
    public class RemoveUsuarioEstablecimientoRequest : IRequest<RemoveUsuarioEstablecimientoResponse>
    {
        public Guid UsuarioEstablecimientoId { get; set; }
    }
}
