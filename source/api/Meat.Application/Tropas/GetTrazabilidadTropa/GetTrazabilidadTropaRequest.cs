using MediatR;
using System;
using System.Text.Json.Serialization;

namespace Meat.Application.Tropas.GetTrazabilidadTropa
{
    public class GetTrazabilidadTropaRequest : IRequest<GetTrazabilidadTropaResponse>
    {
        [JsonIgnore]
        public string CodigoEmpresa { get; set; }

        public long NumeroTropa { get; set; }

        // Opcional: acota la busqueda al establecimiento activo
        public Guid? EstablecimientoId { get; set; }
    }
}
