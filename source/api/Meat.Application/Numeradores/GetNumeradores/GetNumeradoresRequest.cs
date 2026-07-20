using MediatR;
using System;
using System.Text.Json.Serialization;

namespace Meat.Application.Numeradores.GetNumeradores
{
    public class GetNumeradoresRequest : IRequest<GetNumeradoresResponse>
    {
        [JsonIgnore]
        public string CodigoEmpresa { get; set; }

        public Guid? EstablecimientoId { get; set; }
        public bool? Estado { get; set; }
    }
}
