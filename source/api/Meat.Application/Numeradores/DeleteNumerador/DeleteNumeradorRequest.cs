using MediatR;
using System;
using System.Text.Json.Serialization;

namespace Meat.Application.Numeradores.DeleteNumerador
{
    public class DeleteNumeradorRequest : IRequest<DeleteNumeradorResponse>
    {
        public Guid Id { get; set; }

        [JsonIgnore]
        public string CodigoEmpresa { get; set; }
    }
}
