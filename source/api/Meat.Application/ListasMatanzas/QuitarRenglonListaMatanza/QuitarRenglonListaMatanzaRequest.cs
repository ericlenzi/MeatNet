using MediatR;
using System;
using System.Text.Json.Serialization;

namespace Meat.Application.ListasMatanzas.QuitarRenglonListaMatanza
{
    public class QuitarRenglonListaMatanzaRequest : IRequest<QuitarRenglonListaMatanzaResponse>
    {
        public Guid Id { get; set; }                       // lista de matanza
        public Guid RenglonId { get; set; }

        [JsonIgnore]
        public string CodigoEmpresa { get; set; }
        [JsonIgnore]
        public Guid UsuarioId { get; set; }
    }

    public class QuitarRenglonListaMatanzaResponse
    {
    }
}
