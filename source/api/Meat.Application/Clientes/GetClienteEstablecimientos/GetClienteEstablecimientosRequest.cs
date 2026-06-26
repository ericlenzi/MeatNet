using MediatR;
using System;

namespace Meat.Application.Clientes.GetClienteEstablecimientos
{
    public class GetClienteEstablecimientosRequest : IRequest<GetClienteEstablecimientosResponse>
    {
        public Guid ClienteId { get; set; }
    }
}
