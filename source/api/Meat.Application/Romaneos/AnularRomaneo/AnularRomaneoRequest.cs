using Meat.Application.Shared;
using MediatR;
using System;
using System.Text.Json.Serialization;

namespace Meat.Application.Romaneos.AnularRomaneo
{
    public class AnularRomaneoRequest : RequestBase, IRequest<AnularRomaneoResponse>
    {
        [JsonIgnore]
        public Guid Id { get; set; }

        public string Motivo { get; set; }
    }

    public class AnularRomaneoResponse
    {
        public Guid Id { get; set; }
    }
}
