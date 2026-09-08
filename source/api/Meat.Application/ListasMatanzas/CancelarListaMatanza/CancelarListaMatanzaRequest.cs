using Meat.Application.Shared;
using MediatR;
using System;

namespace Meat.Application.ListasMatanzas.CancelarListaMatanza
{
    public class CancelarListaMatanzaRequest : RequestBase, IRequest<CancelarListaMatanzaResponse>
    {
        public Guid Id { get; set; }
        public string Motivo { get; set; }

    }

    public class CancelarListaMatanzaResponse
    {
    }
}
