using Meat.Application.Shared;
using MediatR;
using System;
using System.Collections.Generic;

namespace Meat.Application.Romaneos.GetMonitorFaena
{
    public class GetMonitorFaenaRequest : RequestBase, IRequest<GetMonitorFaenaResponse>
    {

        public Guid ListaMatanzaId { get; set; }
    }

    public class GetMonitorFaenaResponse
    {
        public Guid ListaMatanzaId { get; set; }
        public long NumeroLista { get; set; }
        public string EspecieNombre { get; set; }
        public string EstadoListaMatanzaId { get; set; }
        /// <summary>Puesto (palco) donde se faena la jornada. Lo declara la lista de matanza.</summary>
        public string PuestoCodigo { get; set; }
        public string PuestoNombre { get; set; }

        public int TotalPlanificado { get; set; }
        public int TotalFaenado { get; set; }
        public int TotalPendiente { get; set; }
        public int AnimalesRomaneados { get; set; }         // romaneos no anulados
        public double KgTotales { get; set; }

        /// <summary>Reses condenadas enteras de la jornada (R-E23): faenadas, pero no son carne.</summary>
        public int AnimalesDecomisados { get; set; }

        /// <summary>Medias reses condenadas por separado (R-E27).</summary>
        public int PiezasDecomisadas { get; set; }

        /// <summary>Kg condenados de la jornada, fuera de KgTotales.</summary>
        public double KgDecomisados { get; set; }
        public double RitmoPorHora { get; set; }            // animales/hora desde el 1er romaneo

        public IEnumerable<RenglonMonitorItem> PorRenglon { get; set; } = new List<RenglonMonitorItem>();
    }

    public class RenglonMonitorItem
    {
        public Guid ListaMatanzaDetalleId { get; set; }
        public int Secuencia { get; set; }
        public long NumeroTropa { get; set; }
        public string AlmacenNombre { get; set; }
        public string AlmacenDestinoNombre { get; set; }    // camara destino planificada del renglon
        public string TipoEspecieNombre { get; set; }
        public int Cantidad { get; set; }
        public int CantidadFaenada { get; set; }
        public int Pendiente { get; set; }
        public long? RomaneoDesde { get; set; }             // 1er nro de romaneo no anulado del renglon
        public long? RomaneoHasta { get; set; }             // ultimo nro de romaneo no anulado del renglon
    }
}
