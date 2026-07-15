using MediatR;
using System;
using System.Text.Json.Serialization;

namespace Meat.Application.ListasMatanzas.CancelarListaMatanza
{
    public class CancelarListaMatanzaRequest : IRequest<CancelarListaMatanzaResponse>
    {
        public Guid Id { get; set; }
        public string Motivo { get; set; }

        [JsonIgnore]
        public string CodigoEmpresa { get; set; }
        [JsonIgnore]
        public Guid UsuarioId { get; set; }
    }

    public class CancelarListaMatanzaResponse
    {
    }
}
