using System.ComponentModel.DataAnnotations;

namespace Meat.Application.Materiales.UpdateMaterial
{
    public class UpdateMaterialRequestFromBody
    {
        [Required]
        public string Nombre { get; set; }
        public string TipoMaterialId { get; set; }
        public string UnidadMedidaId { get; set; }
        public string PesoTeorico { get; set; }
        public string ERP_Codigo { get; set; }
        public bool Activo { get; set; }
    }
}
