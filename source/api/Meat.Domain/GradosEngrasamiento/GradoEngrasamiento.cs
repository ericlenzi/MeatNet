using Meat.Domain.Especies;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meat.Domain.GradosEngrasamiento
{
    /// <summary>
    /// Catalogo: grados de engrasamiento (terminacion) de la res por Especie, el otro eje de la
    /// tipificacion oficial. Es la cobertura de grasa de la media res.
    ///
    /// Igual que Conformacion, es una escala ordinal y Orden guarda su posicion.
    /// </summary>
    public class GradoEngrasamiento
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string EspecieId { get; set; }
        public virtual Especie Especie { get; set; }

        /// <summary>Posicion en la escala. Es el orden con el que se listan.</summary>
        public int Orden { get; set; }
        public bool Activo { get; set; }
    }
}
