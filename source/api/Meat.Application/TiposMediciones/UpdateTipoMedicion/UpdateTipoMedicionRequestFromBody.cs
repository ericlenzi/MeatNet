using System.ComponentModel.DataAnnotations;

namespace Meat.Application.TiposMediciones.UpdateTipoMedicion
{
    public class UpdateTipoMedicionRequestFromBody
    {
        [Required]
        public string Nombre { get; set; }
        public bool Activo { get; set; }
    }
}
