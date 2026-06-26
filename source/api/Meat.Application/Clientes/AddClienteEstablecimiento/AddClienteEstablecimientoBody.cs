using System;

namespace Meat.Application.Clientes.AddClienteEstablecimiento
{
    public class AddClienteEstablecimientoBody
    {
        public Guid EstablecimientoId { get; set; }
        public string CodigoRenspa { get; set; }
        public string NumeroCUIG { get; set; }
    }
}
