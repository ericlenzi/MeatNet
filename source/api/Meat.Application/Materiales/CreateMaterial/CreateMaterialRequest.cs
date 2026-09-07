using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Meat.Application.Materiales.CreateMaterial
{
    public class CreateMaterialRequest : IRequest<CreateMaterialResponse>
    {
        [Required]
        public string CodigoMaterial { get; set; }
        [Required]
        public string Nombre { get; set; }
        public string TipoMaterialId { get; set; }
        public string UnidadMedidaId { get; set; }
        public string PesoTeorico { get; set; }
        public string ERP_Codigo { get; set; }
    }
}
