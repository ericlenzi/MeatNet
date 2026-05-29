using Dapper;
using Meat.Queries.Dtos;
using Meat.Queries.Dtos.Response;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Meat.Queries.Articulos
{
    public class ArticulosMarcasPorSucursalQueries : IArticulosMarcasPorSucursalQueries
    {
        private readonly IDbConnection dbConnection;

        public ArticulosMarcasPorSucursalQueries(IDbConnection dbConnection)
        {
            this.dbConnection = dbConnection;
        }

        public async Task<ArticuloMarcasResponse> GetArticulosMarcasPorSucursal(Guid sucursalId)
        {
        
            var query = @"
                SELECT DISTINCT ArticulosMarcas.Id, ArticulosMarcas.Nombre, ArticulosMarcas.SAP_Codigo
                FROM ArticulosMarcas WITH (NOLOCK)
                INNER JOIN Articulos ON Articulos.ArticuloMarcaId = ArticulosMarcas.Id
                where SucursalId =  @sucursalId
                ORDER BY ArticulosMarcas.Nombre ";

            var parameters = new
            {
                sucursalId
            };

            var queryResult = await dbConnection.QueryAsync<ArticulosMarcasDto>(query, parameters);

            return new ArticuloMarcasResponse
            {
               Marcas = queryResult
            };

        }
    }
}
