using MediatR;
using System;
using System.Text.Json.Serialization;

namespace Meat.Application.ListasMatanzas.DeleteListaMatanza
{
    public class DeleteListaMatanzaRequest : IRequest<DeleteListaMatanzaResponse>
    {
        public Guid Id { get; set; }

        [JsonIgnore]
        public string EmpresaId { get; set; }
    }

    public class DeleteListaMatanzaResponse
    {
    }
}
