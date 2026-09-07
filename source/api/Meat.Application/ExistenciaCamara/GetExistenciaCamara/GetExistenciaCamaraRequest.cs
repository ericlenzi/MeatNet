using MediatR;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Meat.Application.ExistenciaCamara.GetExistenciaCamara
{
    /// <summary>
    /// Corte por el que se agrupa la existencia. Cada uno responde una pregunta distinta
    /// sobre el mismo saldo.
    /// </summary>
    public static class AgrupacionExistencia
    {
        /// <summary>Por (camara, material): cuanto hay de cada producto y donde. Inventario.</summary>
        public const string Material = "MATERIAL";

        /// <summary>Por (cliente, material): que tiene cada proveedor.</summary>
        public const string Proveedor = "PROVEEDOR";

        /// <summary>Por (camara, cliente): de quien es lo que hay en cada camara.</summary>
        public const string Camara = "CAMARA";
    }

    public class GetExistenciaCamaraRequest : IRequest<GetExistenciaCamaraResponse>
    {
        [JsonIgnore]
        public string CodigoEmpresa { get; set; }

        public Guid? EstablecimientoId { get; set; }
        public Guid? AlmacenId { get; set; }
        public Guid? MaterialId { get; set; }

        /// <summary>Filtra por el dueno de la hacienda (el proveedor del frigorifico).</summary>
        public Guid? ClienteId { get; set; }

        /// <summary>Ver AgrupacionExistencia. Por defecto MATERIAL, que es la vista de inventario.</summary>
        public string AgruparPor { get; set; }

        /// <summary>
        /// Incluir las lineas con saldo cero (por ejemplo una media res que se cuarteo por
        /// completo). Por defecto se ocultan: no hay existencia que mostrar.
        /// </summary>
        public bool IncluirSaldoCero { get; set; }
    }

    public class GetExistenciaCamaraResponse
    {
        public IEnumerable<ExistenciaCamaraItem> Data { get; set; } = new List<ExistenciaCamaraItem>();

        /// <summary>El corte efectivamente aplicado, para que el front sepa que columnas mostrar.</summary>
        public string AgruparPor { get; set; }

        public int TotalCantidad { get; set; }
        public double TotalPeso { get; set; }
    }

    /// <summary>
    /// Una linea del saldo. Segun la agrupacion se llenan unos campos u otros: los que no
    /// participan del corte vienen en null porque agregarian filas de distinto origen.
    /// </summary>
    public class ExistenciaCamaraItem
    {
        public Guid? AlmacenId { get; set; }
        public string AlmacenNombre { get; set; }

        public Guid? MaterialId { get; set; }
        public string MaterialCodigo { get; set; }
        public string MaterialNombre { get; set; }
        public string TipoMaterialId { get; set; }
        public string TipoMaterialNombre { get; set; }

        public Guid? ClienteId { get; set; }
        public string ClienteNombre { get; set; }

        /// <summary>Piezas en existencia (suma con signo de los movimientos).</summary>
        public int Cantidad { get; set; }

        public double Peso { get; set; }

        /// <summary>Fecha del ultimo movimiento, para saber que tan fresca es la linea.</summary>
        public DateTime? UltimoMovimiento { get; set; }
    }
}
