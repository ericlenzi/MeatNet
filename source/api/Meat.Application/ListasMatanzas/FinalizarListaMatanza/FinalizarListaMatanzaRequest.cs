using MediatR;
using System;
using System.Text.Json.Serialization;

namespace Meat.Application.ListasMatanzas.FinalizarListaMatanza
{
    public class FinalizarListaMatanzaRequest : IRequest<FinalizarListaMatanzaResponse>
    {
        [JsonIgnore]
        public Guid Id { get; set; }

        [JsonIgnore]
        public string CodigoEmpresa { get; set; }
        [JsonIgnore]
        public Guid UsuarioId { get; set; }

        public string Motivo { get; set; }                 // opcional: por que quedo sobrante sin faenar
    }

    public class FinalizarListaMatanzaResponse
    {
        public int TotalLiberado { get; set; }             // animales planificados no faenados que se liberan
        public int RenglonesConSobrante { get; set; }
    }
}
