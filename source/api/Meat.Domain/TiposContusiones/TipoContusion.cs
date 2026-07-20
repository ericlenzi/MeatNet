using Meat.Domain.Especies;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meat.Domain.TiposContusiones
{
    /// <summary>
    /// Catalogo: tipos de contusiones por Especie (solo vacunos).
    /// </summary>
    public class TipoContusion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string EspecieId { get; set; }
        public virtual Especie Especie { get; set; }
        public bool Activo { get; set; }
    }
}
