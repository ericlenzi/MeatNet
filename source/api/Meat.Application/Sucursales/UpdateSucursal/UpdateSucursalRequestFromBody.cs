using System;
using System.ComponentModel.DataAnnotations;

namespace Meat.Application.Sucursales.UpdateSucursal
{
    public class UpdateSucursalRequestFromBody
    {
        [Required]
        public string Nombre { get; set; }
        public Guid EmpresaId { get; set; }
        public bool Activa { get; set; }
        public string Direccion { get; set; }
        public string CodigoPostal { get; set; }
        public string Localidad { get; set; }
        public string Provincia { get; set; }
        public string Zona { get; set; }
        public string Pais { get; set; }
        public string Erp_Codigo { get; set; }
        public string Color { get; set; }
    }
}
