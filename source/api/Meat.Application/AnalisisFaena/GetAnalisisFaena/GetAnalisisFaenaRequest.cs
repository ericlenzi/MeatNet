using MediatR;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Meat.Application.AnalisisFaena.GetAnalisisFaena
{
    public class GetAnalisisFaenaRequest : IRequest<GetAnalisisFaenaResponse>
    {
        [JsonIgnore]
        public string CodigoEmpresa { get; set; }

        public Guid ListaMatanzaId { get; set; }
    }

    public class GetAnalisisFaenaResponse
    {
        // Cabecera
        public Guid ListaMatanzaId { get; set; }
        public long NumeroLista { get; set; }
        public DateTime Fecha { get; set; }
        public string EspecieId { get; set; }
        public string EstadoListaMatanzaId { get; set; }
        public string EstablecimientoNombre { get; set; }

        // Resumen de la jornada
        public int AnimalesFaenados { get; set; }
        public int Piezas { get; set; }
        public double KgFaena { get; set; }

        /// <summary>Kg vivos de los animales faenados. Null si ninguna tropa tiene peso de ingreso (R-A3).</summary>
        public double? KgVivos { get; set; }

        /// <summary>Rinde caliente (%). Null cuando no hay peso vivo para calcularlo (R-A3).</summary>
        public double? RindeCaliente { get; set; }

        /// <summary>Animales faenados cuya tropa no tiene peso de ingreso: quedan fuera del rinde.</summary>
        public int AnimalesSinPesoVivo { get; set; }

        public int PiezasLiberadas { get; set; }

        public IEnumerable<AnalisisClienteItem> PorCliente { get; set; } = new List<AnalisisClienteItem>();
        public IEnumerable<PlanVsRealItem> PlanVsReal { get; set; } = new List<PlanVsRealItem>();
        public IEnumerable<TipificacionConsolidadaItem> Tipificaciones { get; set; } = new List<TipificacionConsolidadaItem>();
        public IEnumerable<DispersionPesoItem> Dispersion { get; set; } = new List<DispersionPesoItem>();
        public IEnumerable<DestinoCamaraItem> Camaras { get; set; } = new List<DestinoCamaraItem>();
    }

    /// <summary>El corte de facturacion del servicio de faena.</summary>
    public class AnalisisClienteItem
    {
        public Guid ClienteId { get; set; }
        public string ClienteNombre { get; set; }
        public int AnimalesFaenados { get; set; }
        public int Piezas { get; set; }
        public double KgFaena { get; set; }
        public double? KgVivos { get; set; }
        public double? RindeCaliente { get; set; }
        /// <summary>Participacion en los kg de faena de la jornada (%).</summary>
        public double ParticipacionKg { get; set; }
    }

    public class PlanVsRealItem
    {
        public long NumeroTropa { get; set; }
        public Guid ClienteId { get; set; }
        public string ClienteNombre { get; set; }
        public string CorralNombre { get; set; }
        public string TipoEspecieNombre { get; set; }
        public int Secuencia { get; set; }
        public int Planificado { get; set; }
        public int Faenado { get; set; }
        public int Diferencia { get; set; }
        /// <summary>Faenado sobre planificado (%).</summary>
        public double Cumplimiento { get; set; }
        public double? PesoPromedioVivo { get; set; }
    }

    public class TipificacionConsolidadaItem
    {
        public string TipificacionId { get; set; }
        public string Descripcion { get; set; }
        public string MaterialNombre { get; set; }
        public int Piezas { get; set; }
        public double KgFaena { get; set; }
        public double PesoPromedio { get; set; }
        /// <summary>Participacion en los kg de la jornada (%).</summary>
        public double ParticipacionKg { get; set; }
    }

    public class DispersionPesoItem
    {
        public string TipoEspecieId { get; set; }
        public string TipoEspecieNombre { get; set; }
        public int Piezas { get; set; }
        public double PesoPromedio { get; set; }
        public double PesoMinimo { get; set; }
        public double PesoMaximo { get; set; }
        /// <summary>Piezas cuyo peso quedo fuera del rango de su tipificacion.</summary>
        public int PiezasFueraRango { get; set; }
    }

    public class DestinoCamaraItem
    {
        public string AlmacenNombre { get; set; }
        public string MaterialNombre { get; set; }
        public int Cantidad { get; set; }
        public double Peso { get; set; }
    }
}
