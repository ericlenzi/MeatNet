using MediatR;
using System;

namespace Meat.Application.Clientes.AddClienteEstablecimiento
{
    public class AddClienteEstablecimientoRequest : IRequest<AddClienteEstablecimientoResponse>
    {
        public Guid ClienteId { get; set; }
        public Guid EstablecimientoId { get; set; }
        public string CodigoRenspa { get; set; }
        public string NumeroCUIG { get; set; }
    }
}
