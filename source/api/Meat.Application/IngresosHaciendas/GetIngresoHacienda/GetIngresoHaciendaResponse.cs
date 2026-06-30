using System;
using System.Collections.Generic;

namespace Meat.Application.IngresosHaciendas.GetIngresoHacienda
{
    public class GetIngresoHaciendaResponse
    {
        public Guid Id { get; set; }
        public long NumeroIngreso { get; set; }

        public Guid EstablecimientoId { get; set; }
        public string EstablecimientoNombre { get; set; }

        public DateTime FechaHoraIngreso { get; set; }

        public string NumeroDte { get; set; }
        public DateTime FechaEmisionDte { get; set; }

        public Guid ClienteId { get; set; }
        public string ClienteNombre { get; set; }

        public Guid ClienteEstablecimientoId { get; set; }
        public string CodigoRenspa { get; set; }
        public string NumeroCUIG { get; set; }

        public int ProvinciaId { get; set; }
        public string ProvinciaNombre { get; set; }
        public string Localidad { get; set; }

        public string OrigenHaciendaId { get; set; }
        public string OrigenHaciendaNombre { get; set; }
        public string UsoHaciendaId { get; set; }
        public string UsoHaciendaNombre { get; set; }

        public string Transportista { get; set; }
        public string Chofer { get; set; }
        public string PatenteCamion { get; set; }
        public string PatenteJaula { get; set; }

        public double PesoBruto { get; set; }
        public double Tara { get; set; }
        public double PesoNeto { get; set; }

        public string EstadoIngresoId { get; set; }
        public string EstadoIngresoNombre { get; set; }
        public DateTime? FechaAprobacion { get; set; }

        public List<PesadaItem> Pesadas { get; set; } = new List<PesadaItem>();
        public List<UbicacionItem> Ubicaciones { get; set; } = new List<UbicacionItem>();
        public List<TropaItem> Tropas { get; set; } = new List<TropaItem>();
    }

    public class PesadaItem
    {
        public Guid Id { get; set; }
        public string TipoEspecieId { get; set; }
        public string TipoEspecieNombre { get; set; }
        public double PesoIngreso { get; set; }
        public string UnidadMedida { get; set; }
    }

    public class UbicacionItem
    {
        public Guid Id { get; set; }
        public Guid? TropaId { get; set; }
        public string TipoEspecieId { get; set; }
        public string TipoEspecieNombre { get; set; }
        public Guid AlmacenId { get; set; }
        public string AlmacenNombre { get; set; }
        public int Cantidad { get; set; }
        public double PesoPromedio { get; set; }
        public string EstadoHaciendaId { get; set; }
        public string EstadoHaciendaNombre { get; set; }
    }

    public class TropaItem
    {
        public Guid Id { get; set; }
        public long NumeroTropa { get; set; }
        public string EspecieCodigo { get; set; }
        public string EstadoTropaId { get; set; }
    }
}
