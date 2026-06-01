using System;

namespace Meat.Application.Sucursales.GetSucursal
{

    public class GetSucursalResponse
    {
        public Guid Id { get; set; }
        public string CodigoSucursal { get; set; }
        public string Nombre { get; set; }
        public string Erp_Codigo { get; set; }
        public Guid EmpresaId { get; set; }
        public bool Activo { get; set; }
        public string Direccion { get; set; }
        public string CodigoPostal { get; set; }
        public string Localidad { get; set; }
        public string Provincia { get; set; }
        public string Zona { get; set; }
        public string Pais { get; set; }
        public string Color { get; set; }
    }
}
