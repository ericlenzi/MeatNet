using System;

namespace Meat.Application.Sucursales.GetSucursalByCodigo
{
    public class GetSucursalByCodigoResponse
    {
        public Guid Id { get; set; }
        public string CodigoSucursal { get; set; }
        public string CodigoEmpresa { get; set; }
        public string Erp_Codigo { get; set; }
        public string Nombre { get; set; }
        public bool Activo { get; set; }
    }
}
