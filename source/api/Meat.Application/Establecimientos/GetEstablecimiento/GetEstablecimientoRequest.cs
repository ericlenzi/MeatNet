using MediatR;
using System;
using System.Text.Json.Serialization;

namespace Meat.Application.Establecimientos.GetEstablecimiento
{
    public class GetEstablecimientoRequest : IRequest<GetEstablecimientoResponse>
    {
        public Guid Id { get; set; }
        [JsonIgnore]
        public string CodigoEmpresa { get; set; }
    }
}
