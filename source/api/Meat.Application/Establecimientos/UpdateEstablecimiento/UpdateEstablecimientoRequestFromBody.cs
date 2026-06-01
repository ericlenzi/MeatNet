using System;
using System.ComponentModel.DataAnnotations;

namespace Meat.Application.Establecimientos.UpdateEstablecimiento
{
    public class UpdateEstablecimientoRequestFromBody
    {
        [Required]
        public string Nombre { get; set; }
        public Guid SucursalId { get; set; }
        public string EspecieId { get; set; }
        public string NumeroSenasa { get; set; }
        public string NumeroOncca { get; set; }
        public bool Activo { get; set; }
    }
}
