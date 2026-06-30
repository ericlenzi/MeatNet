using MediatR;
using System;
using System.Text.Json.Serialization;

namespace Meat.Application.Tropas.GetTropasDisponibles
{
    public class GetTropasDisponiblesRequest : IRequest<GetTropasDisponiblesResponse>
    {
        [JsonIgnore]
        public string CodigoEmpresa { get; set; }

        public Guid? EstablecimientoId { get; set; }
    }
}
