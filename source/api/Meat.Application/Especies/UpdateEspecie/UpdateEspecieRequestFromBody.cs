using System.ComponentModel.DataAnnotations;

namespace Meat.Application.Especies.UpdateEspecie
{
    public class UpdateEspecieRequestFromBody
    {
        [Required]
        public string Nombre { get; set; }

        /// <summary>Banda de rinde esperable (%), para el aviso del Analisis (R-A7).</summary>
        public double? RindeMinimo { get; set; }
        public double? RindeMaximo { get; set; }

        /// <summary>Merma de oreo de referencia (%), para el rinde frio estimado (R-A8).</summary>
        public double? MermaOreoReferencia { get; set; }

        public bool Activo { get; set; }
    }
}
