using MediatR;
using System;
using System.Text.Json.Serialization;

namespace Meat.Application.IngresosHaciendas.EnviarAprobacionIngresoHacienda
{
    public class EnviarAprobacionIngresoHaciendaRequest : IRequest<EnviarAprobacionIngresoHaciendaResponse>
    {
        public Guid Id { get; set; }

        [JsonIgnore]
        public string CodigoEmpresa { get; set; }
    }

    public class EnviarAprobacionIngresoHaciendaResponse
    {
    }
}
