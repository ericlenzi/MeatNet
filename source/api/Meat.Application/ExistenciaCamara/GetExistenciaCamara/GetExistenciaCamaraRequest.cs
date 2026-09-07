using MediatR;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Meat.Application.ExistenciaCamara.GetExistenciaCamara
{
    public class GetExistenciaCamaraRequest : IRequest<GetExistenciaCamaraResponse>
    {
        [JsonIgnore]
        public string CodigoEmpresa { get; set; }

        public Guid? EstablecimientoId { get; set; }
        public Guid? AlmacenId { get; set; }
        public Guid? MaterialId { get; set; }

        /// <summary>
        /// Incluir las lineas con saldo cero (por ejemplo una media res que se cuarteo por
        /// completo). Por defecto se ocultan: no hay existencia que mostrar.
        /// </summary>
        public bool IncluirSaldoCero { get; set; }
    }

    public class GetExistenciaCamaraResponse
    {
        public IEnumerable<ExistenciaCamaraItem> Data { get; set; } = new List<ExistenciaCamaraItem>();

        public int TotalCantidad { get; set; }
        public double TotalPeso { get; set; }
    }

    public class ExistenciaCamaraItem
    {
        public Guid AlmacenId { get; set; }
        public string AlmacenNombre { get; set; }

        public Guid MaterialId { get; set; }
        public string MaterialCodigo { get; set; }
        public string MaterialNombre { get; set; }
        public string TipoMaterialId { get; set; }
        public string TipoMaterialNombre { get; set; }

        /// <summary>Piezas en existencia (suma con signo de los movimientos).</summary>
        public int Cantidad { get; set; }

        public double Peso { get; set; }

        /// <summary>Fecha del ultimo movimiento, para saber que tan fresca es la linea.</summary>
        public DateTime? UltimoMovimiento { get; set; }
    }
}
