using Meat.Application.Shared;
using MediatR;
using System;
using System.Text.Json.Serialization;

namespace Meat.Application.ListasMatanzas.EditarRenglonListaMatanza
{
    public class EditarRenglonListaMatanzaRequest : RequestBase, IRequest<EditarRenglonListaMatanzaResponse>
    {
        [JsonIgnore]
        public Guid Id { get; set; }                       // lista de matanza
        [JsonIgnore]
        public Guid RenglonId { get; set; }

        public int Cantidad { get; set; }
        public int Secuencia { get; set; }
        public string Motivo { get; set; }
    }

    public class EditarRenglonListaMatanzaResponse
    {
    }
}
