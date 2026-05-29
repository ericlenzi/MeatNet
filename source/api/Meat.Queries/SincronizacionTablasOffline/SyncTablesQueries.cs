using Dapper;
using Meat.Queries.SincronizacionTablasOffline.Dto;
using Meat.Queries.SincronizacionTablasOffline.Response;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Meat.Queries.SincronizacionTablasOffline
{
    public class SyncTablesQueries : ISyncTablesQueries
    {
        private readonly IDbConnection dbConnection;

        public SyncTablesQueries(IDbConnection dbConnection)
        {
            this.dbConnection = dbConnection;
        }

        public async Task<GiftcardsResponse> GetGiftcards(DateTime fechaActualizacion)
        {
        
            var query = @"
                SELECT *
                FROM Giftcards a WITH (NOLOCK)
                WHERE
                    FechaActualizacion > @fechaActualizacion";

            var parameters = new
            {
                fechaActualizacion
            };

            var queryResult = await dbConnection.QueryAsync<GiftcardDto>(query, parameters);

            return new GiftcardsResponse
            {
                Giftcards = queryResult
            };
        }
    }
}
