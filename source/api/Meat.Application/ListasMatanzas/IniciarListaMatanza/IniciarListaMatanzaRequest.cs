using MediatR;
using System;
using System.Text.Json.Serialization;

namespace Meat.Application.ListasMatanzas.IniciarListaMatanza
{
    public class IniciarListaMatanzaRequest : IRequest<IniciarListaMatanzaResponse>
    {
        public Guid Id { get; set; }

        [JsonIgnore]
        public string EmpresaId { get; set; }
        [JsonIgnore]
        public Guid UsuarioId { get; set; }
    }

    public class IniciarListaMatanzaResponse
    {
    }
}
