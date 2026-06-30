using MediatR;

namespace Meat.Application.TiposEspecies.UpdateTipoEspecie
{
    public class UpdateTipoEspecieRequest : IRequest<UpdateTipoEspecieResponse>
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string EspecieId { get; set; }
        public string TipoSexoId { get; set; }
        public string CodigoMaterial { get; set; }
        public string ERP_Codigo { get; set; }
        public double PesoTeorico { get; set; }
        public bool Activo { get; set; }
    }
}
