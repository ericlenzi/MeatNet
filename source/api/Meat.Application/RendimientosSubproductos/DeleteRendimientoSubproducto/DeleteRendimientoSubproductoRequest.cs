using MediatR;
using System;

namespace Meat.Application.RendimientosSubproductos.DeleteRendimientoSubproducto
{
    public class DeleteRendimientoSubproductoRequest : IRequest<DeleteRendimientoSubproductoResponse>
    {
        public Guid Id { get; set; }
    }
}
