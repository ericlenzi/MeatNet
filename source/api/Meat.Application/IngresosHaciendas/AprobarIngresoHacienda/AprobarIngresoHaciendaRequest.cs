using MediatR;
using System;
using System.Text.Json.Serialization;

namespace Meat.Application.IngresosHaciendas.AprobarIngresoHacienda
{
    public class AprobarIngresoHaciendaRequest : IRequest<AprobarIngresoHaciendaResponse>
    {
        public Guid Id { get; set; }

        [JsonIgnore]
        public string CodigoEmpresa { get; set; }

        [JsonIgnore]
        public Guid UsuarioId { get; set; }
    }
}
