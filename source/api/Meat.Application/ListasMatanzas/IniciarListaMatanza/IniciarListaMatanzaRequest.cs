using Meat.Application.Shared;
using MediatR;
using System;

namespace Meat.Application.ListasMatanzas.IniciarListaMatanza
{
    public class IniciarListaMatanzaRequest : RequestBase, IRequest<IniciarListaMatanzaResponse>
    {
        public Guid Id { get; set; }

    }

    public class IniciarListaMatanzaResponse
    {
    }
}
