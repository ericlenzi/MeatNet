using Meat.Application.Shared;
using MediatR;

namespace Meat.Application.Tipificaciones.GetTipificacion
{
    public class GetTipificacionRequest : RequestBase, IRequest<GetTipificacionResponse>
    {
        public string Codigo { get; set; }

    }
}
