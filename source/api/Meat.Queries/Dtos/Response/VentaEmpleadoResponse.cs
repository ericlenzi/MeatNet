using System;
using System.Collections.Generic;
using System.Text;

namespace Meat.Queries.Dtos.Response
{
    public class VentaEmpleadoResponse
    {
        public int TotalRows { get; set; }
        public IEnumerable<VentaEmpleadoDto> VentasEmpleados { get; set; }
    }
}
