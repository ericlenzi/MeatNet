using MediatR;

namespace Meat.Application.Conformaciones.GetConformacion
{
    public class GetConformacionRequest : IRequest<GetConformacionResponse>
    {
        public string Codigo { get; set; }
    }
}
