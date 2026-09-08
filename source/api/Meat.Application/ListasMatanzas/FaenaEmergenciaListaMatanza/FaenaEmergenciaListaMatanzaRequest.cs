using Meat.Application.Shared;
using MediatR;
using System;
using System.Text.Json.Serialization;

namespace Meat.Application.ListasMatanzas.FaenaEmergenciaListaMatanza
{
    public class FaenaEmergenciaListaMatanzaRequest : RequestBase, IRequest<FaenaEmergenciaListaMatanzaResponse>
    {
        [JsonIgnore]
        public Guid Id { get; set; }                       // lista de matanza

        public Guid TropaId { get; set; }
        public Guid AlmacenId { get; set; }
        public Guid? AlmacenDestinoId { get; set; }        // camara de faena (requerido: LM en ejecucion)
        public Guid? TipoEspecieId { get; set; }
        public int Cantidad { get; set; }
        public string Motivo { get; set; }
    }

    public class FaenaEmergenciaListaMatanzaResponse
    {
        public Guid RenglonId { get; set; }
        public int Secuencia { get; set; }
    }
}
