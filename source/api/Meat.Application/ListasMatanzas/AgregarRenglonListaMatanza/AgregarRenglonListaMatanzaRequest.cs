using Meat.Application.Shared;
using MediatR;
using System;
using System.Text.Json.Serialization;

namespace Meat.Application.ListasMatanzas.AgregarRenglonListaMatanza
{
    public class AgregarRenglonListaMatanzaRequest : RequestBase, IRequest<AgregarRenglonListaMatanzaResponse>
    {
        [JsonIgnore]
        public Guid Id { get; set; }                       // lista de matanza

        public Guid TropaId { get; set; }
        public Guid AlmacenId { get; set; }
        public Guid? AlmacenDestinoId { get; set; }        // camara de faena (requerido: LM ya confirmada)
        public string TipoEspecieId { get; set; }
        public int Cantidad { get; set; }
        public int? Secuencia { get; set; }                // null = al final
        public string Motivo { get; set; }
    }

    public class AgregarRenglonListaMatanzaResponse
    {
        public Guid RenglonId { get; set; }
    }
}
