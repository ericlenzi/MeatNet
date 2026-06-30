using System.ComponentModel.DataAnnotations;

namespace Meat.Application.TiposEspecies.UpdateTipoEspecie
{
    public class UpdateTipoEspecieRequestFromBody
    {
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string EspecieId { get; set; }
        public string TipoSexoId { get; set; }
        public string CodigoMaterial { get; set; }
        public string ERP_Codigo { get; set; }
        public double PesoTeorico { get; set; }
        public bool Activo { get; set; }
    }
}
