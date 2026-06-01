using MediatR;
using System;

namespace Meat.Application.Establecimientos.DeleteEstablecimiento
{
    public class DeleteEstablecimientoRequest : IRequest<DeleteEstablecimientoResponse>
    {
        public Guid Id { get; set; }
    }
}
