using MediatR;
using System;
using System.Text.Json.Serialization;

namespace Meat.Application.IngresosHaciendas.GetIngresoHacienda
{
    public class GetIngresoHaciendaRequest : IRequest<GetIngresoHaciendaResponse>
    {
        public Guid Id { get; set; }

        [JsonIgnore]
        public string CodigoEmpresa { get; set; }
    }
}
