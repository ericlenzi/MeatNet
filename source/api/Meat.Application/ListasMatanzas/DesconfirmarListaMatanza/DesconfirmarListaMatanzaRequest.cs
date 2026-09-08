using Meat.Application.Shared;
using MediatR;
using System;

namespace Meat.Application.ListasMatanzas.DesconfirmarListaMatanza
{
    public class DesconfirmarListaMatanzaRequest : RequestBase, IRequest<DesconfirmarListaMatanzaResponse>
    {
        public Guid Id { get; set; }

    }

    public class DesconfirmarListaMatanzaResponse
    {
    }
}
