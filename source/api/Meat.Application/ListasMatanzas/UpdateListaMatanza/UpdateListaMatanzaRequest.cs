using MediatR;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Meat.Application.ListasMatanzas.UpdateListaMatanza
{
    public class UpdateListaMatanzaRequest : IRequest<UpdateListaMatanzaResponse>
    {
        [JsonIgnore]
        public Guid Id { get; set; }

        [JsonIgnore]
        public string CodigoEmpresa { get; set; }

        public string EspecieId { get; set; }
        public DateTime Fecha { get; set; }

        public List<ListaMatanzaDetalleInput> Renglones { get; set; } = new List<ListaMatanzaDetalleInput>();
    }

    public class UpdateListaMatanzaResponse
    {
    }
}
