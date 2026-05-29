using Meat.Queries.Dtos;
using Meat.Queries.Dtos.Response;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meat.Queries.Articulos
{
    public interface IArticulosMarcasPorSucursalQueries
    {
        Task<ArticuloMarcasResponse> GetArticulosMarcasPorSucursal(Guid sucursalId);
    }
}
