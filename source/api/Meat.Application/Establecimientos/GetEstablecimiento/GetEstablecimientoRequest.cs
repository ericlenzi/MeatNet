using MediatR;
using System;

namespace Meat.Application.Establecimientos.GetEstablecimiento
{
    public class GetEstablecimientoRequest : IRequest<GetEstablecimientoResponse>
    {
        public Guid Id { get; set; }
    }
}
