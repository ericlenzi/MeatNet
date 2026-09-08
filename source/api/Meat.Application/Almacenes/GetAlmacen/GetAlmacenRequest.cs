using Meat.Application.Shared;
using MediatR;
using System;

namespace Meat.Application.Almacenes.GetAlmacen
{
    public class GetAlmacenRequest : RequestBase, IRequest<GetAlmacenResponse>
    {
        public Guid Id { get; set; }

    }
}
