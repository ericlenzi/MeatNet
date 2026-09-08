using Meat.Application.Shared;
using MediatR;
using System;
using System.Text.Json.Serialization;

namespace Meat.Application.ListasMatanzas.FinalizarListaMatanza
{
    public class FinalizarListaMatanzaRequest : RequestBase, IRequest<FinalizarListaMatanzaResponse>
    {
        [JsonIgnore]
        public Guid Id { get; set; }


        public string Motivo { get; set; }                 // opcional: por que quedo sobrante sin faenar
    }

    public class FinalizarListaMatanzaResponse
    {
        public int TotalLiberado { get; set; }             // animales planificados no faenados que se liberan
        public int RenglonesConSobrante { get; set; }
    }
}
