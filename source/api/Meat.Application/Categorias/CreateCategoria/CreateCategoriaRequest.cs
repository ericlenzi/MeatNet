using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Meat.Application.Categorias.CreateCategoria
{
    public class CreateCategoriaRequest : IRequest<CreateCategoriaResponse>
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string EspecieId { get; set; }
        public string TipoSexoId { get; set; }
        public string CodigoMaterialDesde { get; set; }
        public string CodigoMaterialHasta { get; set; }
    }
}
