using MediatR;
using System;
using System.Text.Json.Serialization;

namespace Meat.Application.Romaneos.AnularRomaneo
{
    public class AnularRomaneoRequest : IRequest<AnularRomaneoResponse>
    {
        [JsonIgnore]
        public Guid Id { get; set; }
        [JsonIgnore]
        public string CodigoEmpresa { get; set; }
        [JsonIgnore]
        public Guid UsuarioId { get; set; }

        public string Motivo { get; set; }
    }

    public class AnularRomaneoResponse
    {
        public Guid Id { get; set; }
    }
}
