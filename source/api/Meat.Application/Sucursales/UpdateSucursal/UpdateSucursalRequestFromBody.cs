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
    }
}
