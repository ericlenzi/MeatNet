using Meat.Domain.Especies;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meat.Domain.MotivosDecomisos
{
    /// <summary>
    /// Catalogo: motivos de decomiso por Especie. Es la causa sanitaria por la que la inspeccion
    /// condena una res entera (decomiso total, en el Romaneo) o retira kilos de una media res
    /// concreta (decomiso parcial, en la RomaneoPieza). Ver R-E23 en EjecucionFaena.md.
    ///
    /// Comparte forma con los cuatro catalogos del palco (Codigo, Nombre, Especie, Orden, Activo)
    /// y por eso comparte pantalla con ellos, pero no es una escala: Orden es solo la posicion en
    /// la lista del puesto, para que los motivos frecuentes queden arriba.
    /// </summary>
    public class MotivoDecomiso
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string EspecieId { get; set; }
        public virtual Especie Especie { get; set; }

        /// <summary>Posicion en la lista del puesto: los motivos mas frecuentes primero.</summary>
        public int Orden { get; set; }

        /// <summary>
        /// El motivo describe un golpe. Si esta marcado, la media res que se decomisa por el
        /// tiene que traer una contusion registrada distinta de la primera de la escala
        /// ("sin contusion"): decomisar kilos por contusion sobre una pieza declarada sana es
        /// una contradiccion (R-E26).
        ///
        /// Es una marca del catalogo y no un codigo escrito en la logica, por la misma razon que
        /// los datos del palco: el nomenclador lo administra el SUPERADMIN y puede cambiar.
        /// </summary>
        public bool ExigeContusion { get; set; }
        public bool Activo { get; set; }
    }
}
