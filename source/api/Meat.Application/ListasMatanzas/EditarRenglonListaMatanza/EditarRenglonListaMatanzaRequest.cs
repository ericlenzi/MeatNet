using MediatR;
using System;
using System.Text.Json.Serialization;

namespace Meat.Application.ListasMatanzas.EditarRenglonListaMatanza
{
    public class EditarRenglonListaMatanzaRequest : IRequest<EditarRenglonListaMatanzaResponse>
    {
        [JsonIgnore]
        public Guid Id { get; set; }                       // lista de matanza
        [JsonIgnore]
        public Guid RenglonId { get; set; }
        [JsonIgnore]
        public string CodigoEmpresa { get; set; }
        [JsonIgnore]
        public Guid UsuarioId { get; set; }

        public int Cantidad { get; set; }
        public int Secuencia { get; set; }
        public string Motivo { get; set; }
    }

    public class EditarRenglonListaMatanzaResponse
    {
    }
}
