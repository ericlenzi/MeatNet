using Meat.Application.Shared;
using MediatR;

namespace Meat.Application.Tipificaciones.DeleteTipificacion
{
    public class DeleteTipificacionRequest : RequestBase, IRequest<DeleteTipificacionResponse>
    {
        public string Codigo { get; set; }

    }
}
