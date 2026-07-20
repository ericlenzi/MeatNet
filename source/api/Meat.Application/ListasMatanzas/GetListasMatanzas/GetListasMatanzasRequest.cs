using MediatR;
using Meat.Application.Shared;
using System;
using System.Collections.Generic;

namespace Meat.Application.ListasMatanzas.GetListasMatanzas
{
    public class GetListasMatanzasRequest : RequestListBase, IRequest<GetListasMatanzasResponse>
    {
        public Guid? EstablecimientoId { get; set; }
        public string EspecieId { get; set; }
        public string EstadoListaMatanzaId { get; set; }
        public DateTime? Fecha { get; set; }
    }

    public class GetListasMatanzasResponse : ResponseListBase<IEnumerable<ListaMatanzaListItem>>
    {
    }

    public class ListaMatanzaListItem
    {
        public Guid Id { get; set; }
        public long NumeroLista { get; set; }
        public DateTime Fecha { get; set; }
        public Guid EstablecimientoId { get; set; }
        public string EstablecimientoNombre { get; set; }
        public string EspecieId { get; set; }
        public string EspecieNombre { get; set; }
        public Guid? PuestoId { get; set; }
        public string PuestoCodigo { get; set; }
        public string EstadoListaMatanzaId { get; set; }
        public string EstadoListaMatanzaNombre { get; set; }
        public int Version { get; set; }
        public int TotalRenglones { get; set; }
        public int TotalCabezas { get; set; }
    }
}
