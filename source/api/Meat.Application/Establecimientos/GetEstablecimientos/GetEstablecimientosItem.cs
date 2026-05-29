using System;

namespace Meat.Application.Establecimientos.GetEstablecimientos
{
    public class GetEstablecimientosItem
    {
        public Guid Id { get; set; }
        public string CodigoEstablecimiento { get; set; }
        public string Nombre { get; set; }
        public string EspecieId { get; set; }
        public bool Activo { get; set; }
    }
}