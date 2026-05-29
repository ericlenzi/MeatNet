using System;

namespace Meat.Queries.Dtos
{
    public class ArticuloPrecioDto
    {
        public string Codigo { get; set; }
        public double Precio { get; set; }
        public double PrecioPRV { get; set; }
        public double PrecioJLQ { get; set; }
        public double PrecioVTA { get; set; }
        public string NumeroSucursal { get; set; }
    }
}
