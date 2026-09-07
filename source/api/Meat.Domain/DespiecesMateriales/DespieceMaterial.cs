using Meat.Domain.Materiales;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meat.Domain.DespiecesMateriales
{
    /// <summary>
    /// Regla de despiece (mini-BOM): transforma un Material origen (ej. MEDIA RES) en un
    /// Material destino (ej. CUARTO DELANTERO). Rendimiento = fraccion (0..1) del peso del origen
    /// que va a este destino; la suma por origen deberia ser ~1. La usa la Liberacion (Paso 4).
    /// </summary>
    public class DespieceMaterial
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        public Guid MaterialOrigenId { get; set; }
        public virtual Material MaterialOrigen { get; set; }

        public Guid MaterialDestinoId { get; set; }
        public virtual Material MaterialDestino { get; set; }

        // Piezas destino por unidad de origen (normalmente 1).
        public int Cantidad { get; set; }

        // Fraccion del peso del origen que va a este destino (0..1).
        public double Rendimiento { get; set; }

        public bool Activo { get; set; }
        public DateTime FechaActualizacion { get; set; }
    }
}
