using MediatR;
using System;
using System.Text.Json.Serialization;

namespace Meat.Application.ListasMatanzas.DeleteListaMatanza
{
    public class DeleteListaMatanzaRequest : IRequest<DeleteListaMatanzaResponse>
    {
        public Guid Id { get; set; }

        [JsonIgnore]
        public string CodigoEmpresa { get; set; }
    }

    public class DeleteListaMatanzaResponse
    {
    }
}
