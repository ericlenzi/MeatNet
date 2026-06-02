using System.Collections.Generic;
using Meat.Domain.TiposClientes;

namespace Meat.Application.TiposClientes.GetTiposClientes
{
    public class GetTiposClientesResponse
    {
        public IEnumerable<TipoCliente> Data { get; set; }
    }
}
