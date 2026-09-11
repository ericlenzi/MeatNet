using System;

namespace Meat.Application.RendimientosSubproductos.GetRendimientoSubproducto
{
    public class GetRendimientoSubproductoResponse
    {
        public Guid Id { get; set; }
        public string EspecieId { get; set; }
        public string EspecieNombre { get; set; }
        public Guid MaterialId { get; set; }
        public string MaterialNombre { get; set; }
        public double Porcentaje { get; set; }
        public bool Activo { get; set; }
    }
}
