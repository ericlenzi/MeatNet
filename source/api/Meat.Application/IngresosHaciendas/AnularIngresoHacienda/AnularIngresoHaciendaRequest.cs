using MediatR;
using System;
using System.Text.Json.Serialization;

namespace Meat.Application.IngresosHaciendas.AnularIngresoHacienda
{
    public class AnularIngresoHaciendaRequest : IRequest<AnularIngresoHaciendaResponse>
    {
        public Guid Id { get; set; }

        [JsonIgnore]
        public string CodigoEmpresa { get; set; }
    }

    public class AnularIngresoHaciendaResponse
    {
    }
}
