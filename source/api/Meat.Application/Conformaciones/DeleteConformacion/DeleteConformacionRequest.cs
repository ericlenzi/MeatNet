using MediatR;

namespace Meat.Application.Conformaciones.DeleteConformacion
{
    public class DeleteConformacionRequest : IRequest<DeleteConformacionResponse>
    {
        public string Codigo { get; set; }
    }
}
