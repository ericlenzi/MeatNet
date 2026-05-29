using Meat.Domain.Sucursales;
using Meat.Queries.Dtos;
using Meat.Queries.Dtos.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meat.Queries.Informes
{
    public interface IVentasEmpleadosQueries
    {
        Task<VentaEmpleadoResponse> GetVentasEmpleadosAsync(DateTime fechaDesde, DateTime fechaHasta, Sucursal suc, int pageSize, int pageIndex);
    }
}
