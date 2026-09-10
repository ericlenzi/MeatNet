using System.ComponentModel.DataAnnotations;

namespace Meat.Application.MotivosDecomisos.UpdateMotivoDecomiso
{
    public class UpdateMotivoDecomisoRequestFromBody
    {
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string EspecieId { get; set; }
        public int Orden { get; set; }
        public bool ExigeContusion { get; set; }
        public bool Activo { get; set; }
    }
}
