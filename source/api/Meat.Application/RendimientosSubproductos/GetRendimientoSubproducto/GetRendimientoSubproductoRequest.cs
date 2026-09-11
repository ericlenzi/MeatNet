using MediatR;
using System;

namespace Meat.Application.RendimientosSubproductos.GetRendimientoSubproducto
{
    public class GetRendimientoSubproductoRequest : IRequest<GetRendimientoSubproductoResponse>
    {
        public Guid Id { get; set; }
    }
}
