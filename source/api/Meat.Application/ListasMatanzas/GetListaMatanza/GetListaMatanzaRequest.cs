using MediatR;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Meat.Application.ListasMatanzas.GetListaMatanza
{
    public class GetListaMatanzaRequest : IRequest<GetListaMatanzaResponse>
    {
        public Guid Id { get; set; }

        [JsonIgnore]
        public string CodigoEmpresa { get; set; }
    }

    public class GetListaMatanzaResponse
    {
        public Guid Id { get; set; }
        public long NumeroLista { get; set; }
        public DateTime Fecha { get; set; }

        public Guid EstablecimientoId { get; set; }
        public string EstablecimientoNombre { get; set; }

        public string EspecieId { get; set; }
        public string EspecieNombre { get; set; }

        public string EstadoListaMatanzaId { get; set; }
        public string EstadoListaMatanzaNombre { get; set; }

        public int Version { get; set; }
        public DateTime? FechaConfirmacion { get; set; }
        public DateTime? FechaInicioEjecucion { get; set; }
        public DateTime? FechaFinalizacion { get; set; }

        public List<RenglonItem> Renglones { get; set; } = new List<RenglonItem>();
        public List<MovimientoItem> Movimientos { get; set; } = new List<MovimientoItem>();
    }

    public class RenglonItem
    {
        public Guid Id { get; set; }
        public Guid TropaId { get; set; }
        public long NumeroTropa { get; set; }
        public Guid AlmacenId { get; set; }
        public string AlmacenNombre { get; set; }
        public int Secuencia { get; set; }
        public int Cantidad { get; set; }
        public int CantidadFaenada { get; set; }
    }

    public class MovimientoItem
    {
        public Guid Id { get; set; }
        public int Version { get; set; }
        public DateTime Fecha { get; set; }
        public Guid UsuarioId { get; set; }
        public string TipoMovimiento { get; set; }
        public Guid? TropaId { get; set; }
        public Guid? AlmacenId { get; set; }
        public int? CantidadAnterior { get; set; }
        public int? CantidadNueva { get; set; }
        public int? SecuenciaAnterior { get; set; }
        public int? SecuenciaNueva { get; set; }
        public string Motivo { get; set; }
    }
}
