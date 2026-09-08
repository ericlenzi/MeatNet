using MediatR;
using System;
using System.Text.Json.Serialization;

namespace Meat.Application.IngresosHaciendas.RechazarIngresoHacienda
{
    public class RechazarIngresoHaciendaRequest : IRequest<RechazarIngresoHaciendaResponse>
    {
        public Guid Id { get; set; }

        [JsonIgnore]
        public string EmpresaId { get; set; }
    }

    public class RechazarIngresoHaciendaResponse
    {
    }
}
