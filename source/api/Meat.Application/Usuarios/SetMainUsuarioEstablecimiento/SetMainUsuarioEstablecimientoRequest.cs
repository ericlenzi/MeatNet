using MediatR;
using System;

namespace Meat.Application.Usuarios.SetMainUsuarioEstablecimiento
{
    public class SetMainUsuarioEstablecimientoRequest : IRequest<SetMainUsuarioEstablecimientoResponse>
    {
        public Guid UsuarioId { get; set; }
        public Guid UsuarioEstablecimientoId { get; set; }
    }
}
