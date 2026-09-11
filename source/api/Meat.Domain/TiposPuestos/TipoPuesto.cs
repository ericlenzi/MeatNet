using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meat.Domain.TiposPuestos
{
    /// <summary>
    /// Catalogo: clasificacion de los puestos de la planta (PAL - PALCO).
    ///
    /// Es comun a todas las empresas: nombra la clase de puesto, no el puesto. La especie con la
    /// que opera cada puesto y su establecimiento son del Puesto, no de esta clasificacion.
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
