using Meat.Application.Shared;
using MediatR;
using System;

namespace Meat.Application.Almacenes.DeleteAlmacen
{
    public class DeleteAlmacenRequest : RequestBase, IRequest<DeleteAlmacenResponse>
    {
        public Guid Id { get; set; }

    }
}
