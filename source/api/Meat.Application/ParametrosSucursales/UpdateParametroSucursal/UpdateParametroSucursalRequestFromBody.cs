using System;
using System.ComponentModel.DataAnnotations;

namespace Meat.Application.ParametrosSucursales.UpdateParametroSucursal
{
    public class UpdateParametroSucursalRequestFromBody
    {
        [Required]
        public Guid SucursalId { get; set; }

        [Required]
        public string Valor { get; set; }
    }
}
