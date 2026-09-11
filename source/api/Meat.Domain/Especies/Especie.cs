using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meat.Domain.Especies
{
    public class Especie
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Codigo { get; set; }
        public string Nombre { get; set; }

        /// <summary>
        /// Banda de rinde caliente esperable para la especie, en porcentaje. Es el rango dentro
        /// del cual un rinde no llama la atencion: bovino ronda el 55%, porcino el 78%.
        ///
        /// No corrige ni acota el calculo, que sigue siendo el de R-A6: solo sirve para que el
        /// Analisis avise cuando el numero se va de rango, que casi siempre significa un peso de
        /// ingreso mal cargado (R-A7). Nullables a proposito: la especie sin banda configurada no
        /// dispara ningun aviso, igual que los catalogos del palco vacios no exigen nada.
        /// </summary>
        public double? RindeMinimo { get; set; }
        public double? RindeMaximo { get; set; }

        /// <summary>
        /// Merma de oreo de referencia del rubro, en porcentaje: lo que la media res pierde en
        /// la camara entre la pesada caliente y la fria. Ronda el 2%.
        ///
        /// Es el valor que se propone y que se usa cuando la planta no declara el suyo. No es
        /// una medicion: habilita un rinde frio ESTIMADO, que la pantalla muestra rotulado como
        /// tal (R-A8). Nullable a proposito: la especie sin merma configurada no muestra ningun
        /// rinde frio, igual que sin banda de rinde no dispara ningun aviso.
        /// </summary>
        public double? MermaOreoReferencia { get; set; }

        public bool Activo { get; set; }
    }
}
