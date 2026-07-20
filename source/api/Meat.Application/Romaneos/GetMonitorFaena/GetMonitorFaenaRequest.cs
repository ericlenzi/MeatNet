using MediatR;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Meat.Application.Romaneos.GetMonitorFaena
{
    public class GetMonitorFaenaRequest : IRequest<GetMonitorFaenaResponse>
    {
        [JsonIgnore]
        public string CodigoEmpresa { get; set; }

        public Guid ListaMatanzaId { get; set; }
    }

    public class GetMonitorFaenaResponse
    {
        public Guid ListaMatanzaId { get; set; }
        public long NumeroLista { get; set; }
        public string EspecieNombre { get; set; }
        public string EstadoListaMatanzaId { get; set; }

        public int TotalPlanificado { get; set; }
        public int TotalFaenado { get; set; }
        public int TotalPendiente { get; set; }
        public int AnimalesRomaneados { get; set; }         // romaneos no anulados
        public double KgTotales { get; set; }
        public double RitmoPorHora { get; set; }            // animales/hora desde el 1er romaneo

        public IEnumerable<RenglonMonitorItem> PorRenglon { get; set; } = new List<RenglonMonitorItem>();
    }

    public class RenglonMonitorItem
    {
        public int Secuencia { get; set; }
        public long NumeroTropa { get; set; }
        public string AlmacenNombre { get; set; }
        public string TipoEspecieNombre { get; set; }
        public int Cantidad { get; set; }
        public int CantidadFaenada { get; set; }
        public int Pendiente { get; set; }
    }
}
