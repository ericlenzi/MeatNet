using System.ComponentModel.DataAnnotations;

namespace Meat.Application.Parametros.UpdateParametro
{
    public class UpdateParametroRequestFromBody
    {
        [Required]
        public string Codigo { get; set; }

        [Required]
        public string Valor { get; set; }
    }
}
