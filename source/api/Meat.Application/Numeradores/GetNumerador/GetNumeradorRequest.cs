using MediatR;
using System;
using System.Text.Json.Serialization;

namespace Meat.Application.Numeradores.GetNumerador
{
    public class GetNumeradorRequest : IRequest<GetNumeradorResponse>
    {
        public Guid Id { get; set; }

        [JsonIgnore]
        public string CodigoEmpresa { get; set; }
    }
}
