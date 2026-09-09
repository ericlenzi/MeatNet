using MediatR;

namespace Meat.Application.TiposEspecies.UpdateTipoEspecie
{
    public class UpdateTipoEspecieRequest : IRequest<UpdateTipoEspecieResponse>
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string EspecieId { get; set; }
        public string TipoSexoId { get; set; }
        public double PesoTeoricoReferencia { get; set; }
        public bool Activo { get; set; }
    }
}
