using Dapper;
using Meat.Domain.Sucursales;
using Meat.Queries.Dtos;
using Meat.Queries.Dtos.Response;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Meat.Queries.ArticulosDuplicados
{
    public class ArticulosDuplicadosQueries : IArticulosDuplicadosQueries
    {
        private readonly IDbConnection dbConnection;

        public ArticulosDuplicadosQueries(IDbConnection dbConnection)
        {
            this.dbConnection = dbConnection;
        }

        public async Task<ArticuloDuplicadoResponse> GetArticulosDuplicados()
        {
        
            var query = @"
                 SELECT codigo, SucursalId, COUNT(*) AS Cantidad
        FROM articulos WITH (NOLOCK)
        GROUP BY codigo, SucursalId
        HAVING COUNT(*) >= 2";

            var articulosBD = await dbConnection.QueryAsync<ArticuloDuplicadoDto>(query);

            return new ArticuloDuplicadoResponse
            {
               Articulos = articulosBD
            };

        }
    }
}
