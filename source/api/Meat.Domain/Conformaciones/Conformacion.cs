using Meat.Domain.Especies;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meat.Domain.Conformaciones
{
    /// <summary>
    /// Catalogo: grados de conformacion de la res por Especie, uno de los ejes de la tipificacion
    /// oficial. Es el desarrollo muscular de la media res, que se determina mirandola en playa.
    ///
    /// Es una escala ordinal, no un conjunto suelto: Orden guarda de mejor a peor, porque
    /// alfabeticamente los codigos no dicen nada.
    /// </summary>
    public class Conformacion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string EspecieId { get; set; }
        public virtual Especie Especie { get; set; }

        /// <summary>Posicion en la escala, de mejor a peor. Es el orden con el que se listan.</summary>
        public int Orden { get; set; }
        public bool Activo { get; set; }
    }
}
