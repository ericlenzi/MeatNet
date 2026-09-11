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

        /// <summary>Dia de faena de la jornada. El tablero queda abierto en la planta: sin la
        /// fecha a la vista, la jornada de ayer se ve igual que la de hoy.</summary>
        public DateTime Fecha { get; set; }

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

        /// <summary>Como se van llenando las camaras de la jornada (R-E29).</summary>
        public IEnumerable<OcupacionCamaraItem> OcupacionCamaras { get; set; } = new List<OcupacionCamaraItem>();
    }

    /// <summary>
    /// Ocupacion de una camara mientras la jornada corre (R-E29). No es existencia: la existencia
    /// nace en la Liberacion. Es lo que hay colgado y lo que va a llegar, para avisar antes de que
    /// la camara se llene.
    /// </summary>
    public class OcupacionCamaraItem
    {
        public Guid? AlmacenId { get; set; }                 // null = renglones sin camara asignada
        public string AlmacenNombre { get; set; }

        public int PiezasColgadas { get; set; }             // piezas de la jornada que van a esta camara
        public double KgColgados { get; set; }

        public int PiezasPendientes { get; set; }           // proyeccion de lo que falta faenar
        public int PiezasSaldoPrevio { get; set; }          // producto de jornadas anteriores
        public double KgSaldoPrevio { get; set; }

        public int PiezasProyectadas { get; set; }          // colgadas + pendientes + saldo previo
        public int Capacidad { get; set; }                  // Almacen.Capacidad; 0 = sin declarar

        /// <summary>Ocupacion proyectada sobre la capacidad (%). Null si la camara no la declara.</summary>
        public double? PorcentajeOcupacion { get; set; }

        /// <summary>La proyeccion se pasa de la capacidad declarada.</summary>
        public bool Excedida { get; set; }
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
