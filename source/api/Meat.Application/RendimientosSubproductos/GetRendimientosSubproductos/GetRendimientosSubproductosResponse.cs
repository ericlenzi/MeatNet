using Meat.Application.Shared;
using System;
using System.Collections.Generic;

namespace Meat.Application.RendimientosSubproductos.GetRendimientosSubproductos
{
    public class GetRendimientosSubproductosResponse : ResponseListBase<IEnumerable<RendimientoSubproductoItem>>
    {
    }

    public class RendimientoSubproductoItem
    {
        public Guid Id { get; set; }
        public string EspecieId { get; set; }
        public string EspecieNombre { get; set; }
        public Guid MaterialId { get; set; }
        public string MaterialCodigo { get; set; }
        public string MaterialNombre { get; set; }
        public double Porcentaje { get; set; }
        public bool Activo { get; set; }
    }
}
