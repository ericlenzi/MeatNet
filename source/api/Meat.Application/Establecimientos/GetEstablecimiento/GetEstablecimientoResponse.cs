using System;
using System.Collections.Generic;

namespace Meat.Application.Establecimientos.GetEstablecimiento
{
    public class GetEstablecimientoResponse
    {
        public Guid Id { get; set; }
        public string CodigoEstablecimiento { get; set; }
        public string Nombre { get; set; }
        public Guid SucursalId { get; set; }
        public IEnumerable<EspecieItem> Especies { get; set; }
        public string NumeroSenasa { get; set; }
        public string NumeroRuca { get; set; }
        public bool Activo { get; set; }
    }
}
