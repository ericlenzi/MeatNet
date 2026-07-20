using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meat.Domain.TiposMediciones
{
    /// <summary>
    /// Catalogo: tipos de mediciones que se registran en la tipificacion.
    /// </summary>
    public class TipoMedicion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public bool Activo { get; set; }
    }
}
