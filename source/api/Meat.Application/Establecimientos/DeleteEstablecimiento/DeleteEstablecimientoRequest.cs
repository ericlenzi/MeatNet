using Meat.Application.Shared;
using MediatR;
using System;

namespace Meat.Application.Establecimientos.DeleteEstablecimiento
{
    public class DeleteEstablecimientoRequest : RequestBase, IRequest<DeleteEstablecimientoResponse>
    {
        public Guid Id { get; set; }
    }
}
