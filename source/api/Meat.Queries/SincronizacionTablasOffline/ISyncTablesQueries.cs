using Meat.Queries.Dtos;
using Meat.Queries.Dtos.Response;
using Meat.Queries.SincronizacionTablasOffline.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meat.Queries.SincronizacionTablasOffline
{
    public interface ISyncTablesQueries
    {
        Task<GiftcardsResponse> GetGiftcards(DateTime fechaActualizacion);
    }
}
