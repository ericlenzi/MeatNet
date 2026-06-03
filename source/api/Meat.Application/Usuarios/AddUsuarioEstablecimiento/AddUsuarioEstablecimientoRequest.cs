using MediatR;
using System;

namespace Meat.Application.Usuarios.AddUsuarioEstablecimiento
{
    public class AddUsuarioEstablecimientoRequest : IRequest<AddUsuarioEstablecimientoResponse>
    {
        public Guid UsuarioId { get; set; }
        public Guid EstablecimientoId { get; set; }
        public bool EsMain { get; set; }
    }
}
