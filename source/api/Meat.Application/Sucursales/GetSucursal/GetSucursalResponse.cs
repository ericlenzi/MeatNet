using System;

namespace Meat.Application.Sucursales.GetSucursal
{

    public class GetSucursalResponse
    {
        public Guid Id { get; set; }
        public string NumeroSucursal { get; set; }
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string Erp_Codigo { get; set; }
        public string Zona { get; set; }
        public string ZonaEstadistica { get; set; }
        public bool Activo { get; set; }
    }
}
