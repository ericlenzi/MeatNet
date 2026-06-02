using MediatR;
using System;
using System.Text.Json.Serialization;

namespace Meat.Application.Empresas.DeleteEmpresa
{
    public class DeleteEmpresaRequest : IRequest<DeleteEmpresaResponse>
    {
        public Guid Id { get; set; }
        [JsonIgnore]
        public string CodigoEmpresaActiva { get; set; }
    }
}
