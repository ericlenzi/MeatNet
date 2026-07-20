using Meat.Domain.Especies;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meat.Domain.TiposDenticiones
{
    /// <summary>
    /// Catalogo: tipos de denticiones por Especie (solo vacunos).
    /// </summary>
    public class TipoDenticion
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
