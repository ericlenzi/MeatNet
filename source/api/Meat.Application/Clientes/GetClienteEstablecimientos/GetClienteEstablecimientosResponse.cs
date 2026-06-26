using System;
using System.Collections.Generic;

namespace Meat.Application.Clientes.GetClienteEstablecimientos
{
    public class GetClienteEstablecimientosResponse
    {
        public IEnumerable<ClienteEstablecimientoItem> Data { get; set; }
    }

    public class ClienteEstablecimientoItem
    {
        public Guid Id { get; set; }
        public Guid EstablecimientoId { get; set; }
        public string CodigoEstablecimiento { get; set; }
        public string Nombre { get; set; }
        public Guid SucursalId { get; set; }
        public string CodigoSucursal { get; set; }
        public string NombreSucursal { get; set; }
        public string CodigoRenspa { get; set; }
        public string NumeroCUIG { get; set; }
    }
}
