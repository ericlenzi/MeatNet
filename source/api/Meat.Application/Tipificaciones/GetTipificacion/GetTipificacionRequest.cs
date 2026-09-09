using System;
using Meat.Application.Shared;
using MediatR;

namespace Meat.Application.Tipificaciones.GetTipificacion
{
    public class GetTipificacionRequest : RequestBase, IRequest<GetTipificacionResponse>
    {
        public Guid Id { get; set; }

    }
}
