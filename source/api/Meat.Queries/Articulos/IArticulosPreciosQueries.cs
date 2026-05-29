using Meat.Queries.Dtos.Response;
using System;
using System.Threading.Tasks;

namespace Meat.Queries.Articulos
{
    public interface IArticulosPreciosQueries
    {
        Task<ArticuloPrecioResponse> UpdateArticuloPreciosAsync(string codigo, double Precio, double PrecioJLQ, double PrecioPRV, double PrecioVTA, string nroSucursal, double TasaIvaFiscal, double TasaIvaFiscalValor, string fechaProceso);
        Task<bool> Exist(Guid idSucursal, string codigo);
    }
}
