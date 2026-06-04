using MediatR;

namespace Meat.Application.Categorias.UpdateCategoria
{
    public class UpdateCategoriaRequest : IRequest<UpdateCategoriaResponse>
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string EspecieId { get; set; }
        public string TipoSexoId { get; set; }
        public string CodigoMaterialDesde { get; set; }
        public string CodigoMaterialHasta { get; set; }
        public bool Activo { get; set; }
    }
}
