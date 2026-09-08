using Meat.Application.Shared;
using MediatR;
using System;

namespace Meat.Application.ListasMatanzas.DeleteListaMatanza
{
    public class DeleteListaMatanzaRequest : RequestBase, IRequest<DeleteListaMatanzaResponse>
    {
        public Guid Id { get; set; }

    }

    public class DeleteListaMatanzaResponse
    {
    }
}
