using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meat.Domain.TiposMovimientosCamaras
{
    /// <summary>
    /// Catalogo de tipos de movimiento de la existencia de camara: clasifica cada
    /// linea del log MovimientoCamara (ingreso por liberacion, baja/alta por cuarteo,
    /// egreso a Ciclo II). El signo de la existencia lo lleva Cantidad/Peso del
    /// movimiento, no este catalogo.
    /// </summary>
    public class TipoMovimientoCamara
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public bool Activo { get; set; }
    }
}
