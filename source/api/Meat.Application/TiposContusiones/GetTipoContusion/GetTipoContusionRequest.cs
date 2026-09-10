using MediatR;

namespace Meat.Application.TiposContusiones.GetTipoContusion
{
    public class GetTipoContusionRequest : IRequest<GetTipoContusionResponse>
    {
        public string Codigo { get; set; }
    }
}
