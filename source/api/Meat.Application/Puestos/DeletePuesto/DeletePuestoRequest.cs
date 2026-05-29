using MediatR;
using System;

namespace Meat.Application.Puestos.DeletePuesto
{
    public class DeletePuestoRequest : IRequest<DeletePuestoResponse>
    {
        public Guid Id { get; set; }
    }
}
