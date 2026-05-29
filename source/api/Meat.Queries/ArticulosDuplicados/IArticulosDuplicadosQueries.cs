using Meat.Domain.Sucursales;
using Meat.Queries.Dtos;
using Meat.Queries.Dtos.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meat.Queries.ArticulosDuplicados
{
    public interface IArticulosDuplicadosQueries
    {
        Task<ArticuloDuplicadoResponse> GetArticulosDuplicados();
    }
}
