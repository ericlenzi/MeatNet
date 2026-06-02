using MediatR;
using System;
using System.Text.Json.Serialization;

namespace Meat.Application.Establecimientos.DeleteEstablecimiento
{
    public class DeleteEstablecimientoRequest : IRequest<DeleteEstablecimientoResponse>
    {
        public Guid Id { get; set; }
        [JsonIgnore]
        public string CodigoEmpresa { get; set; }
    }
}
