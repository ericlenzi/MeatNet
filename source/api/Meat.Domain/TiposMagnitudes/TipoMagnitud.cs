using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meat.Domain.TiposMagnitudes
{
    /// <summary>
    /// Catalogo: que se mide de una pieza del romaneo (PESO). Es la magnitud, no el metodo:
    /// con que se toma el valor (manual, balanza, automatico) lo dice TipoMedicion.
    /// </summary>
    public class TipoMagnitud
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public bool Activo { get; set; }
    }
}
