using MediatR;
using System;

namespace Meat.Application.Clientes.RemoveClienteEstablecimiento
{
    public class RemoveClienteEstablecimientoRequest : IRequest<RemoveClienteEstablecimientoResponse>
    {
        public Guid ClienteEstablecimientoId { get; set; }
    }
}
