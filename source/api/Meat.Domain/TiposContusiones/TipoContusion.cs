using Meat.Domain.Especies;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meat.Domain.TiposContusiones
{
    /// <summary>
    /// Catalogo: grado de contusion de la media res por Especie, el cuarto dato que el tipificador
    /// registra en el palco.
    ///
    /// A diferencia de los otros tres ejes, este se determina por pieza y no por animal: el golpe
    /// esta en una media res concreta. El catalogo incluye una opcion de "sin contusion", y no es
    /// un relleno: es lo que hace que el dato sea obligatorio de verdad, porque el tipificador
    /// tiene que pronunciarse siempre, tambien cuando la res esta sana.
    ///
    /// Es una escala ordinal (sin contusion, leve, moderada, grave) y Orden guarda su posicion.
    /// </summary>
    public class TipoContusion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string EspecieId { get; set; }
        public virtual Especie Especie { get; set; }

        /// <summary>Posicion en la escala, de menor a mayor severidad. Es el orden con el que se listan.</summary>
        public int Orden { get; set; }
        public bool Activo { get; set; }
    }
}
