using MediatR;

namespace Meat.Application.TiposContusiones.DeleteTipoContusion
{
    public class DeleteTipoContusionRequest : IRequest<DeleteTipoContusionResponse>
    {
        public string Codigo { get; set; }
    }
}
