using System.ComponentModel.DataAnnotations;

namespace Meat.Application.Categorias.UpdateCategoria
{
    public class UpdateCategoriaRequestFromBody
    {
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string EspecieId { get; set; }
        public string TipoSexoId { get; set; }
        public string CodigoMaterialDesde { get; set; }
        public string CodigoMaterialHasta { get; set; }
        public bool Activo { get; set; }
    }
}
