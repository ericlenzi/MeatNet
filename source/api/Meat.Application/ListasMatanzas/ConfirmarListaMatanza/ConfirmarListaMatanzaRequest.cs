using Meat.Application.Shared;
using MediatR;
using System;

namespace Meat.Application.ListasMatanzas.ConfirmarListaMatanza
{
    public class ConfirmarListaMatanzaRequest : RequestBase, IRequest<ConfirmarListaMatanzaResponse>
    {
        public Guid Id { get; set; }

    }

    public class ConfirmarListaMatanzaResponse
    {
    }
}
