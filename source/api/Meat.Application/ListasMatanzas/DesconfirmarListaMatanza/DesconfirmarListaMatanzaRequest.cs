using MediatR;
using System;
using System.Text.Json.Serialization;

namespace Meat.Application.ListasMatanzas.DesconfirmarListaMatanza
{
    public class DesconfirmarListaMatanzaRequest : IRequest<DesconfirmarListaMatanzaResponse>
    {
        public Guid Id { get; set; }

        [JsonIgnore]
        public string CodigoEmpresa { get; set; }
        [JsonIgnore]
        public Guid UsuarioId { get; set; }
    }

    public class DesconfirmarListaMatanzaResponse
    {
    }
}
