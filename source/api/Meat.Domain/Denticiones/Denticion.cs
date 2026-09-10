using Meat.Domain.Especies;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meat.Domain.Denticiones
{
    /// <summary>
    /// Catalogo: denticion de la res por Especie, el cuarto dato que el tipificador registra en
    /// el palco junto con conformacion, engrasamiento y contusion.
    ///
    /// Es el recuento de incisivos permanentes, que estima la edad del animal: en vacuno va de
    /// diente de leche a boca llena (0, 2, 4, 6 y 8 dientes). Como conformacion y engrasamiento,
    /// es una escala ordinal y Orden guarda su posicion, porque los codigos no se ordenan solos.
    /// </summary>
    public class Denticion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string EspecieId { get; set; }
        public virtual Especie Especie { get; set; }

        /// <summary>Posicion en la escala, de menor a mayor edad. Es el orden con el que se listan.</summary>
        public int Orden { get; set; }
        public bool Activo { get; set; }
    }
}
