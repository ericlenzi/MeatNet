using MediatR;
using System;
using System.Text.Json.Serialization;

namespace Meat.Application.Empresas.GetEmpresa
{
    public class GetEmpresaRequest : IRequest<GetEmpresaResponse>
    {
        public Guid Id { get; set; }
        [JsonIgnore]
        public string CodigoEmpresa { get; set; }
    }
}
