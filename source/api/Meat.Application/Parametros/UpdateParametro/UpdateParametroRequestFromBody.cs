using System.ComponentModel.DataAnnotations;

namespace Meat.Application.Parametros.UpdateParametro
{
    public class UpdateParametroRequestFromBody
    {
        [Required]
        public string Nombre { get; set; }
        public string Valor { get; set; }
        public bool Activo { get; set; }
    }
}
