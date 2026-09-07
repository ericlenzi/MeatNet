using MediatR;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Meat.Application.ExistenciaCamara.GetMovimientosCamara
{
    public class GetMovimientosCamaraRequest : IRequest<GetMovimientosCamaraResponse>
    {
        [JsonIgnore]
        public string CodigoEmpresa { get; set; }

        public Guid? AlmacenId { get; set; }
        public Guid? MaterialId { get; set; }
        public Guid? TropaId { get; set; }

        /// <summary>Filtra por el dueno de la hacienda.</summary>
        public Guid? ClienteId { get; set; }

        /// <summary>Movimientos originados en una pieza concreta: la trazabilidad de un garron.</summary>
        public Guid? RomaneoPiezaOrigenId { get; set; }

        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
    }

    public class GetMovimientosCamaraResponse
    {
        public IEnumerable<MovimientoCamaraItem> Data { get; set; } = new List<MovimientoCamaraItem>();
    }

    public class MovimientoCamaraItem
    {
        public Guid Id { get; set; }
        public DateTime Fecha { get; set; }

        public string TipoMovimientoId { get; set; }
        public string TipoMovimientoNombre { get; set; }

        public Guid AlmacenId { get; set; }
        public string AlmacenNombre { get; set; }

        public Guid MaterialId { get; set; }
        public string MaterialCodigo { get; set; }
        public string MaterialNombre { get; set; }

        public int Cantidad { get; set; }
        public double Peso { get; set; }

        /// <summary>Agrupa la baja y las altas de un mismo cuarteo.</summary>
        public Guid? TransformacionId { get; set; }

        // Origen: hasta que media res / garron se remonta este material.
        public Guid? RomaneoPiezaOrigenId { get; set; }
        public long? NumeroRomaneo { get; set; }
        public int? NumeroGarron { get; set; }
        public string Letra { get; set; }

        public Guid? TropaId { get; set; }
        public long? NumeroTropa { get; set; }
        public Guid? ClienteId { get; set; }
        public string ClienteNombre { get; set; }
        public string EspecieId { get; set; }
        public string TipoEspecieId { get; set; }

        public string Referencia { get; set; }
    }
}
