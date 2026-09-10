using MediatR;

namespace Meat.Application.TiposContusiones.UpdateTipoContusion
{
    public class UpdateTipoContusionRequest : IRequest<UpdateTipoContusionResponse>
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string EspecieId { get; set; }
        public int Orden { get; set; }
        public bool Activo { get; set; }
    }
}
