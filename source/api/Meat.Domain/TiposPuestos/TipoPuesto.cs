using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meat.Domain.TiposPuestos
{
    /// <summary>
    /// Catalogo: clasificacion de Puestos de faena (ej. PAL - PALCO, ENC - ENCAJADO).
    /// </summary>
    public class TipoPuesto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public bool Activo { get; set; }
    }
}
