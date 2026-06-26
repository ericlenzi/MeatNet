using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Meat.Application.Establecimientos.UpdateEstablecimiento
{
    public class UpdateEstablecimientoRequestFromBody
    {
        [Required]
        public string Nombre { get; set; }
        public Guid SucursalId { get; set; }
        public IEnumerable<string> EspecieIds { get; set; }
        public string NumeroSenasa { get; set; }
        public string NumeroRuca { get; set; }
        public bool Activo { get; set; }
    }
}
