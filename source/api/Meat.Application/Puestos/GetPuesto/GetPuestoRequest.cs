using MediatR;
using System;

namespace Meat.Application.Puestos.GetPuesto
{
    public class GetPuestoRequest : IRequest<GetPuestoResponse>
    {
        public Guid Id { get; set; }
    }
}
