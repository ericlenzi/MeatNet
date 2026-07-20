using Meat.Domain.Especies;
using Meat.Domain.TiposDenticiones;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meat.Domain.Denticiones
{
    /// <summary>
    /// Catalogo: denticiones por Especie y Tipo de Denticion.
    /// </summary>
    public class Denticion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string EspecieId { get; set; }
        public virtual Especie Especie { get; set; }
        public string TipoDenticionId { get; set; }
        public virtual TipoDenticion TipoDenticion { get; set; }
        public bool Activo { get; set; }
    }
}
