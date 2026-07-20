using MediatR;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Meat.Application.ListasMatanzas.CreateListaMatanza
{
    public class CreateListaMatanzaRequest : IRequest<CreateListaMatanzaResponse>
    {
        [JsonIgnore]
        public string CodigoEmpresa { get; set; }

        public Guid EstablecimientoId { get; set; }
        public string EspecieId { get; set; }
        public Guid? PuestoId { get; set; }
        public DateTime Fecha { get; set; }

        public List<ListaMatanzaDetalleInput> Renglones { get; set; } = new List<ListaMatanzaDetalleInput>();
    }

    public class CreateListaMatanzaResponse
    {
        public Guid Id { get; set; }
        public long NumeroLista { get; set; }
    }
}
