using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meat.Domain.DestinosComerciales
{
    /// <summary>
    /// Catalogo: destinos comerciales de la media res / cuarto.
    /// </summary>
    public class DestinoComercial
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public bool Activo { get; set; }
    }
}
