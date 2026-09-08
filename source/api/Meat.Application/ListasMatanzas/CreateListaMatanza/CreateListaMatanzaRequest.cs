using Meat.Application.Shared;
using MediatR;
using System;
using System.Collections.Generic;

namespace Meat.Application.ListasMatanzas.CreateListaMatanza
{
    public class CreateListaMatanzaRequest : RequestBase, IRequest<CreateListaMatanzaResponse>
    {

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
