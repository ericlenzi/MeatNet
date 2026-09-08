using Meat.Application.Shared;
using MediatR;
using System;

namespace Meat.Application.Establecimientos.GetEstablecimiento
{
    public class GetEstablecimientoRequest : RequestBase, IRequest<GetEstablecimientoResponse>
    {
        public Guid Id { get; set; }
    }
}
