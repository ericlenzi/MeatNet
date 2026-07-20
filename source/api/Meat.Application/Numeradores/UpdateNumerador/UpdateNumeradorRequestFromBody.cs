using System.ComponentModel.DataAnnotations;

namespace Meat.Application.Numeradores.UpdateNumerador
{
    public class UpdateNumeradorRequestFromBody
    {
        public string Descripcion { get; set; }
        [Required]
        public string TipoNumerador { get; set; }
        public int UltimoNumero { get; set; }
        public bool Activo { get; set; }
    }
}
