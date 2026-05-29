using System;

namespace Meat.Queries.Dtos
{
    public class VentaEmpleadoDto
    {
        public Guid Id { get; set; }

        public DateTime Fecha { get; set; }

        public double MontoTotal { get; set; }

        public string NombreCliente { get; set; }
        public string CodigoSAPCliente { get; set; }
        public string NroDocCliente { get; set; }

        public string NumeroSucursal { get; set; }
        public string UserName { get; set; }
        public string LegajoEmpleado { get; set; }
        public string TipoComprobante { get; set; }
        public string NumeroComprobanteCompleto { get; set; }
        public int TipoPuntoVenta { get; set; }
    }
}
